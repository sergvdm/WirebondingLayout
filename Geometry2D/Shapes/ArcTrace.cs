using System;

namespace Altium.Geometry2D.Shapes
{
    internal class ArcTrace : TraceSegment, IArcTrace
    {
        public ArcTrace(IArc guide, double width)
            : base(guide, width)
        {
        }

        protected ArcTrace(ArcTrace other)
            : base(other)
        {
        }

        public new IArc Guide => (IArc)base.Guide;
        public override double Perimeter() => throw new NotImplementedException();
        public override double Area() => throw new NotImplementedException();

        public ArcTrace Clone() => new ArcTrace(this);
        protected override Shape CloneImpl() => Clone();
        IArcTrace IArcTrace.Clone() => Clone();
        ITraceSegment ITraceSegment.Clone() => Clone();

        protected override AARectD CalcBoundingRect()
        {
            var hw = Width / 2;
            return Guide.BoundingRect.Inflate(hw, hw);
        }

        protected override void BuildRegion(IGeometry2DEngine geometry2DEngine, IRegionBuilder regionBuilder, IGeometryPathBuilder pathBuilder, bool invert)
        {
            pathBuilder.BeginPath(invert);
            var hw = Width / 2;
            var pt1 = Guide.Center + new Vector2D(Guide.StartAngle) * (Guide.Radius + hw);
            var pt12 = Guide.Center + new Vector2D(Guide.StartAngle + Guide.SweepAngle / 2) * (Guide.Radius + hw);
            var pt2 = Guide.Center + new Vector2D(Guide.EndAngle) * (Guide.Radius + hw);
            var pt23 = Guide.EndPoint + Guide.Tangent(Guide.EndPoint).Unit() * hw;
            var pt3 = Guide.Center + new Vector2D(Guide.EndAngle) * (Guide.Radius - hw);
            var pt34 = Guide.Center + new Vector2D(Guide.EndAngle - Guide.SweepAngle / 2) * (Guide.Radius - hw);
            var pt4 = Guide.Center + new Vector2D(Guide.StartAngle) * (Guide.Radius - hw);
            var pt41 = Guide.StartPoint - Guide.Tangent(Guide.StartPoint).Unit() * hw;
            pathBuilder.Add(new Arc(pt1, pt2, pt12));
            pathBuilder.Add(new Arc(pt2, pt3, pt23));
            pathBuilder.Add(new Arc(pt3, pt4, pt34));
            pathBuilder.Add(new Arc(pt4, pt1, pt41));
            regionBuilder.Add(pathBuilder.EndPath(true));
        }

        public override string ToString()
        {
            return $"ArcTrace(C={Guide.Center},R={Guide.Radius},S={Guide.StartPoint},E={Guide.EndPoint},M={Guide.Midpoint()},W={Width})";
        }
    }
}
