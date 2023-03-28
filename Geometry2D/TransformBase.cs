using Altium.Geometry2D;

namespace Altium.Geometry2D
{
    public abstract class TransformBase : ITransform
    {
        public Point2D Forward(Point2D point) => ForwardCore(point);
        public Point2D Reverse(Point2D point) => ReverseCore(point);
        protected abstract Point2D ForwardCore(Point2D point);
        protected abstract Point2D ReverseCore(Point2D point);
    }
}
