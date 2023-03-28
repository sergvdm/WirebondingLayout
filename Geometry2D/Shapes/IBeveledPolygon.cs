namespace Altium.Geometry2D.Shapes
{
    public interface IBeveledPolygon : IPolygon
    {
        double Beveling { get; }
        ShapeBevelingMode BevelingMode { get; }
        new IBeveledPolygon Clone();
    }
}
