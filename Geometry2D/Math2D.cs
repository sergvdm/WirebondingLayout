using Altium.Geometry2D.Shapes;
using System;

namespace Altium.Geometry2D
{
    /// <summary>
    /// General 2D calculations
    /// </summary>
    public static class Math2D
    {
        #region Constants

        public const double QPI = Math.PI / 4;
        public const double HPI = Math.PI / 2;
        public const double PI = Math.PI;
        public const double THPI = Math.PI * 3 / 2;
        public const double DPI = 2 * Math.PI;
        public const double DegToRadScale = Math2D.PI / 180;
        public const double RadToDegScale = 180 / Math2D.PI;

        #endregion Constants

        #region Miscellaneous calculations

        private static readonly double[] positiveTenPows = new double[] { 1.0, 1.0E1, 1.0E2, 1.0E3, 1.0E4, 1.0E5, 1.0E6, 1.0E7, 1.0E8, 1.0E9, 1.0E10 };
        private static readonly double[] negativeTenPows = new double[] { 1.0, 1.0E-1, 1.0E-2, 1.0E-3, 1.0E-4, 1.0E-5, 1.0E-6, 1.0E-7, 1.0E-8, 1.0E-9, 1.0E-10 };

        public static double Epsilon(int precision)
        {
            return precision >= 0 ? negativeTenPows[precision] : positiveTenPows[-precision];
        }

        public static double Round(this double value, int precision)
        {
            if (precision >= 0)
                return Math.Round(value, precision, MidpointRounding.AwayFromZero);
            else
            {
                var scale = positiveTenPows[-precision];
                return Math.Round(value / scale, MidpointRounding.AwayFromZero) * scale;
            }
        }

        public static decimal Round(this decimal value, int precision)
        {
            if (precision >= 0)
                return Math.Round(value, precision, MidpointRounding.AwayFromZero);
            else
            {
                var scale = (decimal)positiveTenPows[-precision];
                return Math.Round(value / scale, MidpointRounding.AwayFromZero) * scale;
            }
        }

        public static Point2D Round(this Point2D pt, int precision)
        {
            return new Point2D(Round(pt.X, precision), Round(pt.Y, precision));
        }

        public static Vector2D Round(this Vector2D v, int precision)
        {
            return new Vector2D(Round(v.X, precision), Round(v.Y, precision));
        }

        public static AARectD Round(this AARectD r, int precision)
        {
            return new AARectD(r.LB.Round(precision), r.RT.Round(precision));
        }

