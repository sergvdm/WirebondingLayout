using System;

namespace Altium.Geometry2D.Shapes
{
    internal class Arc : GeometryPathSegment, IArc
    {
        public Arc(Point2D startPoint, Point2D endPoint, Point2D pointOnArc)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;

            if (startPoint == endPoint)
            {
                Center = new Point2D((startPoint.X + pointOnArc.X) / 2, (startPoint.Y + pointOnArc.Y) / 2);
                Radius = Center.DistanceTo(startPoint);
            }
            else
            {
                // http://www.ambrsoft.com/TrigoCalc/Circle3D.htm
                //double x1 = startPoint.X, y1 = startPoint.Y;
                //double x2 = pointOnArc.X, y2 = pointOnArc.Y;
                //double x3 = endPoint.X - pointOnArc.X, y3 = endPoint.Y - pointOnArc.Y;
                //double x12py12 = x1 * x1 + y1 * y1;
                //double x22py22 = x2 * x2 + y2 * y2;
                //double x32py32 = x3 * x3 + y3 * y3;
                //double xn = x12py12 * (y2 - y3) + x22py22 * (y3 - y1) + x32py32 * (y1 - y2);
                //double yn = x12py12 * (x3 - x2) + x22py22 * (x1 - x3) + x32py32 * (x2 - x1);
                //double d = 2 * (x1 * (y2 - y3) - y1 * (x2 - x3) + x2 * y3 - x3 * y2);
                //double cx = xn / d;
                //double cy = yn / d;

                // translated to pointOnArc:
                double x1 = startPoint.X - pointOnArc.X, y1 = startPoint.Y - pointOnArc.Y;
                double x3 = endPoint.X - pointOnArc.X, y3 = endPoint.Y - pointOnArc.Y;
                double x12py12 = x1 * x1 + y1 * y1;
                double x32py32 = x3 * x3 + y3 * y3;
                double xn = x12py12 * (-y3) + x32py32 * y1;
                double yn = x12py12 * x3 + x32py32 * (-x1);
                double d = 2 * (x1 * (-y3) - y1 * (-x3));
                double cx = xn / d;
                double cy = yn / d;
                if (double.IsNaN(cx) || double.IsInfinity(cx) || double.IsNaN(cy) || double.IsInfinity(cy))
                {
                    cx = (x1 + x3) / 2;
                    cy = (y1 + y3) / 2;
                }

                Center = new Point2D(cx + pointOnArc.X, cy + pointOnArc.Y);
                Radius = (Center.DistanceTo(startPoint) + Center.DistanceTo(endPoint)) / 2;
            }

            StartAngle = (StartPoint - Center).Angle;
            EndAngle = (EndPoint - Center).Angle;
            SweepAngle = EndAngle - StartAngle;
            if (!new Vector2D(Center, pointOnArc).Angle.BelongsToAngularSpan(StartAngle, SweepAngle))
                SweepAngle += Math2D.DPI * (-Math.Sign(SweepAngle));
            if ((StartPoint == EndPoint || SweepAngle == 0) && Radius > 0) SweepAngle = Math2D.DPI;
        }

        public Arc(Point2D startPoint, Point2D endPoint, double radius, bool cw = false, bool longest = false)
        {
            if (radius < 0)
                throw new ArgumentException("Invalid radius", nameof(radius));
            if (startPoint == endPoint && radius != 0)
                throw new ArgumentException("Invalid end point", nameof(endPoint));

            StartPoint = startPoint;
            EndPoint = endPoint;
            Radius = radius;

            if (startPoint == endPoint)
            {
                Center = startPoint;
            }
            else
            {
                var z = endPoint - startPoint;
                var n = z.Orthogonal().Unit();
                var d = z.Norm();
                var h2 = radius * radius - d * d / 4;
                if (h2 < 0) h2 = 0;
                var h = Math.Sqrt(h2);
                var m = new Point2D((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);
                Center = cw == longest ? m + h * n : m - h * n;
            }

            StartAngle = (StartPoint - Center).Angle;
            EndAngle = (EndPoint - Center).Angle;
            SweepAngle = EndAngle - StartAngle;
            if (startPoint == endPoint && Radius > 0)
                SweepAngle = cw ? -Math2D.DPI : Math2D.DPI;
            else if (cw && SweepAngle > 0)
                SweepAngle -= Math2D.DPI;
            else if (!cw && SweepAngle < 0)
                SweepAngle += Math2D.DPI;
        }

        public Arc(Point2D center, double radius, double startAngle, double sweepAngle)
        {
            if (radius < 0)
                throw new ArgumentException("Invalid radius", nameof(radius));

            Center = center;
            Radius = radius;
            StartAngle = startAngle.NormalizeRadAngle();
            SweepAngle = sweepAngle.LimitRange(-Math2D.DPI, Math2D.DPI);
            EndAngle = (StartAngle + SweepAngle).NormalizeRadAngle();
            StartPoint = Center + new Vector2D(StartAngle) * Radius;
            EndPoint = Center + new Vector2D(EndAngle) * Radius;
        }

        internal Arc(Point2D startPoint, Point2D endPoint, double radius, Point2D center, bool cw = false)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Center = center;
            Radius = radius;
            StartAngle = (StartPoint - Center).Angle;
            EndAngle = (EndPoint - Center).Angle;
            SweepAngle = EndAngle - StartAngle;
            if (startPoint == endPoint && radius > 0)
                SweepAngle = cw ? -Math2D.DPI : Math2D.DPI;
            else if (cw && SweepAngle > 0)
                SweepAngle -= Math2D.DPI;
            else if (!cw && SweepAngle < 0)
                SweepAngle += Math2D.DPI;
        }

