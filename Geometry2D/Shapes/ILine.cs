namespace Altium.Geometry2D.Shapes
{
    public interface ILine : IGeometryPathSegment, ILinearShape
    {
        IRay AsRay(bool reverse = false);
        IInfiniteLine AsInfinite();
        new ILine Clone();
        new ILine Reverse();
    }
}
