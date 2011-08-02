// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfiledSqlClientDriver.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfiledSqlClientDriver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler.Data.Providers
{
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;

    using MvcMiniProfiler.Data;

    using NHibernate.Driver;

    public class ProfiledSqlClientDriver : SqlClientDriver
    {
        public override IDbCommand CreateCommand()
        {
            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlClientDriver - CreateCommand ");
            if (MvcMiniProfiler.MiniProfiler.Current == null)
            {
                return base.CreateCommand();
            }
            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlClientDriver - CreateCommand  Profiling");
            return new ProfiledDbCommand(
                base.CreateCommand() as DbCommand,
                null,
                MvcMiniProfiler.MiniProfiler.Current);
        }

        public override IDbConnection CreateConnection()
        {
            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlClientDriver - CreateConnection ");
            if (MvcMiniProfiler.MiniProfiler.Current == null)
            {
                return base.CreateConnection();
            }

            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlClientDriver - CreateConnection Profiling");
            return ProfiledDbConnection.Get(
                base.CreateConnection() as DbConnection,
                MvcMiniProfiler.MiniProfiler.Current);
        }
    }
}