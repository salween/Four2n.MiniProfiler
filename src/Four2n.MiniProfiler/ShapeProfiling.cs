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
    using Four2n.Orchard.MiniProfiler.Services;
    using global::Orchard.ContentManagement;

    public class ShapeProfiling : IShapeFactoryEvents
    {
        private readonly IProfilerService _profiler;
        public ShapeProfiling(IProfilerService profiler)
        {
            _profiler = profiler;
        }

        public void Creating(ShapeCreatingContext context)
        {
        }

        public void Created(ShapeCreatedContext context)
        {
            var shapeMetadata = (ShapeMetadata)context.Shape.Metadata;
       /*     if (shapeMetadata.Type.Equals("Zone") || context.Shape.ContentItem == null)
            {
                return;
            }*/

            shapeMetadata.OnDisplaying(this.OnDisplaying);
            shapeMetadata.OnDisplayed(this.OnDisplayed);
        }

        public void OnDisplaying(ShapeDisplayingContext context)
        {
            Debug.WriteLine(string.Format("[Four2n.MiniProfiler] - ShapeProfiling - Displaying {0}",  context.ShapeMetadata.Type + " - Display"));
            IContent content = null;
            if (context.Shape.ContentItem != null) {
                content = context.Shape.ContentItem;
            }
            else if (context.Shape.ContentPart != null) {
                content = context.Shape.ContentPart;
            }
            var message = String.Format("Shape Display: {0} ({1}) ({2})",
                context.ShapeMetadata.Type,context.ShapeMetadata.DisplayType,
                (string)(content != null ? content.ContentItem.ContentType : "non-content"));
            _profiler.StepStart(string.Concat((string)context.Shape.ToString(), "Dis"), message,true);
        }

        public void OnDisplayed(ShapeDisplayedContext context)
        {
            Debug.WriteLine(string.Format("[Four2n.MiniProfiler] - ShapeProfiling - Displayed {0}", context.ShapeMetadata.Type + " - Display"));
            _profiler.StepStop(string.Concat((string)context.Shape.ToString(), "Dis"));
        }
    }
}