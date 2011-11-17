// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfiledSqlServerCeDriver.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfiledSqlServerCeDriver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler.Data.Providers
{
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;

    using MvcMiniProfiler.Data;

    using global::Orchard.Data.Providers;

    public class ProfiledSqlServerCeDriver : SqlCeDataServicesProvider.OrchardSqlServerCeDriver
    {
        public override IDbCommand CreateCommand()
        {
            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlServerCeDriver - CreateCommand ");
            if (MvcMiniProfiler.MiniProfiler.Current == null)
            {
                return base.CreateCommand();
            }

            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlServerCeDriver - CreateCommand Profiling");
            return new ProfiledDbCommand(
                base.CreateCommand() as DbCommand,
                null,
                MvcMiniProfiler.MiniProfiler.Current);
        }

        public override IDbConnection CreateConnection()
        {
            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlServerCeDriver - CreateConnection ");
            if (MvcMiniProfiler.MiniProfiler.Current == null)
            {
                return base.CreateConnection();
            }

            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlServerCeDriver - CreateConnection Profiling");
            return new ProfiledDbConnection(
                base.CreateConnection() as DbConnection,
                MvcMiniProfiler.MiniProfiler.Current);
        }
    }
}