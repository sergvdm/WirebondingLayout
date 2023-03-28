namespace Altium.Geometry2D
{
    public interface ITransform
    {
        Point2D Forward(Point2D point);
        Point2D Reverse(Point2D point);
    }
}
