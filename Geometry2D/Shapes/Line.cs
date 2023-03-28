using System;

namespace Altium.Geometry2D.Shapes
{
    internal class Line : GeometryPathSegment, ILine
    {
        public Line(Point2D startPoint, Point2D endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        private Line(Line other)
            : base(other)
        {
        }

        public override bool IsSinglePoint()
        {
            return StartPoint == EndPoint;
        }

        Point2D ILinearShape.Point1 => StartPoint;
        Point2D ILinearShape.Point2 => EndPoint;

        public override Point2D Midpoint()
        {
            return new Point2D((StartPoint.X + EndPoint.X) / 2, (StartPoint.Y + EndPoint.Y) / 2);
        }

        public Vector2D Vector => (EndPoint - StartPoint).Unit();
        public override Vector2D Tangent(Point2D pt) => Vector;
        public override double Curvature(Point2D pt) => 0;
        public override double Length() => StartPoint.DistanceTo(EndPoint);
        public override double LengthFromStart(Point2D point) => StartPoint.DistanceTo(point);

        public override bool Equal(IShape obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (ReferenceEquals(obj, null) || !(obj is Line other)) return false;
            return StartPoint == other.StartPoint && EndPoint == other.EndPoint;
        }

        public Ray AsRay(bool reverse = false) => reverse ? new Ray(EndPoint, StartPoint) : new Ray(StartPoint, EndPoint);
        IRay ILine.AsRay(bool reverse) => AsRay(reverse);

        public InfiniteLine AsInfinite() => new InfiniteLine(StartPoint, EndPoint);
        IInfiniteLine ILine.AsInfinite() => AsInfinite();

        public Line Clone() => new Line(this);
        protected override Shape CloneImpl() => Clone();
        ILine ILine.Clone() => Clone();

        public Line Reverse() => new Line(EndPoint, StartPoint);
        protected override GeometryPathSegment ReverseImpl() => Reverse();
        ILine ILine.Reverse() => Reverse();

        public override IGeometryPathSegment SnapToGrid(IGeometry2DEngine geometry2DEngine)
        {
            return new Line(geometry2DEngine.SnapToGrid(StartPoint), geometry2DEngine.SnapToGrid(EndPoint));
        }

        public override GeometryPathVertex Split(Point2D pt)
        {
            //pt = pt.ProjectTo(this);
            //if ((pt - StartPoint) * Vector < 0) pt = StartPoint;
            //if ((pt - EndPoint) * Vector > 0) pt = EndPoint;
            var line1 = new Line(StartPoint, pt);
            var line2 = new Line(pt, EndPoint);
            return new GeometryPathVertex(line1, line2);
        }

        public override IGeometryPathSegment TryJoin(IGeometry2DEngine geometry2DEngine, IGeometryPathSegment otherSegment, PathSegmentJoinMode otherSegmentJoinMode)
        {
            var tryJoinStart = otherSegmentJoinMode.HasFlag(PathSegmentJoinMode.Start) && otherSegment.StartPoint == EndPoint;
            var tryJoinEnd = otherSegmentJoinMode.HasFlag(PathSegmentJoinMode.End) && otherSegment.EndPoint == StartPoint;
            if (tryJoinStart)
            {
                if (otherSegment.IsSinglePoint()) return this;
                if (this.IsSinglePoint()) return otherSegment;
                if (otherSegment is Line otherLine && (Vector * otherLine.Vector) >= 0)
                {
                    var otherLineNewStartPoint = otherSegment.StartPoint - otherLine.Vector.Unit() * this.Length();
                    var thisLineNewEndPoint = this.EndPoint + this.Vector.Unit() * otherLine.Length();
                    if (this.StartPoint.DistanceTo(otherLineNewStartPoint) < geometry2DEngine.Epsilon && otherLine.EndPoint.DistanceTo(thisLineNewEndPoint) < geometry2DEngine.Epsilon)
                        return new Line(StartPoint, otherLine.EndPoint);
                }
            }

            if (tryJoinEnd)
            {
                if (otherSegment.IsSinglePoint()) return this;
                if (this.IsSinglePoint()) return otherSegment;
                if (otherSegment is Line otherLine && (Vector * otherLine.Vector) >= 0)
                {
                    var thisLineNewStartPoint = this.StartPoint - this.Vector.Unit() * otherLine.Length();
                    var otherLineNewEndPoint = otherLine.EndPoint + otherLine.Vector.Unit() * this.Length();
                    if (otherLine.StartPoint.DistanceTo(thisLineNewStartPoint) < geometry2DEngine.Epsilon && this.EndPoint.DistanceTo(otherLineNewEndPoint) < geometry2DEngine.Epsilon)
                        return new Line(otherLine.StartPoint, EndPoint);
                }
            }

            return null;
        }

        public override int WindingNumber(IGeometry2DEngine geometry2DEngine, Point2D pt)
        {
            var minY = Math.Min(StartPoint.Y, EndPoint.Y);
            var maxY = Math.Max(StartPoint.Y, EndPoint.Y);
            if (pt.Y < minY || pt.Y >= maxY || minY == maxY) return 0;
            return Math.Sign(Vector ^ (pt - StartPoint));
        }

        protected override AARectD CalcBoundingRect()
        {
            return new AARectD(StartPoint, EndPoint);
        }

        protected override void BuildRegion(IGeometry2DEngine geometry2DEngine, IRegionBuilder regionBuilder, IGeometryPathBuilder pathBuilder, bool invert)
        {
            pathBuilder.BeginPath(invert);
            pathBuilder.Add(this);
            regionBuilder.Add(pathBuilder.EndPath(false));
        }

        public override string ToString()
        {
            return $"Line({StartPoint},{EndPoint})";
        }
    }
}