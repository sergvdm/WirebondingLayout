namespace Altium.Geometry2D.Shapes
{
    public interface IRectangle : IPolygon, IRectShape
    {
        new IRectangle Clone();
    }
}
