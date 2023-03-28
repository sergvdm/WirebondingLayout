namespace Altium.Geometry2D
{
    public class TranslateTransform : TransformBase
    {
        private Vector2D v;

        public TranslateTransform(Vector2D v)
        {
            this.v = v;
        }

        protected override Point2D ForwardCore(Point2D point) => point + v;
        protected override Point2D ReverseCore(Point2D point) => point - v;
    }
}
