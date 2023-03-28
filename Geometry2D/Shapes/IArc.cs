namespace Altium.Geometry2D.Shapes
{
    public interface IArc : IGeometryPathSegment, ICircularShape
    {
        double StartAngle { get; }
        double EndAngle { get; }
        double SweepAngle { get; }
        new IArc Clone();
        new IArc Reverse();
    }
}
