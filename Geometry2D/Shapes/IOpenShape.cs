namespace Altium.Geometry2D.Shapes
{
    public interface IOpenShape : ILimitedShape
    {
        Point2D StartPoint { get; }
        Point2D EndPoint { get; }
        Point2D Midpoint();
        bool IsSinglePoint();
        double Length();
        double LengthFromStart(Point2D point);
        Vector2D Tangent(Point2D point);
        double Curvature(Point2D point);
    }
}
