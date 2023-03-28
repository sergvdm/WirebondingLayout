namespace Altium.Geometry2D.Shapes
{
    public interface ILinearShape
    {
        Point2D Point1 { get; }
        Point2D Point2 { get; }
        Vector2D Vector { get; }
    }
}
