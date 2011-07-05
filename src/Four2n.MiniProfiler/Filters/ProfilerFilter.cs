// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilerFilter.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 42n.pl All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfilerFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler.Filters
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using global::Orchard;
    using global::Orchard.DisplayManagement;
    using global::Orchard.Mvc.Filters;
    using global::Orchard.Security;
    using global::Orchard.UI.Admin;

    /// <summary>
    /// Filter for injecting profiler view code.
    /// </summary>
    public class ProfilerFilter : FilterProvider, IResultFilter
    {
        private readonly WorkContext workContext;
        private readonly IAuthorizer authorizer;
        private readonly dynamic shapeFactory;

        public ProfilerFilter(WorkContext workContext, IAuthorizer authorizer, IShapeFactory shapeFactory)
        {
            this.workContext = workContext;
            this.shapeFactory = shapeFactory;
            this.authorizer = authorizer;
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
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

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
    }
}