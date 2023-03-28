using System;

namespace Altium.Geometry2D.Shapes
{
    internal abstract class TraceSegment : ClosedShape, ITraceSegment
    {
        public TraceSegment(IGeometryPathSegment guide, double width)
        {
            Guide = guide;
            Width = width;
        }

        protected TraceSegment(TraceSegment other)
            : base(other)
        {
            Guide = other.Guide;
            Width = other.Width;
        }

        public IGeometryPathSegment Guide { get; }
        public double Width { get; }
        ITraceSegment ITraceSegment.Clone() => (ITraceSegment)(this as ICloneable).Clone();

        public override bool Equal(IShape obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (ReferenceEquals(obj, null) || !(obj is TraceSegment other)) return false;
            return Guide.Equal(other.Guide) && Width == other.Width;
        }
    }
}