        protected Arc(Arc other)
            : base(other)
        {
            Center = other.Center;
            Radius = other.Radius;
            StartAngle = other.StartAngle;
            SweepAngle = other.SweepAngle;
            EndAngle = other.EndAngle;
        }

        public Point2D Center { get; private set; }
        public double Radius { get; private set; }
        public double StartAngle { get; private set; }
        public double SweepAngle { get; private set; }
        public double EndAngle { get; private set; }

        public override bool IsSinglePoint()
        {
            return StartPoint == EndPoint && Center == StartPoint;
        }

        public override Point2D Midpoint()
        {
            return Center + Radius * new Vector2D(StartAngle + SweepAngle / 2);
        }

        public override double Length() => Math.Abs(SweepAngle) * Radius;

        public override bool Equal(IShape obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (ReferenceEquals(obj, null) || !(obj is Arc other)) return false;
            return StartPoint == other.StartPoint && EndPoint == other.EndPoint && Center == other.Center && Radius == other.Radius;
        }

        public Arc Clone() => new Arc(this);
        protected override Shape CloneImpl() => Clone();
        IArc IArc.Clone() => Clone();

        public Arc Reverse()
        {
            var result = new Arc(this);
            result.StartPoint = EndPoint;
            result.EndPoint = StartPoint;
            result.Center = Center;
            result.Radius = Radius;
            result.StartAngle = EndAngle;
            result.SweepAngle = -SweepAngle;
            result.EndAngle = StartAngle;
            return result;
        }

        protected override GeometryPathSegment ReverseImpl() => Reverse();
        IArc IArc.Reverse() => Reverse();

        public override double LengthFromStart(Point2D pt)
        {
            var pointSweepAngle = StartAngle.MinAngularSweepTo(new Vector2D(Center, pt).Angle);
            if (Math.Sign(pointSweepAngle) != Math.Sign(SweepAngle)) pointSweepAngle += Math2D.DPI * Math.Sign(SweepAngle);
            return Math.Abs(pointSweepAngle) * Radius;
        }

        public override Vector2D Tangent(Point2D pt)
        {
            return SweepAngle >= 0 ? new Vector2D(Center, pt).Orthogonal() : new Vector2D(pt, Center).Orthogonal();
        }

        public override double Curvature(Point2D pt)
        {
            var c = 1 / Radius;
            return SweepAngle >= 0 ? c : -c;
        }

