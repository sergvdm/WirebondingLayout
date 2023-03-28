using Altium.Geometry2D;
using Altium.Geometry2D.Shapes;

namespace Altium.Geometry2D
{
    public class MirrorByLineTransform : TransformBase
    {
        private ILinearShape line;

        public MirrorByLineTransform(ILinearShape line)
        {
            this.line = line;
        }

        protected override Point2D ForwardCore(Point2D point) => point.MirrorBy(line);
        protected override Point2D ReverseCore(Point2D point) => point.MirrorBy(line);
    }
}
