using Altium.Geometry2D;

namespace Altium.Geometry2D
{
    public class RotateTransform : TransformBase
    {
        private Point2D origin;
        private double angle;

        public RotateTransform(Point2D origin, double angle)
        {
            this.origin = origin;
            this.angle = angle;
        }

        protected override Point2D ForwardCore(Point2D point) => origin + (point - origin).Rotate(angle);
        protected override Point2D ReverseCore(Point2D point) => origin + (point - origin).Rotate(-angle);
    }
}
