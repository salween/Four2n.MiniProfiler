// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilerFilter.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Filter for injecting profiler view code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using global::Orchard;
    using global::Orchard.DisplayManagement;
    using global::Orchard.Mvc.Filters;
    using global::Orchard.Security;
    using global::Orchard.UI.Admin;
    using Four2n.Orchard.MiniProfiler.Services;

    using StackExchange.Profiling;

    /// <summary>
    /// Filter for injecting profiler view code.
    /// </summary>
    public class ProfilerFilter : FilterProvider, IResultFilter, IActionFilter
    {
        #region Constants and Fields

        private const string ActionKey = "G:ActExec";

        private const string ResultKey = "G:ResExec";

        private const string stackKey = "ProfilingActionFilterStack";

        private readonly IAuthorizer authorizer;
        private readonly dynamic shapeFactory;

        private readonly WorkContext workContext;
        private readonly IProfilerService _profiler;

        #endregion

        #region Constructors and Destructors

        public ProfilerFilter(WorkContext workContext, IAuthorizer authorizer, IShapeFactory shapeFactory, IProfilerService profiler)
        {
            this.workContext = workContext;
            this.shapeFactory = shapeFactory;
            this.authorizer = authorizer;
            _profiler = profiler;
        }

        #endregion

        #region Public Methods

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var stack = HttpContext.Current.Items[stackKey] as Stack<IDisposable>;
            if (stack != null && stack.Count > 0)
            {
                stack.Pop().Dispose();
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var mp = MiniProfiler.Current;
            if (mp != null)
            {
                var stack = HttpContext.Current.Items[stackKey] as Stack<IDisposable>;
                if (stack == null)
                {
                    stack = new Stack<IDisposable>();
                    HttpContext.Current.Items[stackKey] = stack;
                }

                var profiler = MiniProfiler.Current;
                if (profiler != null)
                {
                    var tokens = filterContext.RouteData.DataTokens;
                    string area = tokens.ContainsKey("area") && !string.IsNullOrEmpty(tokens["area"].ToString()) ?
                        string.Concat(tokens["area"], ".") :
                        string.Empty;
                    string controller = string.Concat(filterContext.Controller.ToString().Split('.').Last(), ".");
                    string action = filterContext.ActionDescriptor.ActionName;

                    stack.Push(profiler.Step("Controller: " + area + controller + action));
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult)) {
                return;
            }

            if (!this.IsActivable()) {
                return;
            }

            _profiler.StepStop(ResultKey);
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
            {
                return;
            }

            if (!this.IsActivable())
            {
                return;
            }

            var head = this.workContext.Layout.Head;
            head.Add(this.shapeFactory.MiniProfilerTemplate());

            _profiler.StepStart(ResultKey, string.Format("Result: {0}", filterContext.Result.ToString()));
        }

        #endregion

        #region Methods

        private bool IsActivable()
        {
            // activate on front-end only
            if (AdminFilter.IsApplied(new RequestContext(this.workContext.HttpContext, new RouteData())))
            {
                return false;
            }

            // if not logged as a site owner, still activate if it's a local request (development machine)
            if (!this.authorizer.Authorize(StandardPermissions.SiteOwner))
            {
                return this.workContext.HttpContext.Request.IsLocal;
            }

            return true;
        }

        #endregion
    }
}