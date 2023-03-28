namespace Altium.Geometry2D.Shapes
{
    internal class InfiniteLine : Shape, IInfiniteLine
    {
        public InfiniteLine(Point2D point1, Point2D point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public InfiniteLine(Point2D point, Vector2D vector)
            : this(point, point + vector)
        {
        }

        protected InfiniteLine(InfiniteLine other)
            : base(other)
        {
            Point1 = other.Point1;
            Point2 = other.Point2;
        }

        public Point2D Point1 { get; }
        public Point2D Point2 { get; }
        public Vector2D Vector => (Point2 - Point1).Unit();

        public InfiniteLine Ortogonal()
        {
            return new InfiniteLine(Point1, Vector.Orthogonal());
        }

        public override bool Equal(IShape obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (ReferenceEquals(obj, null) || !(obj is InfiniteLine other)) return false;
            return Point1 == other.Point1 && Vector == other.Vector;
        }

        public InfiniteLine Clone() => new InfiniteLine(this);
        protected override Shape CloneImpl() => Clone();
        IInfiniteLine IInfiniteLine.Clone() => Clone();

        public override string ToString()
        {
            return $"InfLine({Point1},{Vector})";
        }
    }
}