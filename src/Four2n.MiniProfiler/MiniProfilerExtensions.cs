// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MiniProfilerExtensions.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the MiniProfilerExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler
{
    using System;
    using System.Web;

    using MvcMiniProfiler;

    public static class MiniProfilerExtensions
    {
        public static void StepStart(this MiniProfiler profiler, string key, HttpContextBase httpContext, string message)
        {
            var step = profiler.Step(message);
            httpContext.Items[key] = step;
        }

        public static void StepStop(this MiniProfiler profiler, string key, HttpContextBase httpContext)
        {
            var step = httpContext.Items[key] as IDisposable;
            if (step != null)
            {
                step.Dispose();
            }
        }
    }
}