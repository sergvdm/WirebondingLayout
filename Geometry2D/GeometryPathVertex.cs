namespace Altium.Geometry2D
{
    public struct GeometryPathVertex
    {
        public GeometryPathVertex(IGeometryPathSegment prevSegment, IGeometryPathSegment nextSegment)
        {
            PrevSegment = prevSegment;
            NextSegment = nextSegment;
        }

        public IGeometryPathSegment PrevSegment;
        public IGeometryPathSegment NextSegment;
        public Point2D Point => PrevSegment != null ? PrevSegment.EndPoint : NextSegment != null ? NextSegment.StartPoint : default(Point2D);

        public override string ToString()
        {
            return $"PV({Point})";
        }
    }
}