        public static Point2D Min(Point2D point1, Point2D point2)
        {
            return new Point2D(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
        }

        public static Point2D Max(Point2D point1, Point2D point2)
        {
            return new Point2D(Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
        }

        public static int LimitRange(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static double LimitRange(this double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static double LimitAngularRange(this double value, double startAngle, double sweepAngle)
        {
            value = value.NormalizeRadAngle();
            startAngle = startAngle.NormalizeRadAngle();
            var distance = value - startAngle;
            if (distance > Math2D.PI) distance -= Math2D.DPI;
            else if (distance < -Math2D.PI) distance += Math2D.DPI;
            if (sweepAngle >= 0)
            {
                if (distance < 0) value = startAngle;
                else if (distance > sweepAngle) value = startAngle + sweepAngle;
            }
            else
            {
                if (distance > 0) value = startAngle;
                if (distance < sweepAngle) value = startAngle + sweepAngle;
            }

            return value;
        }

        public static double DegToRad(this double deg)
        {
            return deg * DegToRadScale;
        }

        public static double RadToDeg(this double rad)
        {
            return rad * RadToDegScale;
        }

        public static double NormalizeDegAngle(this double deg)
        {
            if (deg < -180)
                deg += Math.Ceiling((-deg - 180) / 360) * 360;
            else if (deg > 180)
                deg -= Math.Ceiling((deg - 180) / 360) * 360;
            return deg;
        }

        public static double NormalizeRadAngle(this double rad)
        {
            if (rad < -Math2D.PI)
                rad += Math.Ceiling((-rad - Math2D.PI) / Math2D.DPI) * Math2D.DPI;
            else if (rad > Math2D.PI)
                rad -= Math.Ceiling((rad - Math2D.PI) / Math2D.DPI) * Math2D.DPI;
            return rad;
        }

        public static double MinAngularSweepTo(this double angle, double toAngle)
        {
            angle = angle.NormalizeRadAngle();
            toAngle = toAngle.NormalizeRadAngle();
            var sweep = toAngle - angle;
            if (sweep > Math2D.PI) sweep -= Math2D.DPI;
            else if (sweep < -Math2D.PI) sweep += Math2D.DPI;
            return sweep;
        }

        public static double AngularDistanceFromStartOfSweep(this double angle, double startAngle, bool sweepDirection)
        {
            angle = angle.NormalizeRadAngle();
            startAngle = startAngle.NormalizeRadAngle();
            var ad = sweepDirection ? angle - startAngle : startAngle - angle;
            if (ad < 0) ad += Math2D.DPI;
            return ad;
        }

        public static bool BelongsToAngularSpan(this double angle, double startAngle, double sweepAngle)
        {
            return angle.AngularDistanceFromStartOfSweep(startAngle, sweepAngle >= 0) <= Math.Abs(sweepAngle);
        }

        public static double CircularSegmentSweepAngle(double radius, double segmentHeight)
        {
            return 2 * Math.Acos(1 - segmentHeight / radius);
        }

        public static double DistanceTo(this Point2I pt1, Point2I pt2)
        {
            double dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double DistanceTo(this Point2D pt1, Point2D pt2)
        {
            double dx = pt2.X - pt1.X, dy = pt2.Y - pt1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Point2D ProjectTo(this Point2D pt, Point2D lp1, Point2D lp2)
        {
            var v = (lp2 - lp1).Orthogonal();
            var ir = Intersection(pt, pt + v, lp1, lp2);
            return ir.Point1;
        }

        public static Point2D ProjectTo(this Point2D pt, ILinearShape line)
        {
            return ProjectTo(pt, line.Point1, line.Point2);
        }

        public static Point2D MirrorBy(this Point2D pt, Point2D mirrorPoint)
        {
            return pt + (mirrorPoint - pt) * 2;
        }

        public static Point2D MirrorBy(this Point2D pt, Point2D lp1, Point2D lp2)
        {
            var mirrorPoint = pt.ProjectTo(lp1, lp2);
            return pt.MirrorBy(mirrorPoint);
        }

        public static Point2D MirrorBy(this Point2D pt, ILinearShape mirrorLine)
        {
            return MirrorBy(pt, mirrorLine.Point1, mirrorLine.Point2);
        }

        public static Point2D TransformForward(this Point2D pt, ITransform transform)
        {
            return transform.Forward(pt);
        }

        public static Point2D TransformReverse(this Point2D pt, ITransform transform)
        {
            return transform.Reverse(pt);
        }

        #endregion Miscellaneous calculations

        #region Basic intersections

        public static IntersectionResult Intersection(Point2D l1p1, Point2D l1p2, Point2D l2p1, Point2D l2p2)
        {
            var xmin = Math.Min(Math.Min(l1p1.X, l1p2.X), Math.Min(l2p1.X, l2p2.X));
            var xmax = Math.Max(Math.Max(l1p1.X, l1p2.X), Math.Max(l2p1.X, l2p2.X));
            var ymin = Math.Min(Math.Min(l1p1.Y, l1p2.Y), Math.Min(l2p1.Y, l2p2.Y));
            var ymax = Math.Max(Math.Max(l1p1.Y, l1p2.Y), Math.Max(l2p1.Y, l2p2.Y));
            var ov = new Vector2D((xmin + xmax) / 2, (ymin + ymax) / 2);
            var p11 = l1p1 - ov;
            var p12 = l1p2 - ov;
            var p21 = l2p1 - ov;
            var p22 = l2p2 - ov;
            var d1 = p11.X * p12.Y - p11.Y * p12.X;
            var d2 = p21.X * p22.Y - p21.Y * p22.X;
            var v1 = p12 - p11;
            var v2 = p22 - p21;
            var d = v1 ^ v2;
            var x = (d2 * v1.X - d1 * v2.X) / d;
            var y = (d2 * v1.Y - d1 * v2.Y) / d;
            if (double.IsNaN(x) || double.IsInfinity(x) || double.IsNaN(y) || double.IsInfinity(y))
            {
                return new IntersectionResult
                {
                    Point1 = p11 + (Point2D.Zero - p11).ProjectTo(v1) + ov,
                    Point2 = p21 + (Point2D.Zero - p21).ProjectTo(v2) + ov,
                    AreIntersected = false
                };
            }

            return new IntersectionResult
            {
                Point1 = new Point2D(x, y) + ov,
                Point2 = new Point2D(x, y) + ov,
                AreIntersected = true
            };
        }

        public static IntersectionResult Intersection(ILinearShape line1, ILinearShape line2)
        {
            return Intersection(line1.Point1, line1.Point2, line2.Point1, line2.Point2);
        }

        public static IntersectionResult Intersection(Point2D lp1, Point2D lp2, Point2D circleCenter, double circleRadius)
        {
            var ov = (Vector2D)(((lp1 + lp2) / 2 + circleCenter) / 2);
            var c = circleCenter - ov;
            lp1 = lp1 - ov;
            lp2 = lp2 - ov;
            var p = c.ProjectTo(lp1, lp2);
            var d = c.DistanceTo(p);
            var v = (lp2 - lp1).Orthogonal();
            if (v * (p - c) < 0) v = -v;
            if (d >= circleRadius)
            {
                return new IntersectionResult
                {
                    Point1 = p + ov,
                    Point2 = c + v.Unit() * circleRadius + ov,
                    AreIntersected = false
                };
            }
            else
            {
                var a = v.Angle;
                var halfSweep = CircularSegmentSweepAngle(circleRadius, circleRadius - d) * 0.5;
                var p1 = c + new Vector2D(a + halfSweep) * circleRadius;
                var p2 = c + new Vector2D(a - halfSweep) * circleRadius;
                return new IntersectionResult
                {
                    Point1 = p1 + ov,
                    Point2 = p2 + ov,
                    AreIntersected = true
                };
            }
        }

        public static IntersectionResult Intersection(ILinearShape line, ICircularShape circle)
        {
            return Intersection(line.Point1, line.Point2, circle.Center, circle.Radius);
        }

        public static IntersectionResult Intersection(Point2D circle1Center, double circle1Radius, Point2D circle2Center, double circle2Radius)
        {
            var ov = (Vector2D)((circle1Center + circle2Center) / 2);
            var c1 = circle1Center - ov;
            var c2 = circle2Center - ov;
            var v = c2 - c1;
            var vu = v.Unit();
            var vo = vu.Orthogonal();
            var d = v.Norm();

            var rmin = Math.Min(circle1Radius, circle2Radius);
            var rmax = Math.Max(circle1Radius, circle2Radius);
            if ((rmax - rmin) < d && d < (circle1Radius + circle2Radius))
            {
                var rmin2 = rmin * rmin;
                var rmax2 = rmax * rmax;
                var l = (rmax2 - rmin2 + d * d) / (2 * d);
                var h = Math.Sqrt(rmax2 - l * l);
                var vl = vu * l;
                var vh = vo * h;
                Point2D p1;
                Point2D p2;
                if (circle1Radius >= circle2Radius)
                {
                    p1 = c1 + (vl + vh);
                    p2 = c1 + (vl - vh);
                }
                else
                {
                    p1 = c2 + (-vl + vh);
                    p2 = c2 + (-vl - vh);
                }

                return new IntersectionResult()
                {
                    Point1 = p1 + ov,
                    Point2 = p2 + ov,
                    AreIntersected = true
                };
            }

            Point2D cp1, cp2;
            if (circle1Radius >= circle2Radius)
            {
                if (d <= circle1Radius)
                {
                    cp1 = c1 + vu * circle1Radius;
                    cp2 = c2 + vu * circle2Radius;
                }
                else
                {
                    cp1 = c1 + vu * circle1Radius;
                    cp2 = c2 - vu * circle2Radius;
                }
            }
            else
            {
                if (d <= circle2Radius)
                {
                    cp1 = c1 - vu * circle1Radius;
                    cp2 = c2 - vu * circle2Radius;
                }
                else
                {
                    cp1 = c1 + vu * circle1Radius;
                    cp2 = c2 - vu * circle2Radius;
                }
            }

            return new IntersectionResult()
            {
                Point1 = cp1 + ov,
                Point2 = cp2 + ov,
                AreIntersected = false
            };
        }

        public static IntersectionResult Intersection(ICircularShape circle1, ICircularShape circle2)
        {
            return Intersection(circle1.Center, circle1.Radius, circle2.Center, circle2.Radius);
        }

        public struct IntersectionResult
        {
            public Point2D Point1;
            public Point2D Point2;
            public bool AreIntersected;
        }

        #endregion Basic intersections
    }
}
