namespace Altium.Geometry2D.Shapes
{
    internal class RoundedRectangle : RoundedPolygon, IRoundedRectangle
    {
        public RoundedRectangle(Point2D vertex0, Point2D vertex2, double rounding, ShapeRoundingMode roundingMode, double rotation)
            : this(new RoundedRectInitData(vertex0, vertex2, rounding, roundingMode, rotation))
        {
        }

        public RoundedRectangle(Point2D center, double width, double height, double rounding, ShapeRoundingMode roundingMode, double rotation)
            : this(new RoundedRectInitData(center, width, height, rounding, roundingMode, rotation))
        {
        }

        internal RoundedRectangle(RoundedRectInitData roundedRectInitData)
            : base(roundedRectInitData.RoundedPolyInitData)
        {
            Center = roundedRectInitData.RectInitData.Center;
            Rotation = roundedRectInitData.RectInitData.Rotation;
            Width = roundedRectInitData.RectInitData.Width;
            Height = roundedRectInitData.RectInitData.Height;
        }

        protected RoundedRectangle(RoundedRectangle other)
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
            if (ReferenceEquals(obj, null) || !(obj is RoundedRectangle other)) return false;
            return Center == other.Center && Rotation == other.Rotation && Width == other.Width && Height == other.Height && Rounding == other.Rounding;
        }

        public new RoundedRectangle Clone() => new RoundedRectangle(this);
        protected override Shape CloneImpl() => Clone();
        IRoundedRectangle IRoundedRectangle.Clone() => Clone();

        public override string ToString()
        {
            return $"RoundRect(C={Center},W={Width},H={Height},R={Rotation},Round={Rounding}%,V0={Vertices[0]},V1={Vertices[1]},V2={Vertices[2]},V3={Vertices[3]})";
        }
    }

    internal class RoundedRectInitData
    {
        public RoundedRectInitData(Point2D vertex0, Point2D vertex2, double rounding, ShapeRoundingMode roundingMode, double rotation)
        {
            RectInitData = new RectInitData(vertex0, vertex2, rotation);
            RoundedPolyInitData = new RoundedPolyInitData(rounding, roundingMode, RectInitData.Vertices);
        }

        public RoundedRectInitData(Point2D center, double width, double height, double rounding, ShapeRoundingMode roundingMode, double rotation)
        {
            RectInitData = new RectInitData(center, width, height, rotation);
            RoundedPolyInitData = new RoundedPolyInitData(rounding, roundingMode, RectInitData.Vertices);
        }

        public RoundedPolyInitData RoundedPolyInitData { get; }
        public RectInitData RectInitData { get; }
    }
}