        public override IGeometryPathSegment SnapToGrid(IGeometry2DEngine geometry2DEngine)
        {
            var newStartPoint = geometry2DEngine.SnapToGrid(StartPoint);
            var newEndPoint = geometry2DEngine.SnapToGrid(EndPoint);
            if (StartPoint == newStartPoint && EndPoint == newEndPoint)
                return Clone();

            if (Radius < geometry2DEngine.Epsilon)
                return new Line(newStartPoint, newEndPoint);

            var snappedMidpoint = geometry2DEngine.SnapToGrid(Midpoint());
            if (snappedMidpoint == newStartPoint || snappedMidpoint == newEndPoint || geometry2DEngine.AreCoincident(snappedMidpoint, new Line(newStartPoint, newEndPoint)))
                return new Line(newStartPoint, newEndPoint);

            Point2D newCenter;
            double newRadius;
            if (newStartPoint == newEndPoint)
            {
                newCenter = Center;
                newRadius = newCenter.DistanceTo(newStartPoint);
            }
            else
            {
                var z = newEndPoint - newStartPoint;
                var n = z.Orthogonal().Unit();
                var d = z.Norm();
                var h2 = Radius * Radius - d * d / 4;
                if (h2 < 0) h2 = 0;
                if (h2 == 0 && newStartPoint == newEndPoint)
                    return new Line(newStartPoint, newEndPoint);

                var h = Math.Sqrt(h2);
                var m = (newStartPoint + newEndPoint) / 2;
                var cw = SweepAngle < 0;
                var longest = Math.Abs(SweepAngle) > Math2D.PI;
                newCenter = cw == longest ? m + h * n : m - h * n;
                newRadius = (newCenter.DistanceTo(newStartPoint) + newCenter.DistanceTo(newEndPoint)) / 2;
            }

            if (double.IsInfinity(newRadius) || double.IsNaN(newCenter.X) || double.IsNaN(newCenter.Y))
                return new Line(newStartPoint, newEndPoint);

            var newStartAngle = (newStartPoint - newCenter).Angle;
            var newEndAngle = (newEndPoint - newCenter).Angle;
            var newSweepAngle = newEndAngle - newStartAngle;
            if (newStartPoint == newEndPoint && Radius > 0)
                newSweepAngle = SweepAngle < 0 ? -Math2D.DPI : Math2D.DPI;
            else if (SweepAngle < 0 && newSweepAngle > 0)
                newSweepAngle -= Math2D.DPI;
            else if (SweepAngle >= 0 && newSweepAngle < 0)
                newSweepAngle += Math2D.DPI;
            var newArc = new Arc(newStartPoint, newEndPoint, newRadius, newCenter, SweepAngle < 0);
            return newArc;
        }

        public override GeometryPathVertex Split(Point2D pt)
        {
            var splitAngle = new Vector2D(Center, pt).Angle;
            var arc1SweepAngle = splitAngle - StartAngle;
            if (Math.Sign(arc1SweepAngle) != Math.Sign(SweepAngle))
                arc1SweepAngle += Math2D.DPI * (-Math.Sign(arc1SweepAngle));
            var arc2SweepAngle = SweepAngle - arc1SweepAngle;
            if (Math.Sign(arc2SweepAngle) != Math.Sign(SweepAngle))
                arc2SweepAngle += Math2D.DPI * (-Math.Sign(arc2SweepAngle));
            var pointOnArc1 = Center + new Vector2D(StartAngle + arc1SweepAngle / 2) * Radius;
            var pointOnArc2 = Center + new Vector2D(splitAngle + arc2SweepAngle / 2) * Radius;
            var arc1 = new Arc(StartPoint, pt, Radius, Center, SweepAngle < 0);
            var arc2 = new Arc(pt, EndPoint, Radius, Center, SweepAngle < 0);
            return new GeometryPathVertex(arc1, arc2);
        }

        public override IGeometryPathSegment TryJoin(IGeometry2DEngine geometry2DEngine, IGeometryPathSegment otherSegment, PathSegmentJoinMode otherSegmentJoinMode)
        {
            var tryJoinStart = otherSegmentJoinMode.HasFlag(PathSegmentJoinMode.Start) && otherSegment.StartPoint == EndPoint;
            var tryJoinEnd = otherSegmentJoinMode.HasFlag(PathSegmentJoinMode.End) && otherSegment.EndPoint == StartPoint;
            if (tryJoinStart)
            {
                if (otherSegment.IsSinglePoint()) return this;
                if (this.IsSinglePoint()) return otherSegment;
                if (otherSegment is Arc otherArc && (SweepAngle * otherArc.SweepAngle) >= 0)
                {
                    if (this.Center == otherArc.Center && this.Radius == otherArc.Radius)
                        return new Arc(StartPoint, otherArc.EndPoint, Radius, Center, SweepAngle < 0);
                }
            }

            if (tryJoinEnd)
            {
                if (otherSegment.IsSinglePoint()) return this;
                if (this.IsSinglePoint()) return otherSegment;
                if (otherSegment is Arc otherArc && (SweepAngle * otherArc.SweepAngle) >= 0)
                {
                    if (this.Center == otherArc.Center && this.Radius == otherArc.Radius)
                        return new Arc(otherArc.StartPoint, EndPoint, Radius, Center, SweepAngle < 0);
                }
            }

            return null;
        }

