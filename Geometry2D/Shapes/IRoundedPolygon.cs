namespace Altium.Geometry2D.Shapes
{
    public interface IRoundedPolygon : IPolygon
    {
        double Rounding { get; }
        ShapeRoundingMode RoundingMode { get; }
        new IRoundedPolygon Clone();
    }
}
