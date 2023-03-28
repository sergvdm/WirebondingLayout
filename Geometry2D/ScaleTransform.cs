using Altium.Geometry2D;

namespace Altium.Geometry2D
{
    public class ScaleTransform : TransformBase
    {
        private Point2D origin;
        private double scale;

        public ScaleTransform(Point2D origin, double scale)
        {
            this.origin = origin;
            this.scale = scale;
        }

        protected override Point2D ForwardCore(Point2D point) => origin + (point - origin) * scale;
        protected override Point2D ReverseCore(Point2D point) => origin + (point - origin) / scale;
    }
}
