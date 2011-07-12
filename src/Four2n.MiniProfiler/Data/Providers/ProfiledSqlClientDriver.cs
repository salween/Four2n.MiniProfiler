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

    using MvcMiniProfiler.Data;

    using NHibernate.Driver;

    public class ProfiledSqlClientDriver : SqlClientDriver
    {
        public override IDbCommand CreateCommand()
        {
            return new ProfiledDbCommand(
                base.CreateCommand() as DbCommand,
                null,
                MvcMiniProfiler.MiniProfiler.Current);
        }

        public override IDbConnection CreateConnection()
        {
            return ProfiledDbConnection.Get(
                base.CreateConnection() as DbConnection,
                MvcMiniProfiler.MiniProfiler.Current);
        }
    }
}