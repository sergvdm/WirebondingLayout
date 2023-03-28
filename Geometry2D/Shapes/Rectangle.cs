namespace Altium.Geometry2D.Shapes
{
    internal class Rectangle : Polygon, IRectangle
    {
        public Rectangle(Point2D vertex0, Point2D vertex2, double rotation)
            : this(new RectInitData(vertex0, vertex2, rotation))
        {
        }

        public Rectangle(Point2D center, double width, double height, double rotation)
            : this(new RectInitData(center, width, height, rotation))
        {
        }

        internal Rectangle(RectInitData rectInitData)
            : base(rectInitData.Vertices)
        {
            Center = rectInitData.Center;
            Rotation = rectInitData.Rotation;
            Width = rectInitData.Width;
            Height = rectInitData.Height;
        }

        protected Rectangle(Rectangle other)
            : base(other)
        {
            Center = other.Center;
            Rotation = other.Rotation;
            Width = other.Width;
            Height = other.Height;
        }

        public Point2D Center { get; }
        public double Rotation { get; }
        public double Width { get; }
        public double Height { get; }
        public double DiagonalLength => Vertices[0].DistanceTo(Vertices[2]);

        public override bool Equal(IShape obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (ReferenceEquals(obj, null) || !(obj is Rectangle other)) return false;
            return Center == other.Center && Rotation == other.Rotation && Width == other.Width && Height == other.Height;
        }

        public new Rectangle Clone() => new Rectangle(this);
        protected override Shape CloneImpl() => Clone();
        IRectangle IRectangle.Clone() => Clone();
        public override double Area() => Width * Height;

        public override string ToString()
        {
            return $"Rect(C={Center},W={Width},H={Height},R={Rotation},V0={Vertices[0]},V1={Vertices[1]},V2={Vertices[2]},V3={Vertices[3]})";
        }
    }

    internal class RectInitData
    {
        public RectInitData(Point2D vertex0, Point2D vertex2, double rotation)
        {
            Rotation = rotation.NormalizeRadAngle();
            Center = new Line(vertex0, vertex2).Midpoint();
            var centerLine = new InfiniteLine(Center, new Vector2D(rotation));
            var vertex1 = vertex2.MirrorBy(centerLine);
            var vertex3 = vertex0.MirrorBy(centerLine);
            Vertices = new Point2D[4];
            Vertices[0] = vertex0;
            Vertices[1] = vertex1;
            Vertices[2] = vertex2;
            Vertices[3] = vertex3;
            Width = vertex0.DistanceTo(vertex1);
            Height = vertex1.DistanceTo(vertex2);
        }

        public RectInitData(Point2D center, double width, double height, double rotation)
        {
            Center = center;
            Width = width;
            Height = height;
            Rotation = rotation.NormalizeRadAngle();
            var v1 = Width * 0.5 * new Vector2D(Rotation);
            var v2 = Height * 0.5 * new Vector2D(Rotation + Math2D.HPI);
            Vertices = new Point2D[4];
            Vertices[0] = Center + (-v1 - v2);
            Vertices[1] = Center + (v1 - v2);
            Vertices[2] = Center + (v1 + v2);
            Vertices[3] = Center + (-v1 + v2);
        }

        public Point2D Center { get; }
        public double Rotation { get; }
        public double Width { get; }
        public double Height { get; }
        public Point2D[] Vertices { get; }
    }
}