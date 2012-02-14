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
    using System.Web.Mvc;
    using System.Web.Routing;

    using StackExchange.Profiling;

    using global::Orchard;
    using global::Orchard.DisplayManagement;
    using global::Orchard.Mvc.Filters;
    using global::Orchard.Security;
    using global::Orchard.UI.Admin;
    using Four2n.Orchard.MiniProfiler.Services;

    /// <summary>
    /// Filter for injecting profiler view code.
    /// </summary>
    public class ProfilerFilter : FilterProvider, IResultFilter, IActionFilter
    {
        #region Constants and Fields

        private const string ActionKey = "G:ActExec";

        private const string ResultKey = "G:ResExec";

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
            _profiler.StepStop(ActionKey);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _profiler.StepStart(
                ActionKey,
                string.Format("Action: {0}.{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName));
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