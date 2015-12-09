// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoorMansTSqlFormatter.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the PoorMansTSqlFormatter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using PoorMansTSqlFormatterLib;
using PoorMansTSqlFormatterLib.Formatters;
using StackExchange.Profiling;
using StackExchange.Profiling.SqlFormatters;

namespace Four2n.Orchard.MiniProfiler.Formatters {
    public class PoorMansTSqlFormatter : ISqlFormatter {
        public string FormatSql(string commandText, List<SqlTimingParameter> parameters) {

            var sqlFormatter = new SqlServerFormatter();
            var sqlFormat = sqlFormatter.GetFormattedSql(commandText,parameters);

            var poorMansFormatter = new TSqlStandardFormatter();
            var fullFormatter = new SqlFormattingManager(poorMansFormatter);
            return fullFormatter.Format(sqlFormat);
        }
    }
}