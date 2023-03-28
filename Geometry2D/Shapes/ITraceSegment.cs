namespace Altium.Geometry2D.Shapes
{
    public interface ITraceSegment : IClosedShape
    {
        IGeometryPathSegment Guide { get; }
        double Width { get; }
        new ITraceSegment Clone();
    }
}
