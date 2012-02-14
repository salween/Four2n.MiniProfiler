namespace Four2n.Orchard.MiniProfiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;

    using StackExchange.Profiling;

    using global::Orchard;
    using global::Orchard.DisplayManagement.Implementation;
    using global::Orchard.DisplayManagement.Shapes;

    public class ShapeProfiling : IShapeFactoryEvents, IShapeDisplayEvents
    {
        private IOrchardServices services;

        public ShapeProfiling(IOrchardServices services)
        {
            this.services = services;
        }

        public void Creating(ShapeCreatingContext context)
        {
        }

        public void Created(ShapeCreatedContext context)
        {
            var shapeMetadata = (ShapeMetadata)context.Shape.Metadata;
/*
            if (shapeMetadata.Type.Equals("Zone") || context.Shape.ContentItem == null)
            {
                return;
            }
*/

            shapeMetadata.OnDisplaying(this.OnDisplaying);
            shapeMetadata.OnDisplayed(this.OnDisplayed);
        }

        public void Displaying(ShapeDisplayingContext context)
        {
            if (context.ShapeMetadata.Type.Equals("Zone"))
            {
                return;
            }
            Debug.WriteLine("[Four2n.MiniProfiler] - ShapeProfiling - Displaying ");
            MiniProfiler.Current.StepStart((string)context.Shape.ToString() + "Dis", services.WorkContext.HttpContext, context.ShapeMetadata.Type + " - Display");
        }

        public void Displayed(ShapeDisplayedContext context)
        {
            if (context.ShapeMetadata.Type.Equals("Zone"))
            {
                return;
            }
            Debug.WriteLine("[Four2n.MiniProfiler] - ShapeProfiling - Displayed ");
            MiniProfiler.Current.StepStop((string)context.Shape.ToString() + "Dis", services.WorkContext.HttpContext);
        }

        public void OnDisplaying(ShapeDisplayingContext context)
        {
            Debug.WriteLine(string.Format("[Four2n.MiniProfiler] - ShapeProfiling - Displaying {0}",  context.ShapeMetadata.Type + " - Display"));
            var message = string.Concat(
                context.ShapeMetadata.Type,
                " ",
                (string)(context.Shape.ContentItem != null ? context.Shape.ContentItem.ContentType : null),
                " Display");
            MiniProfiler.Current.StepStart(string.Concat((string)context.Shape.ToString(), "Dis"), services.WorkContext.HttpContext, message);
        }

        public void OnDisplayed(ShapeDisplayedContext context)
        {
            Debug.WriteLine(string.Format("[Four2n.MiniProfiler] - ShapeProfiling - Displayed {0}", context.ShapeMetadata.Type + " - Display"));
            MiniProfiler.Current.StepStop(string.Concat((string)context.Shape.ToString(), "Dis"), services.WorkContext.HttpContext);
        }
    }
}