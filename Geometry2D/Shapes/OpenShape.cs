namespace Altium.Geometry2D.Shapes
{
    internal abstract class OpenShape : LimitedShape, IOpenShape
    {
        protected OpenShape()
        {
        }

        protected OpenShape(OpenShape other)
            : base(other)
        {
            StartPoint = other.StartPoint;
            EndPoint = other.EndPoint;
        }

        public Point2D StartPoint { get; protected set; }
        public Point2D EndPoint { get; protected set; }

        public abstract Point2D Midpoint();

        public abstract bool IsSinglePoint();
        public abstract double Length();
        public abstract double LengthFromStart(Point2D point);
        public abstract Vector2D Tangent(Point2D point);
        public abstract double Curvature(Point2D point);
    }
}
