namespace Altium.Geometry2D.Shapes
{
    internal abstract class LimitedShape : Shape, ILimitedShape
    {
        protected LimitedShape()
        {
        }

        protected LimitedShape(LimitedShape other)
            : base(other)
        {
            boundingRect = other.boundingRect;
        }

        private AARectD? boundingRect;
        public AARectD BoundingRect
        {
            get
            {
                if (!boundingRect.HasValue)
                    boundingRect = CalcBoundingRect();
                return boundingRect.Value;
            }
        }

        protected void InvalidateBoundingRect()
        {
            boundingRect = null;
        }

        public virtual IRegion CreateRegion(IGeometry2DEngine geometry2DEngine, bool invert = false)
        {
            var regionBuilder = geometry2DEngine.CreateRegionBuilder();
            var pathBuilder = geometry2DEngine.CreatePathBuilder();
            regionBuilder.BeginRegion();
            BuildRegion(geometry2DEngine, regionBuilder, pathBuilder, invert);
            return regionBuilder.EndRegion();
        }

        protected abstract AARectD CalcBoundingRect();

        protected virtual void BuildRegion(IGeometry2DEngine geometry2DEngine, IRegionBuilder regionBuilder, IGeometryPathBuilder pathBuilder, bool invert)
        {
        }
    }
}