// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilerStorage.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using StackExchange.Profiling;
    using StackExchange.Profiling.Storage;

    public class ProfilerStorage : IStorage
    {
      /// <summary>
        /// The string that prefixes all keys that MiniProfilers are saved under, e.g.
        /// "mini-profiler-ecfb0050-7ce8-4bf1-bf82-2cb38e90e31e".
        /// </summary>
        public const string CacheKeyPrefix = "mini-profiler-";

        /// <summary>
        /// How long to cache each <see cref="MiniProfiler"/> for (i.e. the absolute expiration parameter of 
        /// <see cref="System.Web.Caching.Cache.Insert(string, object, System.Web.Caching.CacheDependency, System.DateTime, System.TimeSpan, System.Web.Caching.CacheItemUpdateCallback)"/>)
        /// </summary>
        public TimeSpan CacheDuration { get; set; }

        /// <summary>
        /// Returns a new HttpRuntimeCacheStorage class that will cache MiniProfilers for the specified duration.
        /// </summary>
        public ProfilerStorage(TimeSpan cacheDuration)
        {
            CacheDuration = cacheDuration;
        }

        /// <summary>
        /// Saves <paramref name="profiler"/> to the HttpRuntime.Cache under a key concated with <see cref="CacheKeyPrefix"/>
        /// and the parameter's <see cref="MiniProfiler.Id"/>.
        /// </summary>
        public void Save(MiniProfiler profiler)
        {
            InsertIntoCache(GetCacheKey(profiler.Id), profiler);
        }

        /// <summary>
        /// remembers we did not view the profile
        /// </summary>
        public void SetUnviewed(string user, Guid id)
        { 
            var ids = GetPerUserUnviewedIds(user);
            lock (ids)
            {
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                    if (ids.Count > 20)
                    {
                        ids.RemoveAt(0);
                    }
                }
            }
        }

        /// <summary>
        /// Set the profile to viewed for this user
        /// </summary>
        public void SetViewed(string user, Guid id)
        {
            var ids = GetPerUserUnviewedIds(user);

            lock (ids)
            {
                ids.Remove(id);
            }
        }

        /// <summary>
        /// Returns the saved <see cref="MiniProfiler"/> identified by <paramref name="id"/>. Also marks the resulting
        /// profiler <see cref="MiniProfiler.HasUserViewed"/> to true.
        /// </summary>
        public MiniProfiler Load(Guid id)
        {
            var result = HttpRuntime.Cache[GetCacheKey(id)] as MiniProfiler;
            return result;
        }

        /// <summary>
        /// Returns a list of <see cref="MiniProfiler.Id"/>s that haven't been seen by <paramref name="user"/>.
        /// </summary>
        /// <param name="user">User identified by the current <see cref="MiniProfiler.Settings.UserProvider"/>.</param>
        public List<Guid> GetUnviewedIds(string user)
        {
            var ids = GetPerUserUnviewedIds(user);
            lock (ids)
            {
                return new List<Guid>(ids);
            }
        }

        private void InsertIntoCache(string key, object value)
        {
            // use insert instead of add; add fails if the item already exists
            HttpRuntime.Cache.Insert(
                key: key,
                value: value,
                dependencies: null,
                absoluteExpiration: DateTime.Now.Add(CacheDuration), // servers will cache based on local now
                slidingExpiration: System.Web.Caching.Cache.NoSlidingExpiration,
                priority: System.Web.Caching.CacheItemPriority.Low,
                onRemoveCallback: null);
        }

        private string GetCacheKey(Guid id)
        {
            return CacheKeyPrefix + id;
        }

        private string GetPerUserUnviewedCacheKey(string user)
        {
            return CacheKeyPrefix + "unviewed-for-user-" + user;
        }

        private List<Guid> GetPerUserUnviewedIds(MiniProfiler profiler)
        {
            return GetPerUserUnviewedIds(profiler.User);
        }

        private List<Guid> GetPerUserUnviewedIds(string user)
        {
            var key = GetPerUserUnviewedCacheKey(user);
            var result = HttpRuntime.Cache[key] as List<Guid>;

            if (result == null)
            {
                lock (AddPerUserUnviewedIdsLock)
                {
                    // check again, as we could have been waiting
                    result = HttpRuntime.Cache[key] as List<Guid>;
                    if (result == null)
                    {
                        result = new List<Guid>();
                        InsertIntoCache(key, result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Syncs access to runtime cache when adding a new list of ids for a user.
        /// </summary>
        private static readonly object AddPerUserUnviewedIdsLock = new object();
    }
}