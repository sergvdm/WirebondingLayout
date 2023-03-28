using Altium.Geometry2D.Shapes;

namespace Altium.Geometry2D
{
    internal abstract class GeometryPathSegment : OpenShape, IGeometryPathSegment
    {
        protected GeometryPathSegment()
        {
        }

        protected GeometryPathSegment(GeometryPathSegment other)
            : base(other)
        {
        }

        public abstract IGeometryPathSegment SnapToGrid(IGeometry2DEngine geometry2DEngine);
        public abstract GeometryPathVertex Split(Point2D point);
        public abstract IGeometryPathSegment TryJoin(IGeometry2DEngine geometry2DEngine, IGeometryPathSegment otherSegment, PathSegmentJoinMode otherSegmentJoinMode);
        protected abstract GeometryPathSegment ReverseImpl();
        IGeometryPathSegment IGeometryPathSegment.Reverse() => ReverseImpl();
        public abstract int WindingNumber(IGeometry2DEngine geometry2DEngine, Point2D point);
        IGeometryPathSegment IGeometryPathSegment.Clone() => (IGeometryPathSegment)CloneImpl();
    }
}
