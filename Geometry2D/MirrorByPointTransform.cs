using Altium.Geometry2D;

namespace Altium.Geometry2D
{
    public class MirrorByPointTransform : TransformBase
    {
        private Point2D point;

        public MirrorByPointTransform(Point2D point)
        {
            this.point = point;
        }

        protected override Point2D ForwardCore(Point2D point) => point.MirrorBy(point);
        protected override Point2D ReverseCore(Point2D point) => point.MirrorBy(point);
    }
}
