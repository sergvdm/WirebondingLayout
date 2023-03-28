namespace Altium.Geometry2D.Shapes
{
    internal class BeveledRectangle : BeveledPolygon, IBeveledRectangle
    {
        public BeveledRectangle(Point2D vertex0, Point2D vertex2, double beveling, ShapeBevelingMode bevelingMode, double rotation)
     : this(new BeveledRectInitData(vertex0, vertex2, beveling, bevelingMode, rotation))
        {
        }

        public BeveledRectangle(Point2D center, double width, double height, double beveling, ShapeBevelingMode bevelingMode, double rotation)
            : this(new BeveledRectInitData(center, width, height, beveling, bevelingMode, rotation))
        {
        }

        internal BeveledRectangle(BeveledRectInitData beveledRectInitData)
            : base(beveledRectInitData.BeveledPolyInitData)
        {
            Center = beveledRectInitData.RectInitData.Center;
            Rotation = beveledRectInitData.RectInitData.Rotation;
            Width = beveledRectInitData.RectInitData.Width;
            Height = beveledRectInitData.RectInitData.Height;
        }

        protected BeveledRectangle(BeveledRectangle other)
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
            if (ReferenceEquals(obj, null) || !(obj is BeveledRectangle other)) return false;
            return Center == other.Center && Rotation == other.Rotation && Width == other.Width && Height == other.Height && Beveling == other.Beveling;
        }

        public new BeveledRectangle Clone() => new BeveledRectangle(this);
        protected override Shape CloneImpl() => Clone();
        IBeveledRectangle IBeveledRectangle.Clone() => Clone();

        public override string ToString()
        {
            return $"BevRect(C={Center},W={Width},H={Height},R={Rotation},Cut={Beveling}%,V0={Vertices[0]},V1={Vertices[1]},V2={Vertices[2]},V3={Vertices[3]})";
        }
    }

    internal class BeveledRectInitData
    {
        public BeveledRectInitData(Point2D vertex0, Point2D vertex2, double beveling, ShapeBevelingMode bevelingMode, double rotation)
        {
            RectInitData = new RectInitData(vertex0, vertex2, rotation);
            BeveledPolyInitData = new BeveledPolyInitData(beveling, bevelingMode, RectInitData.Vertices);
        }

        public BeveledRectInitData(Point2D center, double width, double height, double beveling, ShapeBevelingMode bevelingMode, double rotation)
        {
            RectInitData = new RectInitData(center, width, height, rotation);
            BeveledPolyInitData = new BeveledPolyInitData(beveling, bevelingMode, RectInitData.Vertices);
        }

        public BeveledPolyInitData BeveledPolyInitData { get; }
        public RectInitData RectInitData { get; }
    }
}