        public override int WindingNumber(IGeometry2DEngine geometry2DEngine, Point2D pt)
        {
            Func<Point2D, bool> areCoincident = (point) =>
            {
                if (point.DistanceTo(StartPoint) < geometry2DEngine.Epsilon || point.DistanceTo(EndPoint) < geometry2DEngine.Epsilon) return true;
                var v = new Vector2D(Center, point);
                var a = v.Angle;
                if (!a.BelongsToAngularSpan(StartAngle, SweepAngle)) return false;
                var p = Center + v.Unit() * Radius;
                return point.DistanceTo(p) < geometry2DEngine.Epsilon;
            };

            var br = BoundingRect;
            var minY = br.LB.Y;
            var maxY = br.RT.Y;
            if (pt.Y < minY || pt.Y >= maxY || minY == maxY) return 0;
            var ir = Math2D.Intersection(new Point2D(Center.X - 1, pt.Y), new Point2D(Center.X + 1, pt.Y), Center, Radius);
            if (ir.AreIntersected)
            {
                int ni = 0;
                bool b1 = areCoincident(ir.Point1);
                bool b2 = areCoincident(ir.Point2);
                if (b1 && b2 && StartPoint != EndPoint)
                {
                    if (ir.Point1.DistanceTo(ir.Point2) < geometry2DEngine.Epsilon)
                    {
                        if (ir.Point1.DistanceTo(Midpoint()) <= ir.Point2.DistanceTo(Midpoint()))
                            b2 = false;
                        else
                            b1 = false;
                    }

                    if ((ir.Point1.DistanceTo(StartPoint) < geometry2DEngine.Epsilon && Tangent(StartPoint).Y < 0) ||
                        (ir.Point1.DistanceTo(EndPoint) < geometry2DEngine.Epsilon && Tangent(EndPoint).Y >= 0)) b1 = false;
                    else if ((ir.Point2.DistanceTo(StartPoint) < geometry2DEngine.Epsilon && Tangent(StartPoint).Y < 0) ||
                        (ir.Point2.DistanceTo(EndPoint) < geometry2DEngine.Epsilon && Tangent(EndPoint).Y >= 0)) b2 = false;
                }

                if (b1) ni++;
                if (b2) ni++;
                if (ni == 2)
                {
                    var d = Center.DistanceTo(pt);
                    if (d < Radius) return 2 * Math.Sign(SweepAngle);
                    else return 0;
                }
                else if (ni == 1)
                {
                    var p = b1 ? ir.Point1 : ir.Point2;
                    return Math.Sign(p.X - pt.X) * Math.Sign(p.X - Center.X) * Math.Sign(SweepAngle);
                }
            }
            else if (ir.Point2.Y == pt.Y && ir.Point2.Y < Midpoint().Y)
            {
                if (ir.Point2 == StartPoint) return -Math.Sign(pt.X - ir.Point2.X);
                if (ir.Point2 == EndPoint) return Math.Sign(pt.X - ir.Point2.X);
            }

            return 0;
        }

        protected override AARectD CalcBoundingRect()
        {
            double left, bottom, right, top;
            if (Math2D.PI.BelongsToAngularSpan(StartAngle, SweepAngle)) left = Center.X - Radius;
            else left = Math.Min(StartPoint.X, EndPoint.X);
            if (Math2D.THPI.BelongsToAngularSpan(StartAngle, SweepAngle)) bottom = Center.Y - Radius;
            else bottom = Math.Min(StartPoint.Y, EndPoint.Y);
            if (0.0.BelongsToAngularSpan(StartAngle, SweepAngle)) right = Center.X + Radius;
            else right = Math.Max(StartPoint.X, EndPoint.X);
            if (Math2D.HPI.BelongsToAngularSpan(StartAngle, SweepAngle)) top = Center.Y + Radius;
            else top = Math.Max(StartPoint.Y, EndPoint.Y);
            return new AARectD(new Point2D(left, bottom), new Point2D(right, top));
        }

        protected override void BuildRegion(IGeometry2DEngine geometry2DEngine, IRegionBuilder regionBuilder, IGeometryPathBuilder pathBuilder, bool invert)
        {
            pathBuilder.BeginPath(invert);
            pathBuilder.Add(this);
            regionBuilder.Add(pathBuilder.EndPath(false));
        }

        public override string ToString()
        {
            return $"Arc(C={Center},R={Radius},S={StartPoint},E={EndPoint},M={Midpoint()})";
        }
    }
}