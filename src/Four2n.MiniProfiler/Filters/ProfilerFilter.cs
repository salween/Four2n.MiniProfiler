namespace Four2n.Orchard.MiniProfiler.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using global::Orchard;
    using global::Orchard.DisplayManagement;
    using global::Orchard.Mvc.Filters;
    using global::Orchard.Security;
    using global::Orchard.UI.Admin;

    using MiniProfiler = MvcMiniProfiler.MiniProfiler;

    public class ProfilerFilter : FilterProvider, IActionFilter, IResultFilter
    {
        private readonly WorkContext _workContext;
        private readonly IAuthorizer _authorizer;
        private readonly dynamic _shapeFactory;

        public ProfilerFilter(WorkContext workContext, IAuthorizer authorizer, IShapeFactory shapeFactory)
        {
            this._workContext = workContext;
            this._shapeFactory = shapeFactory;
            this._authorizer = authorizer;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MiniProfiler.Start();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
                return;

            if (!IsActivable())
            {
                return;
            }

            var head = _workContext.Layout.Head;
            head.Add(_shapeFactory.MiniProfilerTemplate());
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            MiniProfiler.Stop();
        }

        private bool IsActivable()
        {
            // activate on front-end only
            if (AdminFilter.IsApplied(new RequestContext(_workContext.HttpContext, new RouteData())))
                return false;

            // if not logged as a site owner, still activate if it's a local request (development machine)
            if (!_authorizer.Authorize(StandardPermissions.SiteOwner))
                return _workContext.HttpContext.Request.IsLocal;

            return true;
        }
    }
}