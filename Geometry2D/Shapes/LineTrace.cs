namespace Altium.Geometry2D.Shapes
{
    internal class LineTrace : TraceSegment, ILineTrace
    {
        public LineTrace(ILine guide, double width)
            : base(guide, width)
        {
        }

        protected LineTrace(LineTrace other)
            : base(other)
        {
        }

        public new ILine Guide => (ILine)base.Guide;
        public override double Perimeter() => Guide.Length() * 2 + Math2D.DPI * Width;
        public override double Area() => Guide.Length() * Width + Math2D.PI * Width * Width / 4;

        public LineTrace Clone() => new LineTrace(this);
        protected override Shape CloneImpl() => Clone();
        ILineTrace ILineTrace.Clone() => Clone();
        ITraceSegment ITraceSegment.Clone() => Clone();

        protected override AARectD CalcBoundingRect()
        {
            var hw = Width / 2;
            return Guide.BoundingRect.Inflate(hw, hw);
        }

        protected override void BuildRegion(IGeometry2DEngine geometry2DEngine, IRegionBuilder regionBuilder, IGeometryPathBuilder pathBuilder, bool invert)
        {
            pathBuilder.BeginPath(invert);
            var vop = Guide.Vector.Orthogonal().Unit();
            var von = -vop;
            var hw = Width / 2;
            var pt1 = Guide.StartPoint + von * hw;
            var pt2 = Guide.EndPoint + von * hw;
            var pt23 = Guide.EndPoint + Guide.Tangent(Guide.EndPoint).Unit() * hw;
            var pt3 = Guide.EndPoint + vop * hw;
            var pt4 = Guide.StartPoint + vop * hw;
            var pt41 = Guide.StartPoint - Guide.Tangent(Guide.StartPoint).Unit() * hw;
            pathBuilder.Add(new Line(pt1, pt2));
            pathBuilder.Add(new Arc(pt2, pt3, pt23));
            pathBuilder.Add(new Line(pt3, pt4));
            pathBuilder.Add(new Arc(pt4, pt1, pt41));
            regionBuilder.Add(pathBuilder.EndPath(true));
        }

        public override string ToString()
        {
            return $"LineTrace(S={Guide.StartPoint},E={Guide.EndPoint},W={Width})";
        }
    }
}
