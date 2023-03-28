using Altium.Geometry2D;
using System.Linq;

namespace Altium.Geometry2D
{
    public class TransformChain : TransformBase
    {
        private ITransform[] transforms;

        public TransformChain(params ITransform[] transforms)
        {
            this.transforms = transforms;
        }

        protected override Point2D ForwardCore(Point2D point) => transforms.Aggregate(point, (pt, tr) => tr.Forward(pt));
        protected override Point2D ReverseCore(Point2D point) => transforms.Reverse().Aggregate(point, (pt, tr) => tr.Reverse(pt));
    }
}
