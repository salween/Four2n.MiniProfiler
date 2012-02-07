﻿// --------------------------------------------------------------------------------------------------------------------
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

    using StackExchange.Profiling.Data;

    using NHibernate.Driver;

    public class ProfiledSqlClientDriver : SqlClientDriver
    {
        public override IDbCommand CreateCommand()
        {
            var command = base.CreateCommand();
            if (StackExchange.Profiling.MiniProfiler.Current != null)
            {
                Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlClientDriver - CreateCommand ");
                command = new ProfiledDbCommand((DbCommand)command,
                null,
                StackExchange.Profiling.MiniProfiler.Current);
                Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlClientDriver - CreateCommand  Profiling");
            }
            return command;
        }

        public override IDbConnection CreateConnection()
        {
            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlClientDriver - CreateConnection ");
            if (StackExchange.Profiling.MiniProfiler.Current == null)
            {
                return base.CreateConnection();
            }

            Debug.WriteLine("[Four2n.MiniProfiler] - ProfiledSqlClientDriver - CreateConnection Profiling");
            return new ProfiledDbConnection(
                base.CreateConnection() as DbConnection,
                StackExchange.Profiling.MiniProfiler.Current);
        }
    }
}