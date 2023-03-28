using Altium.Geometry2D.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altium.Geometry2D
{
    public class Geometry2DEngine : IGeometry2DEngine
    {
        public Geometry2DEngine(Geometry2DEngineSettings settings = null)
        {
            Settings = settings ?? new Geometry2DEngineSettings();
            Precision = Settings.Precision;
            Epsilon = Math2D.Epsilon(Precision);
        }

        private Geometry2DEngineSettings Settings { get; }

        public int Precision { get; }
        public double Epsilon { get; }

        #region Primitive shapes factory

        public Point2D SnapToGrid(Point2D point) => point.Round(Settings.Precision);
        public IInfiniteLine CreateInfiniteLine(Point2D point, Vector2D vector) => new InfiniteLine(point, vector);
        public IInfiniteLine CreateInfiniteLine(ILinearShape refShape) => CreateInfiniteLine(refShape.Point1, refShape.Vector);
        public IRay CreateRay(Point2D point, Vector2D vector) => new Ray(point, vector);
        public IRay CreateRay(ILinearShape refShape) => CreateRay(refShape.Point1, refShape.Vector);
        public ILine CreateLine(Point2D startPoint, Point2D endPoint) => new Line(startPoint, endPoint);
        public ILine CreateLine(ILinearShape refShape) => CreateLine(refShape.Point1, refShape.Point1 + refShape.Vector);
        public IArc CreateArc(Point2D startPoint, Point2D endPoint, Point2D pointOnArc) => new Arc(startPoint, endPoint, pointOnArc);
        public IArc CreateArc(Point2D startPoint, Point2D endPoint, double radius, bool cw = false, bool longest = false) => new Arc(startPoint, endPoint, radius, cw, longest);
        public IArc CreateArc(Point2D center, double radius, double startAngle, double sweepAngle) => new Arc(center, radius, startAngle, sweepAngle);
        public IArc CreateArc(ICircularShape refShape, double startAngle, double sweepAngle) => CreateArc(refShape.Center, refShape.Radius, startAngle, sweepAngle);
        public ICircle CreateCircle(Point2D center, double radius) => new Circle(center, radius);
        public ICircle CreateCircle(ICircularShape refShape) => CreateCircle(refShape.Center, refShape.Radius);
        public IRectangle CreateRectangle(Point2D center, double width, double height, double rotation = 0) => new Rectangle(center, width, height, rotation);
        public IRectangle CreateRectangle(Point2D vertex0, Point2D vertex2, double rotation = 0) => new Rectangle(vertex0, vertex2, rotation);
        public IRectangle CreateRectangle(IRectShape refShape) => CreateRectangle(refShape.Center, refShape.Width, refShape.Height, refShape.Rotation);
        public IRoundedRectangle CreateRoundedRectangle(Point2D center, double width, double height, double rounding, ShapeRoundingMode roundingMode, double rotation = 0) => new RoundedRectangle(center, width, height, rounding, roundingMode, rotation);
        public IRoundedRectangle CreateRoundedRectangle(Point2D vertex0, Point2D vertex2, double rounding, ShapeRoundingMode roundingMode, double rotation = 0) => new RoundedRectangle(vertex0, vertex2, rounding, roundingMode, rotation);
        public IRoundedRectangle CreateRoundedRectangle(IRectShape refShape, double rounding, ShapeRoundingMode roundingMode) => CreateRoundedRectangle(refShape.Center, refShape.Width, refShape.Height, rounding, roundingMode, refShape.Rotation);
        public IBeveledRectangle CreateBeveledRectangle(Point2D center, double width, double height, double beveling, ShapeBevelingMode bevelingMode, double rotation = 0) => new BeveledRectangle(center, width, height, beveling, bevelingMode, rotation);
        public IBeveledRectangle CreateBeveledRectangle(Point2D vertex0, Point2D vertex2, double beveling, ShapeBevelingMode bevelingMode, double rotation = 0) => new BeveledRectangle(vertex0, vertex2, beveling, bevelingMode, rotation);
        public IBeveledRectangle CreateBeveledRectangle(IRectShape refShape, double beveling, ShapeBevelingMode bevelingMode) => CreateBeveledRectangle(refShape.Center, refShape.Width, refShape.Height, beveling, bevelingMode, refShape.Rotation);
        public IPolygon CreatePolygon(params Point2D[] vertices) => new Polygon(vertices);
        public IPolygon CreatePolygon(IEnumerable<Point2D> vertices) => new Polygon(vertices);
        public IRoundedPolygon CreateRoundedPolygon(double rounding, ShapeRoundingMode roundingMode, params Point2D[] vertices) => new RoundedPolygon(rounding, roundingMode, vertices);
        public IRoundedPolygon CreateRoundedPolygon(double rounding, ShapeRoundingMode roundingMode, IEnumerable<Point2D> vertices) => new RoundedPolygon(rounding, roundingMode, vertices.ToArray());
        public IBeveledPolygon CreateBeveledPolygon(double beveling, ShapeBevelingMode bevelingMode, params Point2D[] vertices) => new BeveledPolygon(beveling, bevelingMode, vertices);
        public IBeveledPolygon CreateBeveledPolygon(double beveling, ShapeBevelingMode bevelingMode, IEnumerable<Point2D> vertices) => new BeveledPolygon(beveling, bevelingMode, vertices.ToArray());
        public ILineTrace CreateLineTrace(ILine guide, double width) => new LineTrace(guide, width);
        public IArcTrace CreateArcTrace(IArc guide, double width) => new ArcTrace(guide, width);

        #endregion Primitive shapes factory

        #region Coincident check

        public bool AreCoincident(Point2D pt1, Point2D pt2)
        {
            return Math2D.Round(pt1, Precision) == Math2D.Round(pt2, Precision);
        }

        private bool AreInEpsilon(Point2D pt1, Point2D pt2) => pt1.DistanceTo(pt2) < Epsilon;

        public bool AreCoincident(Point2D pt, IInfiniteLine infLine)
        {
            var p = pt.ProjectTo(infLine);
            return AreInEpsilon(pt, p);
        }

        public bool AreCoincident(Point2D pt, IRay ray)
        {
            if (AreCoincident(pt, ray.Point1)) return true;
            var p = pt.ProjectTo(ray);
            if (!AreInEpsilon(pt, p)) return false;
            return ((p - ray.Point1) * ray.Vector >= 0);
        }

        public bool AreCoincident(Point2D pt, ILine line) => AreCoincident(line, pt);
        public bool AreCoincident(Point2D pt, ICircle circle) => AreCoincident(circle, pt);
        public bool AreCoincident(Point2D pt, IArc arc) => AreCoincident(arc, pt);
        public bool AreCoincident(Point2D pt, IGeometryPathSegment segment) => AreCoincident(segment, pt);
        public bool AreCoincident(Point2D pt, IGeometryPath path) => AreCoincident(path, pt);
        public bool AreCoincident(Point2D pt, IRegion region) => AreCoincident(region, pt);

        public bool AreCoincident(IInfiniteLine infLine, Point2D pt) => AreCoincident(pt, infLine);
        public bool AreCoincident(IInfiniteLine infLine1, IInfiniteLine infLine2)
        {
            return AreCoincident(infLine1.Point1, infLine2) && (infLine1.Vector ^ infLine2.Vector) == 0;
        }

        public bool AreCoincident(IInfiniteLine infLine, IRay ray)
        {
            return AreCoincident(ray.Point1, infLine) && (infLine.Vector ^ ray.Vector) == 0;
        }

        public bool AreCoincident(IInfiniteLine infLine, ILine line)
        {
            return AreCoincident(line.StartPoint, infLine) && AreCoincident(line.EndPoint, infLine);
        }

        public bool AreCoincident(IInfiniteLine infLine, ICircle circle)
        {
            return AreCoincident(circle.Center, infLine) && circle.Radius == 0;
        }

        public bool AreCoincident(IInfiniteLine infLine, IArc arc)
        {
            return AreCoincident(arc.StartPoint, infLine) && AreCoincident(arc.EndPoint, infLine) && AreCoincident(arc.Midpoint(), infLine);
        }

        public bool AreCoincident(IInfiniteLine infLine, IGeometryPathSegment segment)
        {
            if (segment is ILine line)
            {
                return AreCoincident(infLine, line);
            }
            else if (segment is IArc arc)
            {
                return AreCoincident(infLine, arc);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public bool AreCoincident(IInfiniteLine infLine, IGeometryPath path)
        {
            return path.Segments.All(x => AreCoincident(infLine, x));
        }

        public bool AreCoincident(IInfiniteLine infLine, IRegion region)
        {
            return region.Paths.All(x => AreCoincident(infLine, x));
        }

        public bool AreCoincident(IRay ray, Point2D pt) => AreCoincident(pt, ray);
        public bool AreCoincident(IRay ray, IInfiniteLine infLine) => AreCoincident(infLine, ray);
        public bool AreCoincident(IRay ray1, IRay ray2)
        {
            return AreCoincident(ray1.Point1, ray2) && AreCoincident(ray2.Point1, ray1) && (ray1.Vector * ray2.Vector) >= 0 && (ray1.Vector ^ ray2.Vector) == 0;
        }

        public bool AreCoincident(IRay ray, ILine line)
        {
            return AreCoincident(line.StartPoint, ray) && AreCoincident(line.EndPoint, ray);
        }

        public bool AreCoincident(IRay ray, ICircle circle)
        {
            return AreCoincident(circle.Center, ray) && circle.Radius == 0;
        }

        public bool AreCoincident(IRay ray, IArc arc)
        {
            return AreCoincident(arc.StartPoint, ray) && AreCoincident(arc.EndPoint, ray) && AreCoincident(arc.Midpoint(), ray);
        }

        public bool AreCoincident(IRay ray, IGeometryPathSegment segment)
        {
            if (segment is ILine line)
            {
                return AreCoincident(ray, line);
            }
            else if (segment is IArc arc)
            {
                return AreCoincident(ray, arc);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public bool AreCoincident(IRay ray, IGeometryPath path)
        {
            return path.Segments.All(x => AreCoincident(ray, x));
        }

        public bool AreCoincident(IRay ray, IRegion region)
        {
            return region.Paths.All(x => AreCoincident(ray, x));
        }

        public bool AreCoincident(ILine line, Point2D pt)
        {
            if (AreCoincident(pt, line.StartPoint) || AreCoincident(pt, line.EndPoint)) return true;
            var p = pt.ProjectTo(line);
            if (!AreInEpsilon(pt, p)) return false;
            return ((p - line.StartPoint) * line.Vector >= 0 && (p - line.EndPoint) * line.Vector <= 0);
        }

        public bool AreCoincident(ILine line, IInfiniteLine infLine) => AreCoincident(infLine, line);
        public bool AreCoincident(ILine line, IRay ray) => AreCoincident(ray, line);

        public bool AreCoincident(ILine line1, ILine line2)
        {
            return (AreCoincident(line1, line2.StartPoint) && AreCoincident(line1, line2.EndPoint)) ||
                (AreCoincident(line2, line1.StartPoint) && AreCoincident(line2, line1.EndPoint));
        }

        public bool AreCoincident(ILine line, ICircle circle)
        {
            return line.IsSinglePoint() && circle.Radius == 0 && AreCoincident(line.StartPoint, circle.Center);
        }

        public bool AreCoincident(ILine line, IArc arc)
        {
            return AreCoincident(line.StartPoint, arc.StartPoint) && AreCoincident(line.EndPoint, arc.EndPoint) && AreCoincident(line.Midpoint(), arc.Midpoint());
        }

        public bool AreCoincident(ICircle circle, Point2D pt)
        {
            var v = new Vector2D(circle.Center, pt);
            var p = circle.Center + v.Unit() * circle.Radius;
            return AreInEpsilon(pt, p);
        }

        public bool AreCoincident(ICircle circle, IInfiniteLine infLine) => AreCoincident(infLine, circle);
        public bool AreCoincident(ICircle circle, IRay ray) => AreCoincident(ray, circle);

        public bool AreCoincident(ICircle circle, ILine line) => AreCoincident(line, circle);
        public bool AreCoincident(ICircle circle1, ICircle circle2)
        {
            return AreCoincident(new Point2D(circle1.Center.X - circle1.Radius, circle1.Center.Y), new Point2D(circle2.Center.X - circle2.Radius, circle2.Center.Y))
                && AreCoincident(new Point2D(circle1.Center.X + circle1.Radius, circle1.Center.Y), new Point2D(circle2.Center.X + circle2.Radius, circle2.Center.Y))
                && AreCoincident(new Point2D(circle1.Center.X, circle1.Center.Y - circle1.Radius), new Point2D(circle2.Center.X, circle2.Center.Y - circle2.Radius))
                && AreCoincident(new Point2D(circle1.Center.X, circle1.Center.Y + circle1.Radius), new Point2D(circle2.Center.X, circle2.Center.Y + circle2.Radius));
        }

        public bool AreCoincident(ICircle circle, IArc arc) => AreCoincident(arc, circle);

        public bool AreCoincident(ICircle circle, IGeometryPathSegment segment)
        {
            if (segment is ILine line)
            {
                return AreCoincident(circle, line);
            }
            else if (segment is IArc arc)
            {
                return AreCoincident(circle, arc);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public bool AreCoincident(ICircle circle, IGeometryPath path)
        {
            return path.Segments.All(x => AreCoincident(circle, x));
        }

        public bool AreCoincident(ICircle circle, IRegion region)
        {
            return region.Paths.All(x => AreCoincident(circle, x));
        }

        public bool AreCoincident(IArc arc, Point2D pt)
        {
            if (AreCoincident(pt, arc.StartPoint) || AreCoincident(pt, arc.EndPoint) || AreCoincident(pt, arc.Midpoint())) return true;
            var v = new Vector2D(arc.Center, pt);
            var a = v.Angle;
            if (!a.BelongsToAngularSpan(arc.StartAngle, arc.SweepAngle)) return false;
            var p = arc.Center + v.Unit() * arc.Radius;
            return AreInEpsilon(pt, p);
        }

        public bool AreCoincident(IArc arc, IInfiniteLine infLine) => AreCoincident(infLine, arc);
        public bool AreCoincident(IArc arc, IRay ray) => AreCoincident(ray, arc);
        public bool AreCoincident(IArc arc, ILine line) => AreCoincident(line, arc);
        public bool AreCoincident(IArc arc, ICircle circle)
        {
            return AreCoincident(circle, arc.StartPoint) && AreCoincident(circle, arc.EndPoint) && AreCoincident(circle, arc.Midpoint());
        }

        public bool AreCoincident(IArc arc1, IArc arc2)
        {
            return (AreCoincident(arc1, arc2.StartPoint) && AreCoincident(arc1, arc2.EndPoint) && AreCoincident(arc1, arc2.Midpoint())) ||
                (AreCoincident(arc2, arc1.StartPoint) && AreCoincident(arc2, arc1.EndPoint) && AreCoincident(arc2, arc1.Midpoint()));
        }

        public bool AreCoincident(IGeometryPathSegment segment, Point2D point)
        {
            if (segment is ILine line)
            {
                return AreCoincident(line, point);
            }
            else if (segment is IArc arc)
            {
                return AreCoincident(arc, point);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public bool AreCoincident(IGeometryPathSegment segment, IInfiniteLine infLine)
        {
            if (segment is ILine line)
            {
                return AreCoincident(line, infLine);
            }
            else if (segment is IArc arc)
            {
                return AreCoincident(arc, infLine);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public bool AreCoincident(IGeometryPathSegment segment, IRay ray)
        {
            if (segment is ILine line)
            {
                return AreCoincident(line, ray);
            }
            else if (segment is IArc arc)
            {
                return AreCoincident(arc, ray);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public bool AreCoincident(IGeometryPathSegment segment1, IGeometryPathSegment segment2)
        {
            if (segment1 is ILine line1)
            {
                if (segment2 is ILine line2)
                {
                    return AreCoincident(line1, line2);
                }
                else if (segment2 is IArc arc2)
                {
                    return AreCoincident(line1, arc2);
                }
                else
                    throw new ArgumentException($"Unsupported path segment type: {segment2.GetType()}", nameof(segment2));
            }
            else if (segment1 is IArc arc1)
            {
                if (segment2 is ILine line2)
                {
                    return AreCoincident(arc1, line2);
                }
                else if (segment2 is IArc arc2)
                {
                    return AreCoincident(arc1, arc2);
                }
                else
                    throw new ArgumentException($"Unsupported path segment type: {segment2.GetType()}", nameof(segment2));
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment1.GetType()}", nameof(segment1));
        }

        public bool AreCoincident(IGeometryPathSegment segment, IGeometryPath path) => AreCoincident(path, segment);
        public bool AreCoincident(IGeometryPathSegment segment, IRegion region) => AreCoincident(region, segment);

        public bool AreCoincident(IGeometryPath path, Point2D point)
        {
            foreach (var segment in path.Segments)
            {
                if (AreCoincident(segment, point)) return true;
            }

            return false;
        }

        public bool AreCoincident(IGeometryPath path, IInfiniteLine infLine)
        {
            return path.Segments.All(x => AreCoincident(x, infLine));
        }

        public bool AreCoincident(IGeometryPath path, IRay ray)
        {
            return path.Segments.All(x => AreCoincident(x, ray));
        }

        public bool AreCoincident(IGeometryPath path, IGeometryPathSegment segment)
        {
            path = Reduce(path);
            return path.Segments.All(x => AreCoincident(x, segment));
        }

        public bool AreCoincident(IGeometryPath path1, IGeometryPath path2)
        {
            if (path1.IsClosed != path2.IsClosed) return false;
            path1 = Reduce(path1);
            path2 = Reduce(path2);
            return AreCoincident(path1, path2, false) || AreCoincident(path1, path2, true);
        }

        private bool AreCoincident(IGeometryPath path1, IGeometryPath path2, bool reversePath2)
        {
            var result = true;
            var enum1 = new PathSegmentIterator(path1.Segments, null).Segments.GetEnumerator();
            var enum2 = new PathSegmentIterator(path2.Segments, null, reversePath2).Segments.GetEnumerator();
            if (path1.IsClosed)
            {
                bool firstCommonSegmentFound = false;
                if (enum1.MoveNext())
                {
                    while (!firstCommonSegmentFound && enum2.MoveNext())
                        firstCommonSegmentFound = AreCoincident(enum1.Current, enum2.Current);
                }

                result = firstCommonSegmentFound;
                if (result)
                {
                    enum2 = new PathSegmentIterator(path2.Segments, enum2.Current, reversePath2).Segments.GetEnumerator();
                    enum2.MoveNext();
                    while (result && enum1.MoveNext() && enum2.MoveNext())
                        result = AreCoincident(enum1.Current, enum2.Current);
                }
            }
            else
            {
                while (result && enum1.MoveNext() && enum2.MoveNext())
                    result = AreCoincident(enum1.Current, enum2.Current);
            }

            return result;
        }

        public bool AreCoincident(IGeometryPath path, IRegion region) => AreCoincident(region, path);

        public bool AreCoincident(IRegion region, IInfiniteLine infLine)
        {
            return region.Paths.All(x => AreCoincident(x, infLine));
        }

        public bool AreCoincident(IRegion region, IRay ray)
        {
            return region.Paths.All(x => AreCoincident(x, ray));
        }

        public bool AreCoincident(IRegion region, Point2D point)
        {
            return region.Paths.Any(x => AreCoincident(x, point));
        }

        public bool AreCoincident(IRegion region, IGeometryPathSegment segment)
        {
            return region.Paths.Any(x => AreCoincident(x, segment));
        }

        public bool AreCoincident(IRegion region, IGeometryPath path)
        {
            return region.Paths.Any(x => AreCoincident(x, path));
        }

        public bool AreCoincident(IRegion region1, IRegion region2)
        {
            return region1.Paths.All(path1 => region2.Paths.Any(path2 => AreCoincident(path1, path2)));
        }

        #endregion Coincident check

        #region Calculating shortest line between objects

        public ILine ShortestLineBetween(Point2D pt1, Point2D pt2)
        {
            return new Line(pt1, pt2);
        }

        public ILine ShortestLineBetween(Point2D pt, IInfiniteLine line)
        {
            return new Line(pt, pt.ProjectTo(line));
        }

        public ILine ShortestLineBetween(Point2D pt, IRay line)
        {
            var cp = pt.ProjectTo(line);
            cp = (cp - line.Point1) * line.Vector >= 0 ? cp : line.Point1;
            return new Line(pt, cp);
        }

        public ILine ShortestLineBetween(Point2D pt, ILine line)
        {
            var cp = pt.ProjectTo(line);
            var t1 = (cp - line.StartPoint) * line.Vector;
            var t2 = (cp - line.EndPoint) * line.Vector;
            cp = t1 >= 0 && t2 <= 0 ? cp : t1 >= 0 ? line.EndPoint : line.StartPoint;
            return new Line(pt, cp);
        }

        public ILine ShortestLineBetween(Point2D pt, ICircle circle)
        {
            var vu = (pt - circle.Center).Unit();
            var cp = circle.Center + vu * circle.Radius;
            return new Line(pt, cp);
        }

        public ILine ShortestLineBetween(Point2D pt, IArc arc)
        {
            var vu = (pt - arc.Center).Unit();
            var cp = arc.Center + vu * arc.Radius;
            if (AreCoincident(cp, arc.StartPoint)
                || AreCoincident(cp, arc.EndPoint)
                || vu.Angle.BelongsToAngularSpan(arc.StartAngle, arc.SweepAngle))
            {
                return new Line(pt, cp);
            }
            else
            {
                var sl1 = new Line(pt, arc.StartPoint);
                var sl2 = new Line(pt, arc.EndPoint);
                return sl1.Length() <= sl2.Length() ? sl1 : sl2;
            }
        }

        public ILine ShortestLineBetween(Point2D pt, IGeometryPathSegment segment)
        {
            if (segment is ILine line)
            {
                return ShortestLineBetween(pt, line);
            }
            else if (segment is IArc arc)
            {
                return ShortestLineBetween(pt, arc);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public ILine ShortestLineBetween(Point2D pt, IGeometryPath path)
        {
            ILine result = null;
            double resultLength = double.MaxValue;
            foreach (var asegment in path.Segments)
            {
                var sl = ShortestLineBetween(pt, asegment);
                var length = sl.Length();
                if (result == null || resultLength > length)
                {
                    result = sl;
                    resultLength = length;
                }
            }

            return result;
        }

        public ILine ShortestLineBetween(Point2D pt, IRegion region)
        {
            ILine result = null;
            double resultLength = double.MaxValue;
            foreach (var asegment in region.Paths.SelectMany(x => x.Segments))
            {
                var sl = ShortestLineBetween(pt, asegment);
                var length = sl.Length();
                if (result == null || resultLength > length)
                {
                    result = sl;
                    resultLength = length;
                }
            }

            return result;
        }

        public ILine ShortestLineBetween(IInfiniteLine infLine, Point2D pt) => ShortestLineBetween(pt, infLine).Reverse();
        public ILine ShortestLineBetween(IInfiniteLine infLine1, IInfiniteLine infLine2)
        {
            var ir = Math2D.Intersection(infLine1, infLine2);
            return new Line(ir.Point1, ir.Point2);
        }

        public ILine ShortestLineBetween(IInfiniteLine infLine, IRay ray)
        {
            var ir = Math2D.Intersection(infLine, ray);
            if (ir.AreIntersected)
            {
                var cp1 = ir.Point1;
                var b1 = true;

                var t21 = (ir.Point1 - ray.Point1) * ray.Vector;
                Point2D cp2;
                bool b2 = false;
                if (t21 >= 0)
                {
                    cp2 = ir.Point1;
                    b2 = true;
                }
                else
                {
                    cp2 = ray.Point1;
                }

                if (b1 == b2) return new Line(cp1, cp2);
                if (b1 && !b2) return ShortestLineBetween(cp2, infLine).Reverse();
                else return ShortestLineBetween(cp1, ray);
            }
            else
            {
                return new Line(ir.Point1, ir.Point2);
            }
        }

        public ILine ShortestLineBetween(IInfiniteLine infLine, ILine line)
        {
            var ir = Math2D.Intersection(infLine, line);
            if (ir.AreIntersected)
            {
                var cp1 = ir.Point1;
                var b1 = true;

                var t21 = (ir.Point1 - line.StartPoint) * line.Vector;
                var t22 = (ir.Point1 - line.EndPoint) * line.Vector;
                Point2D cp2;
                bool b2 = false;
                if (t21 >= 0 && t22 <= 0)
                {
                    cp2 = ir.Point1;
                    b2 = true;
                }
                else
                {
                    cp2 = t21 >= 0 ? line.EndPoint : line.StartPoint;
                }

                if (b1 == b2) return new Line(cp1, cp2);
                if (b1 && !b2) return ShortestLineBetween(cp2, infLine).Reverse();
                else return ShortestLineBetween(cp1, line);
            }
            else
            {
                return new Line(ir.Point1, ir.Point2);
            }
        }

        public ILine ShortestLineBetween(IInfiniteLine infLine, ICircle circle)
        {
            var ir = Math2D.Intersection(infLine, circle);
            if (ir.AreIntersected)
            {
                if (ir.Point1.DistanceTo(infLine.Point1) <= ir.Point2.DistanceTo(infLine.Point1))
                    return new Line(ir.Point1, ir.Point1);
                else
                    return new Line(ir.Point2, ir.Point2);
            }
            else
            {
                return new Line(ir.Point1, ir.Point2);
            }
        }

        public ILine ShortestLineBetween(IInfiniteLine infLine, IArc arc)
        {
            var ir = Math2D.Intersection(infLine, arc);
            if (ir.AreIntersected)
            {
                var b1 = AreCoincident(arc, ir.Point1);
                var b2 = AreCoincident(arc, ir.Point2);
                if (b1 && b2)
                {
                    if (ir.Point1.DistanceTo(infLine.Point1) <= ir.Point2.DistanceTo(infLine.Point1))
                        return new Line(ir.Point1, ir.Point1);
                    else
                        return new Line(ir.Point2, ir.Point2);
                }

                if (b1) return new Line(ir.Point1, ir.Point1);
                if (b2) return new Line(ir.Point2, ir.Point2);
            }
            else
            {
                if (AreCoincident(infLine, ir.Point1) && AreCoincident(arc, ir.Point2))
                    return new Line(ir.Point1, ir.Point2);
            }

            var sl3 = ShortestLineBetween(arc.StartPoint, infLine);
            var sl4 = ShortestLineBetween(arc.EndPoint, infLine);
            return sl3.Length() <= sl4.Length() ? sl3.Reverse() : sl4.Reverse();
        }

        public ILine ShortestLineBetween(IInfiniteLine infLine, IGeometryPathSegment segment)
        {
            if (segment is ILine line)
            {
                return ShortestLineBetween(infLine, line);
            }
            else if (segment is IArc arc)
            {
                return ShortestLineBetween(infLine, arc);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public ILine ShortestLineBetween(IInfiniteLine infLine, IGeometryPath path)
        {
            return path.Segments.Select(x => ShortestLineBetween(infLine, x)).OrderBy(x => x.Length()).First();
        }

        public ILine ShortestLineBetween(IInfiniteLine infLine, IRegion region)
        {
            return region.Paths.Select(x => ShortestLineBetween(infLine, x)).OrderBy(x => x.Length()).First();
        }

        public ILine ShortestLineBetween(IRay ray, Point2D pt) => ShortestLineBetween(pt, ray).Reverse();
        public ILine ShortestLineBetween(IRay ray, IInfiniteLine infLine) => ShortestLineBetween(infLine, ray).Reverse();
        public ILine ShortestLineBetween(IRay ray1, IRay ray2)
        {
            var ir = Math2D.Intersection(ray1, ray2);
            if (ir.AreIntersected)
            {
                var t11 = (ir.Point1 - ray1.Point1) * ray1.Vector;
                Point2D cp1;
                bool b1 = false;
                if (t11 >= 0)
                {
                    cp1 = ir.Point1;
                    b1 = true;
                }
                else
                {
                    cp1 = ray1.Point1;
                }

                var t21 = (ir.Point1 - ray2.Point1) * ray2.Vector;
                Point2D cp2;
                bool b2 = false;
                if (t21 >= 0)
                {
                    cp2 = ir.Point1;
                    b2 = true;
                }
                else
                {
                    cp2 = ray2.Point1;
                }

                if (b1 == b2) return new Line(cp1, cp2);
                if (b1 && !b2) return ShortestLineBetween(cp2, ray1).Reverse();
                else return ShortestLineBetween(cp1, ray2);
            }
            else
            {
                return new Line(ir.Point1, ir.Point2);
            }
        }

        public ILine ShortestLineBetween(IRay ray, ILine line)
        {
            var ir = Math2D.Intersection(ray, line);
            if (ir.AreIntersected)
            {
                var t11 = (ir.Point1 - ray.Point1) * ray.Vector;
                Point2D cp1;
                bool b1 = false;
                if (t11 >= 0)
                {
                    cp1 = ir.Point1;
                    b1 = true;
                }
                else
                {
                    cp1 = ray.Point1;
                }

                var t21 = (ir.Point1 - line.StartPoint) * line.Vector;
                var t22 = (ir.Point1 - line.EndPoint) * line.Vector;
                Point2D cp2;
                bool b2 = false;
                if (t21 >= 0 && t22 <= 0)
                {
                    cp2 = ir.Point1;
                    b2 = true;
                }
                else
                {
                    cp2 = t21 >= 0 ? line.EndPoint : line.StartPoint;
                }

                if (b1 == b2) return new Line(cp1, cp2);
                if (b1 && !b2) return ShortestLineBetween(cp2, ray).Reverse();
                else return ShortestLineBetween(cp1, line);
            }
            else
            {
                return new Line(ir.Point1, ir.Point2);
            }
        }

        public ILine ShortestLineBetween(IRay ray, ICircle circle)
        {
            var ir = Math2D.Intersection(ray, circle);
            if (ir.AreIntersected)
            {
                var b1 = AreCoincident(ray, ir.Point1);
                var b2 = AreCoincident(ray, ir.Point2);
                if (b1 && b2)
                {
                    if (ir.Point1.DistanceTo(ray.Point1) <= ir.Point2.DistanceTo(ray.Point1))
                        return new Line(ir.Point1, ir.Point1);
                    else
                        return new Line(ir.Point2, ir.Point2);
                }

                if (b1) return new Line(ir.Point1, ir.Point1);
                if (b2) return new Line(ir.Point2, ir.Point2);
            }
            else
            {
                if (AreCoincident(ray, ir.Point1))
                    return new Line(ir.Point1, ir.Point2);
            }

            return ShortestLineBetween(ray.Point1, circle);
        }

        public ILine ShortestLineBetween(IRay ray, IArc arc)
        {
            var ir = Math2D.Intersection(ray, arc);
            if (ir.AreIntersected)
            {
                var b1 = AreCoincident(ray, ir.Point1) && AreCoincident(arc, ir.Point1);
                var b2 = AreCoincident(ray, ir.Point2) && AreCoincident(arc, ir.Point2);
                if (b1 && b2)
                {
                    if (ir.Point1.DistanceTo(ray.Point1) <= ir.Point2.DistanceTo(ray.Point1))
                        return new Line(ir.Point1, ir.Point1);
                    else
                        return new Line(ir.Point2, ir.Point2);
                }

                if (b1) return new Line(ir.Point1, ir.Point1);
                if (b2) return new Line(ir.Point2, ir.Point2);
            }
            else
            {
                if (AreCoincident(ray, ir.Point1) && AreCoincident(arc, ir.Point2))
                    return new Line(ir.Point1, ir.Point2);
            }

            var sl2 = ShortestLineBetween(ray.Point1, arc);
            var sl3 = ShortestLineBetween(arc.StartPoint, ray);
            var sl4 = ShortestLineBetween(arc.EndPoint, ray);
            return sl2.Length() <= sl3.Length() && sl2.Length() <= sl4.Length() ? sl2 :
                sl3.Length() <= sl4.Length() ? sl3.Reverse() : sl4.Reverse();
        }

        public ILine ShortestLineBetween(IRay ray, IGeometryPathSegment segment)
        {
            if (segment is ILine line)
            {
                return ShortestLineBetween(ray, line);
            }
            else if (segment is IArc arc)
            {
                return ShortestLineBetween(ray, arc);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public ILine ShortestLineBetween(IRay ray, IGeometryPath path)
        {
            return path.Segments.Select(x => ShortestLineBetween(ray, x)).OrderBy(x => x.Length()).First();
        }

        public ILine ShortestLineBetween(IRay ray, IRegion region)
        {
            return region.Paths.Select(x => ShortestLineBetween(ray, x)).OrderBy(x => x.Length()).First();
        }

        public ILine ShortestLineBetween(ILine line, Point2D pt) => ShortestLineBetween(pt, line).Reverse();
        public ILine ShortestLineBetween(ILine line, IInfiniteLine infLine) => ShortestLineBetween(infLine, line).Reverse();
        public ILine ShortestLineBetween(ILine line, IRay ray) => ShortestLineBetween(ray, line).Reverse();
        public ILine ShortestLineBetween(ILine line1, ILine line2)
        {
            var ir = Math2D.Intersection(line1, line2);
            if (ir.AreIntersected)
            {
                var t11 = (ir.Point1 - line1.StartPoint) * line1.Vector;
                var t12 = (ir.Point1 - line1.EndPoint) * line1.Vector;
                Point2D cp1;
                bool b1 = false;
                if (t11 >= 0 && t12 <= 0)
                {
                    cp1 = ir.Point1;
                    b1 = true;
                }
                else
                {
                    cp1 = t11 >= 0 ? line1.EndPoint : line1.StartPoint;
                }

                var t21 = (ir.Point1 - line2.StartPoint) * line2.Vector;
                var t22 = (ir.Point1 - line2.EndPoint) * line2.Vector;
                Point2D cp2;
                bool b2 = false;
                if (t21 >= 0 && t22 <= 0)
                {
                    cp2 = ir.Point1;
                    b2 = true;
                }
                else
                {
                    cp2 = t21 >= 0 ? line2.EndPoint : line2.StartPoint;
                }

                if (b1 == b2) return new Line(cp1, cp2);
                if (b1 && !b2) return ShortestLineBetween(cp2, line1).Reverse();
                else return ShortestLineBetween(cp1, line2);
            }
            else
            {
                return new Line(ir.Point1, ir.Point2);
            }
        }

        public ILine ShortestLineBetween(ILine line, ICircle circle)
        {
            var ir = Math2D.Intersection(line, circle);
            if (ir.AreIntersected)
            {
                var b1 = AreCoincident(line, ir.Point1);
                var b2 = AreCoincident(line, ir.Point2);
                if (b1 && b2)
                {
                    if (ir.Point1.DistanceTo(line.StartPoint) <= ir.Point2.DistanceTo(line.StartPoint))
                        return new Line(ir.Point1, ir.Point1);
                    else
                        return new Line(ir.Point2, ir.Point2);
                }

                if (b1) return new Line(ir.Point1, ir.Point1);
                if (b2) return new Line(ir.Point2, ir.Point2);
            }
            else
            {
                if (AreCoincident(line, ir.Point1))
                    return new Line(ir.Point1, ir.Point2);
            }

            var sl1 = ShortestLineBetween(line.StartPoint, circle);
            var sl2 = ShortestLineBetween(line.EndPoint, circle);
            return sl1.Length() <= sl2.Length() ? sl1 : sl2;
        }

        public ILine ShortestLineBetween(ILine line, IArc arc)
        {
            var ir = Math2D.Intersection(line, arc);
            if (ir.AreIntersected)
            {
                var b1 = AreCoincident(line, ir.Point1) && AreCoincident(arc, ir.Point1);
                var b2 = AreCoincident(line, ir.Point2) && AreCoincident(arc, ir.Point2);
                if (b1 && b2)
                {
                    if (ir.Point1.DistanceTo(line.StartPoint) <= ir.Point2.DistanceTo(line.StartPoint))
                        return new Line(ir.Point1, ir.Point1);
                    else
                        return new Line(ir.Point2, ir.Point2);
                }

                if (b1) return new Line(ir.Point1, ir.Point1);
                if (b2) return new Line(ir.Point2, ir.Point2);
            }
            else
            {
                if (AreCoincident(line, ir.Point1) && AreCoincident(arc, ir.Point2))
                    return new Line(ir.Point1, ir.Point2);
            }

            var sl1 = ShortestLineBetween(line.StartPoint, arc);
            var sl2 = ShortestLineBetween(line.EndPoint, arc);
            var sl3 = ShortestLineBetween(arc.StartPoint, line);
            var sl4 = ShortestLineBetween(arc.EndPoint, line);
            return sl1.Length() <= sl2.Length() && sl1.Length() <= sl3.Length() && sl1.Length() <= sl4.Length() ? sl1 :
                sl2.Length() <= sl3.Length() && sl2.Length() <= sl4.Length() ? sl2 :
                sl3.Length() <= sl4.Length() ? sl3.Reverse() : sl4.Reverse();
        }

        public ILine ShortestLineBetween(ICircle circle, Point2D pt) => ShortestLineBetween(pt, circle).Reverse();
        public ILine ShortestLineBetween(ICircle circle, IInfiniteLine infLine) => ShortestLineBetween(infLine, circle).Reverse();
        public ILine ShortestLineBetween(ICircle circle, IRay ray) => ShortestLineBetween(ray, circle).Reverse();
        public ILine ShortestLineBetween(ICircle circle, ILine line) => ShortestLineBetween(line, circle).Reverse();
        public ILine ShortestLineBetween(ICircle circle1, ICircle circle2)
        {
            var cip = Math2D.Intersection(circle1, circle2);
            return cip.AreIntersected ? new Line(cip.Point1, cip.Point1) : new Line(cip.Point1, cip.Point2);
        }

        public ILine ShortestLineBetween(ICircle circle, IArc arc) => ShortestLineBetween(arc, circle).Reverse();

        public ILine ShortestLineBetween(IArc arc, Point2D pt) => ShortestLineBetween(pt, arc).Reverse();
        public ILine ShortestLineBetween(IArc arc, IInfiniteLine infLine) => ShortestLineBetween(infLine, arc).Reverse();
        public ILine ShortestLineBetween(IArc arc, IRay ray) => ShortestLineBetween(ray, arc).Reverse();
        public ILine ShortestLineBetween(IArc arc, ILine line) => ShortestLineBetween(line, arc).Reverse();
        public ILine ShortestLineBetween(IArc arc, ICircle circle)
        {
            var cip = Math2D.Intersection(arc, circle);
            if (cip.AreIntersected)
            {
                var b1 = AreCoincident(arc, cip.Point1);
                var b2 = AreCoincident(arc, cip.Point2);
                if (b1 && b2)
                {
                    if ((cip.Point1 - arc.Center).Angle.AngularDistanceFromStartOfSweep(arc.StartAngle, arc.SweepAngle >= 0)
                     <= (cip.Point2 - arc.Center).Angle.AngularDistanceFromStartOfSweep(arc.StartAngle, arc.SweepAngle >= 0))
                        return new Line(cip.Point1, cip.Point1);
                    else
                        return new Line(cip.Point2, cip.Point2);
                }

                if (b1) return new Line(cip.Point1, cip.Point1);
                if (b2) return new Line(cip.Point2, cip.Point2);
            }
            else
            {
                if (AreCoincident(arc, cip.Point1)) return new Line(cip.Point1, cip.Point2);
            }

            var sl1 = ShortestLineBetween(arc.StartPoint, circle);
            var sl2 = ShortestLineBetween(arc.EndPoint, circle);
            return sl1.Length() <= sl2.Length() ? sl1 : sl2;
        }

        public ILine ShortestLineBetween(IArc arc1, IArc arc2)
        {
            var cip = Math2D.Intersection(arc1, arc2);
            if (cip.AreIntersected)
            {
                var b1 = AreCoincident(arc1, cip.Point1) && AreCoincident(arc2, cip.Point1);
                var b2 = AreCoincident(arc1, cip.Point2) && AreCoincident(arc2, cip.Point2);
                if (b1 && b2)
                {
                    if ((cip.Point1 - arc1.Center).Angle.AngularDistanceFromStartOfSweep(arc1.StartAngle, arc1.SweepAngle >= 0)
                       <= (cip.Point2 - arc1.Center).Angle.AngularDistanceFromStartOfSweep(arc1.StartAngle, arc1.SweepAngle >= 0))
                        return new Line(cip.Point1, cip.Point1);
                    else
                        return new Line(cip.Point2, cip.Point2);
                }

                if (b1) return new Line(cip.Point1, cip.Point1);
                if (b2) return new Line(cip.Point2, cip.Point2);
            }
            else
            {
                if (AreCoincident(arc1, cip.Point1) && AreCoincident(arc2, cip.Point2)) return new Line(cip.Point1, cip.Point2);
            }

            var sl11 = ShortestLineBetween(arc1.StartPoint, arc2);
            var sl12 = ShortestLineBetween(arc1.EndPoint, arc2);
            var sl21 = ShortestLineBetween(arc2.StartPoint, arc1);
            var sl22 = ShortestLineBetween(arc2.EndPoint, arc1);
            return sl11.Length() <= sl12.Length() && sl11.Length() <= sl21.Length() && sl11.Length() <= sl22.Length() ? sl11 :
                sl12.Length() <= sl21.Length() && sl12.Length() <= sl22.Length() ? sl12 :
                sl21.Length() <= sl22.Length() ? sl21.Reverse() : sl22.Reverse();
        }

        public ILine ShortestLineBetween(IGeometryPathSegment segment, Point2D pt) => ShortestLineBetween(pt, segment).Reverse();
        public ILine ShortestLineBetween(IGeometryPathSegment segment, IInfiniteLine infLine) => ShortestLineBetween(infLine, segment).Reverse();
        public ILine ShortestLineBetween(IGeometryPathSegment segment, IRay ray) => ShortestLineBetween(ray, segment).Reverse();
        public ILine ShortestLineBetween(IGeometryPathSegment segment1, IGeometryPathSegment segment2)
        {
            if (segment1 is ILine line1)
            {
                if (segment2 is ILine line2)
                {
                    return ShortestLineBetween(line1, line2);
                }
                else if (segment2 is IArc arc2)
                {
                    return ShortestLineBetween(line1, arc2);
                }
                else
                    throw new ArgumentException($"Unsupported path segment type: {segment2.GetType()}", nameof(segment2));
            }
            else if (segment1 is IArc arc1)
            {
                if (segment2 is ILine line2)
                {
                    return ShortestLineBetween(arc1, line2);
                }
                else if (segment2 is IArc arc2)
                {
                    return ShortestLineBetween(arc1, arc2);
                }
                else
                    throw new ArgumentException($"Unsupported path segment type: {segment2.GetType()}", nameof(segment2));
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment1.GetType()}", nameof(segment1));
        }

        public ILine ShortestLineBetween(IGeometryPath path, Point2D pt) => ShortestLineBetween(pt, path).Reverse();
        public ILine ShortestLineBetween(IGeometryPath path, IInfiniteLine infLine) => ShortestLineBetween(infLine, path).Reverse();
        public ILine ShortestLineBetween(IGeometryPath path, IRay ray) => ShortestLineBetween(ray, path).Reverse();
        public ILine ShortestLineBetween(IGeometryPath path, IGeometryPathSegment segment)
        {
            ILine result = null;
            double resultLength = double.MaxValue;
            foreach (var asegment in path.Segments)
            {
                var sl = ShortestLineBetween(asegment, segment);
                var length = sl.Length();
                if (result == null || resultLength > length)
                {
                    result = sl;
                    resultLength = length;
                }
            }

            return result;
        }

        public ILine ShortestLineBetween(IGeometryPath path1, IGeometryPath path2)
        {
            ILine result = null;
            double resultLength = double.MaxValue;
            foreach (var asegment1 in path1.Segments)
            {
                foreach (var asegment2 in path2.Segments)
                {
                    var sl = ShortestLineBetween(asegment1, asegment2);
                    var length = sl.Length();
                    if (result == null || resultLength > length)
                    {
                        result = sl;
                        resultLength = length;
                    }
                }
            }

            return result;
        }

        public ILine ShortestLineBetween(IRegion region, Point2D pt) => ShortestLineBetween(pt, region).Reverse();

        public ILine ShortestLineBetween(IRegion region, IInfiniteLine infLine) => ShortestLineBetween(infLine, region).Reverse();
        public ILine ShortestLineBetween(IRegion region, IRay ray) => ShortestLineBetween(ray, region).Reverse();

        public ILine ShortestLineBetween(IRegion region, IGeometryPathSegment segment)
        {
            ILine result = null;
            double resultLength = double.MaxValue;
            foreach (var asegment in region.Paths.SelectMany(x => x.Segments))
            {
                var sl = ShortestLineBetween(asegment, segment);
                var length = sl.Length();
                if (result == null || resultLength > length)
                {
                    result = sl;
                    resultLength = length;
                }
            }

            return result;
        }

        public ILine ShortestLineBetween(IRegion region, IGeometryPath path)
        {
            ILine result = null;
            double resultLength = double.MaxValue;
            foreach (var asegment1 in region.Paths.SelectMany(x => x.Segments))
            {
                foreach (var asegment2 in path.Segments)
                {
                    var sl = ShortestLineBetween(asegment1, asegment2);
                    var length = sl.Length();
                    if (result == null || resultLength > length)
                    {
                        result = sl;
                        resultLength = length;
                    }
                }
            }

            return result;
        }

        public ILine ShortestLineBetween(IRegion region1, IRegion region2)
        {
            ILine result = null;
            double resultLength = double.MaxValue;
            foreach (var asegment1 in region1.Paths.SelectMany(x => x.Segments))
            {
                foreach (var asegment2 in region2.Paths.SelectMany(x => x.Segments))
                {
                    var sl = ShortestLineBetween(asegment1, asegment2);
                    var length = sl.Length();
                    if (result == null || resultLength > length)
                    {
                        result = sl;
                        resultLength = length;
                    }
                }
            }

            return result;
        }

        #endregion Calculating shortest line between objects

        #region Calculating all intersection points between primitives

        public IEnumerable<Point2D> Intersections(Point2D pt1, Point2D pt2)
        {
            var points = new UniquePointList(1, this);
            var tp = (pt1 + pt2) / 2;
            if (AreCoincident(tp, pt1) && AreCoincident(tp, pt2)) points.Add(tp);
            return points;
        }

        public IEnumerable<Point2D> Intersections(Point2D pt, IInfiniteLine infLine) => Intersections(infLine, pt);
        public IEnumerable<Point2D> Intersections(Point2D pt, IRay ray) => Intersections(ray, pt);
        public IEnumerable<Point2D> Intersections(Point2D pt, ILine line) => Intersections(line, pt);
        public IEnumerable<Point2D> Intersections(Point2D pt, ICircle circle) => Intersections(circle, pt);
        public IEnumerable<Point2D> Intersections(Point2D pt, IArc arc) => Intersections(arc, pt);
        public IEnumerable<Point2D> Intersections(Point2D pt, IGeometryPathSegment segment) => Intersections(segment, pt);
        public IEnumerable<Point2D> Intersections(Point2D pt, IGeometryPath path) => Intersections(path, pt);
        public IEnumerable<Point2D> Intersections(Point2D pt, IRegion region) => Intersections(region, pt);

        public IEnumerable<Point2D> Intersections(IInfiniteLine line, Point2D pt)
        {
            var points = new UniquePointList(1, this);
            var tp = (pt + pt.ProjectTo(line)) / 2;
            if (AreCoincident(tp, pt) && AreCoincident(tp, line)) points.Add(tp);
            return points;
        }

        public IEnumerable<Point2D> Intersections(IInfiniteLine infLine1, IInfiniteLine infLine2)
        {
            var points = new UniquePointList(1, this);
            var ir = Math2D.Intersection(infLine1, infLine2);
            var tp = ir.AreIntersected ? ir.Point1 : (ir.Point1 + ir.Point2) / 2;
            if (AreCoincident(tp, infLine1) && AreCoincident(tp, infLine2)) points.Add(tp);
            return points;
        }

        public IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IRay ray)
        {
            var points = new UniquePointList(1, this);
            if (AreCoincident(ray.Point1, infLine)) points.Add(ray.Point1);
            if (points.Count == 0)
            {
                var ir = Math2D.Intersection(infLine, ray);
                var tp = ir.AreIntersected ? ir.Point1 : (ir.Point1 + ir.Point2) / 2;
                if (AreCoincident(tp, infLine) && AreCoincident(tp, ray)) points.Add(tp);
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IInfiniteLine infLine, ILine line)
        {
            var points = new UniquePointList(1, this);
            if (AreCoincident(line.StartPoint, infLine)) points.Add(line.StartPoint);
            if (AreCoincident(line.EndPoint, infLine)) points.Add(line.EndPoint);
            if (points.Count == 0)
            {
                var ir = Math2D.Intersection(infLine, line);
                var tp = ir.AreIntersected ? ir.Point1 : (ir.Point1 + ir.Point2) / 2;
                if (AreCoincident(tp, infLine) && AreCoincident(tp, line)) points.Add(tp);
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IInfiniteLine infLine, ICircle circle)
        {
            var points = new UniquePointList(2, this);
            var ir = Math2D.Intersection(infLine, circle);
            if (ir.AreIntersected)
            {
                var tp = ir.Point1;
                if (AreCoincident(tp, infLine) && AreCoincident(tp, circle)) points.Add(tp);
                tp = ir.Point2;
                if (AreCoincident(tp, infLine) && AreCoincident(tp, circle)) points.Add(tp);
            }
            else
            {
                var tp = (ir.Point1 + ir.Point2);
                if (AreCoincident(tp, infLine) && AreCoincident(tp, circle)) points.Add(tp);
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IArc arc)
        {
            var points = new UniquePointList(2, this);
            if (AreCoincident(arc.StartPoint, infLine)) points.Add(arc.StartPoint);
            if (AreCoincident(arc.EndPoint, infLine)) points.Add(arc.EndPoint);
            if (points.Count < 2)
            {
                var ir = Math2D.Intersection(infLine, arc);
                if (ir.AreIntersected)
                {
                    var tp = ir.Point1;
                    if (AreCoincident(tp, infLine) && AreCoincident(tp, arc)) points.Add(tp);
                    tp = ir.Point2;
                    if (AreCoincident(tp, infLine) && AreCoincident(tp, arc)) points.Add(tp);
                }
                else
                {
                    var tp = (ir.Point1 + ir.Point2);
                    if (AreCoincident(tp, infLine) && AreCoincident(tp, arc)) points.Add(tp);
                }
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IGeometryPathSegment segment)
        {
            if (segment is ILine line)
            {
                return Intersections(infLine, line);
            }
            else if (segment is IArc arc)
            {
                return Intersections(infLine, arc);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IGeometryPath path)
        {
            return path.Segments.SelectMany(x => Intersections(infLine, x));
        }

        public IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IRegion region)
        {
            return region.Paths.SelectMany(x => Intersections(infLine, x));
        }

        public IEnumerable<Point2D> Intersections(IRay ray, Point2D pt)
        {
            var points = new UniquePointList(1, this);
            var tp = (pt + pt.ProjectTo(ray)) / 2;
            if (AreCoincident(tp, ray) && AreCoincident(tp, pt)) points.Add(tp);
            return points;
        }

        public IEnumerable<Point2D> Intersections(IRay ray, IInfiniteLine infLine) => Intersections(infLine, ray);
        public IEnumerable<Point2D> Intersections(IRay ray1, IRay ray2)
        {
            var points = new UniquePointList(1, this);
            if (AreCoincident(ray1.Point1, ray2)) points.Add(ray1.Point1);
            if (AreCoincident(ray2.Point1, ray1)) points.Add(ray2.Point1);
            if (points.Count == 0)
            {
                var ir = Math2D.Intersection(ray1, ray2);
                var tp = ir.AreIntersected ? ir.Point1 : (ir.Point1 + ir.Point2) / 2;
                if (AreCoincident(tp, ray1) && AreCoincident(tp, ray2)) points.Add(tp);
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IRay ray, ILine line)
        {
            var points = new UniquePointList(1, this);
            if (AreCoincident(ray.Point1, line)) points.Add(ray.Point1);
            if (AreCoincident(line.StartPoint, ray)) points.Add(line.StartPoint);
            if (AreCoincident(line.EndPoint, ray)) points.Add(line.EndPoint);
            if (points.Count == 0)
            {
                var ir = Math2D.Intersection(ray, line);
                var tp = ir.AreIntersected ? ir.Point1 : (ir.Point1 + ir.Point2) / 2;
                if (AreCoincident(tp, ray) && AreCoincident(tp, line)) points.Add(tp);
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IRay ray, ICircle circle)
        {
            var points = new UniquePointList(2, this);
            if (AreCoincident(ray.Point1, circle)) points.Add(ray.Point1);
            if (points.Count < 2)
            {
                var ir = Math2D.Intersection(ray, circle);
                if (ir.AreIntersected)
                {
                    var tp = ir.Point1;
                    if (AreCoincident(tp, ray) && AreCoincident(tp, circle)) points.Add(tp);
                    tp = ir.Point2;
                    if (AreCoincident(tp, ray) && AreCoincident(tp, circle)) points.Add(tp);
                }
                else
                {
                    var tp = (ir.Point1 + ir.Point2);
                    if (AreCoincident(tp, ray) && AreCoincident(tp, circle)) points.Add(tp);
                }
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IRay ray, IArc arc)
        {
            var points = new UniquePointList(2, this);
            if (AreCoincident(ray.Point1, arc)) points.Add(ray.Point1);
            if (AreCoincident(arc.StartPoint, ray)) points.Add(arc.StartPoint);
            if (AreCoincident(arc.EndPoint, ray)) points.Add(arc.EndPoint);
            if (points.Count < 2)
            {
                var ir = Math2D.Intersection(ray, arc);
                if (ir.AreIntersected)
                {
                    var tp = ir.Point1;
                    if (AreCoincident(tp, ray) && AreCoincident(tp, arc)) points.Add(tp);
                    tp = ir.Point2;
                    if (AreCoincident(tp, ray) && AreCoincident(tp, arc)) points.Add(tp);
                }
                else
                {
                    var tp = (ir.Point1 + ir.Point2);
                    if (AreCoincident(tp, ray) && AreCoincident(tp, arc)) points.Add(tp);
                }
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IRay ray, IGeometryPathSegment segment)
        {
            if (segment is ILine line)
            {
                return Intersections(ray, line);
            }
            else if (segment is IArc arc)
            {
                return Intersections(ray, arc);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public IEnumerable<Point2D> Intersections(IRay ray, IGeometryPath path)
        {
            return path.Segments.SelectMany(x => Intersections(ray, x));
        }

        public IEnumerable<Point2D> Intersections(IRay ray, IRegion region)
        {
            return region.Paths.SelectMany(x => Intersections(ray, x));
        }

        public IEnumerable<Point2D> Intersections(ILine line, Point2D pt)
        {
            var points = new UniquePointList(1, this);
            var tp = (pt + pt.ProjectTo(line)) / 2;
            if (AreCoincident(tp, line) && AreCoincident(tp, pt)) points.Add(tp);
            return points;
        }

        public IEnumerable<Point2D> Intersections(ILine line, IInfiniteLine infLine) => Intersections(infLine, line);
        public IEnumerable<Point2D> Intersections(ILine line, IRay ray) => Intersections(ray, line);

        public IEnumerable<Point2D> Intersections(ILine line1, ILine line2)
        {
            var points = new UniquePointList(1, this);
            if (AreCoincident(line1.StartPoint, line2)) points.Add(line1.StartPoint);
            if (AreCoincident(line1.EndPoint, line2)) points.Add(line1.EndPoint);
            if (AreCoincident(line2.StartPoint, line1)) points.Add(line2.StartPoint);
            if (AreCoincident(line2.EndPoint, line1)) points.Add(line2.EndPoint);
            if (points.Count == 0)
            {
                var ir = Math2D.Intersection(line1, line2);
                var tp = ir.AreIntersected ? ir.Point1 : (ir.Point1 + ir.Point2) / 2;
                if (AreCoincident(tp, line1) && AreCoincident(tp, line2)) points.Add(tp);
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(ILine line, ICircle circle) => Intersections(circle, line);
        public IEnumerable<Point2D> Intersections(ILine line, IArc arc) => Intersections(arc, line);

        public IEnumerable<Point2D> Intersections(ICircle circle, Point2D pt)
        {
            var points = new UniquePointList(1, this);
            var vu = (pt - circle.Center).Unit();
            var tp = (pt + circle.Center + vu * circle.Radius) / 2;
            if (AreCoincident(tp, circle) && AreCoincident(tp, pt)) points.Add(tp);
            return points;
        }

        public IEnumerable<Point2D> Intersections(ICircle circle, IInfiniteLine infLine) => Intersections(infLine, circle);
        public IEnumerable<Point2D> Intersections(ICircle circle, IRay ray) => Intersections(ray, circle);

        public IEnumerable<Point2D> Intersections(ICircle circle, ILine line)
        {
            var points = new UniquePointList(2, this);
            if (AreCoincident(line.StartPoint, circle)) points.Add(line.StartPoint);
            if (AreCoincident(line.EndPoint, circle)) points.Add(line.EndPoint);
            if (points.Count < 2)
            {
                var ir = Math2D.Intersection(line, circle);
                if (ir.AreIntersected)
                {
                    var tp = ir.Point1;
                    if (AreCoincident(tp, circle) && AreCoincident(tp, line)) points.Add(tp);
                    tp = ir.Point2;
                    if (AreCoincident(tp, circle) && AreCoincident(tp, line)) points.Add(tp);
                }
                else
                {
                    var tp = (ir.Point1 + ir.Point2);
                    if (AreCoincident(tp, circle) && AreCoincident(tp, line)) points.Add(tp);
                }
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(ICircle circle1, ICircle circle2)
        {
            var points = new UniquePointList(2, this);
            var ir = Math2D.Intersection(circle1, circle2);
            if (ir.AreIntersected)
            {
                var tp = ir.Point1;
                if (AreCoincident(tp, circle1) && AreCoincident(tp, circle2)) points.Add(tp);
                tp = ir.Point2;
                if (AreCoincident(tp, circle1) && AreCoincident(tp, circle2)) points.Add(tp);
            }
            else
            {
                var tp = (ir.Point1 + ir.Point2);
                if (AreCoincident(tp, circle1) && AreCoincident(tp, circle2)) points.Add(tp);
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(ICircle circle, IArc arc) => Intersections(arc, circle);

        public IEnumerable<Point2D> Intersections(IArc arc, Point2D pt)
        {
            var points = new UniquePointList(1, this);
            var vu = (pt - arc.Center).Unit();
            var tp = (pt + arc.Center + vu * arc.Radius) / 2;
            if (AreCoincident(tp, arc) && AreCoincident(tp, pt)) points.Add(tp);
            return points;
        }

        public IEnumerable<Point2D> Intersections(IArc arc, IInfiniteLine infLine) => Intersections(infLine, arc);
        public IEnumerable<Point2D> Intersections(IArc arc, IRay ray) => Intersections(ray, arc);

        public IEnumerable<Point2D> Intersections(IArc arc, ILine line)
        {
            var points = new UniquePointList(2, this);
            if (AreCoincident(arc.StartPoint, line)) points.Add(arc.StartPoint);
            if (AreCoincident(arc.EndPoint, line)) points.Add(arc.EndPoint);
            if (AreCoincident(line.StartPoint, arc)) points.Add(line.StartPoint);
            if (AreCoincident(line.EndPoint, arc)) points.Add(line.EndPoint);
            if (points.Count < 2)
            {
                var ir = Math2D.Intersection(line, arc);
                if (ir.AreIntersected)
                {
                    var tp = ir.Point1;
                    if (AreCoincident(tp, arc) && AreCoincident(tp, line)) points.Add(tp);
                    tp = ir.Point2;
                    if (AreCoincident(tp, arc) && AreCoincident(tp, line)) points.Add(tp);
                }
                else
                {
                    var tp = (ir.Point1 + ir.Point2);
                    if (AreCoincident(tp, arc) && AreCoincident(tp, line)) points.Add(tp);
                }
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IArc arc, ICircle circle)
        {
            var points = new UniquePointList(2, this);
            if (AreCoincident(arc.StartPoint, circle)) points.Add(arc.StartPoint);
            if (AreCoincident(arc.EndPoint, circle)) points.Add(arc.EndPoint);
            if (points.Count < 2)
            {
                var ir = Math2D.Intersection(arc, circle);
                if (ir.AreIntersected)
                {
                    var tp = ir.Point1;
                    if (AreCoincident(tp, arc) && AreCoincident(tp, circle)) points.Add(tp);
                    tp = ir.Point2;
                    if (AreCoincident(tp, arc) && AreCoincident(tp, circle)) points.Add(tp);
                }
                else
                {
                    var tp = (ir.Point1 + ir.Point2);
                    if (AreCoincident(tp, arc) && AreCoincident(tp, circle)) points.Add(tp);
                }
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IArc arc1, IArc arc2)
        {
            var points = new UniquePointList(2, this);
            if (AreCoincident(arc1.StartPoint, arc2)) points.Add(arc1.StartPoint);
            if (AreCoincident(arc1.EndPoint, arc2)) points.Add(arc1.EndPoint);
            if (AreCoincident(arc2.StartPoint, arc1)) points.Add(arc2.StartPoint);
            if (AreCoincident(arc2.EndPoint, arc1)) points.Add(arc2.EndPoint);
            if (points.Count < 2)
            {
                var ir = Math2D.Intersection(arc1, arc2);
                if (ir.AreIntersected)
                {
                    var tp = ir.Point1;
                    if (AreCoincident(tp, arc1) && AreCoincident(tp, arc2)) points.Add(tp);
                    tp = ir.Point2;
                    if (AreCoincident(tp, arc1) && AreCoincident(tp, arc2)) points.Add(tp);
                }
                else
                {
                    var tp = (ir.Point1 + ir.Point2);
                    if (AreCoincident(tp, arc1) && AreCoincident(tp, arc2)) points.Add(tp);
                }
            }

            return points;
        }

        public IEnumerable<Point2D> Intersections(IGeometryPathSegment segment, Point2D point)
        {
            if (segment is ILine line)
            {
                return Intersections(line, point);
            }
            else if (segment is IArc arc)
            {
                return Intersections(arc, point);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public IEnumerable<Point2D> Intersections(IGeometryPathSegment segment, IInfiniteLine infLine) => Intersections(infLine, segment);
        public IEnumerable<Point2D> Intersections(IGeometryPathSegment segment, IRay ray) => Intersections(ray, segment);

        public IEnumerable<Point2D> Intersections(IGeometryPathSegment segment1, IGeometryPathSegment segment2)
        {
            if (segment1 is ILine line1)
            {
                if (segment2 is ILine line2)
                {
                    return Intersections(line1, line2);
                }
                else if (segment2 is IArc arc2)
                {
                    return Intersections(arc2, line1);
                }
                else
                    throw new ArgumentException($"Unsupported path segment type: {segment2.GetType()}", nameof(segment2));
            }
            else if (segment1 is IArc arc1)
            {
                if (segment2 is ILine line2)
                {
                    return Intersections(arc1, line2);
                }
                else if (segment2 is IArc arc2)
                {
                    return Intersections(arc1, arc2);
                }
                else
                    throw new ArgumentException($"Unsupported path segment type: {segment2.GetType()}", nameof(segment2));
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment1.GetType()}", nameof(segment1));
        }

        public IEnumerable<Point2D> Intersections(IGeometryPath path, Point2D point)
        {
            var result = new UniquePointList(1, this);
            foreach (var asegment in path.Segments)
            {
                result.AddRange(Intersections(asegment, point));
            }

            return result;
        }

        public IEnumerable<Point2D> Intersections(IGeometryPath path, IInfiniteLine infLine) => Intersections(infLine, path);
        public IEnumerable<Point2D> Intersections(IGeometryPath path, IRay ray) => Intersections(ray, path);

        public IEnumerable<Point2D> Intersections(IGeometryPath path, IGeometryPathSegment segment)
        {
            var result = new UniquePointList(1, this);
            foreach (var asegment in path.Segments)
            {
                result.AddRange(Intersections(asegment, segment));
            }

            return result;
        }

        public IEnumerable<Point2D> Intersections(IGeometryPath path1, IGeometryPath path2)
        {
            var result = new UniquePointList(1, this);
            foreach (var asegment1 in path1.Segments)
            {
                foreach (var asegment2 in path2.Segments)
                {
                    result.AddRange(Intersections(asegment1, asegment2));
                }
            }

            return result;
        }

        public IEnumerable<Point2D> Intersections(IRegion region, Point2D point)
        {
            var result = new UniquePointList(1, this);
            foreach (var asegment in region.Paths.SelectMany(x => x.Segments))
            {
                result.AddRange(Intersections(asegment, point));
            }

            return result;
        }

        public IEnumerable<Point2D> Intersections(IRegion region, IInfiniteLine infLine) => Intersections(infLine, region);
        public IEnumerable<Point2D> Intersections(IRegion region, IRay ray) => Intersections(ray, region);

        public IEnumerable<Point2D> Intersections(IRegion region, IGeometryPathSegment segment)
        {
            var result = new UniquePointList(1, this);
            foreach (var asegment in region.Paths.SelectMany(x => x.Segments))
            {
                result.AddRange(Intersections(asegment, segment));
            }

            return result;
        }

        public IEnumerable<Point2D> Intersections(IRegion region, IGeometryPath path)
        {
            var result = new UniquePointList(1, this);
            foreach (var asegment1 in region.Paths.SelectMany(x => x.Segments))
            {
                foreach (var asegment2 in path.Segments)
                {
                    result.AddRange(Intersections(asegment1, asegment2));
                }
            }

            return result;
        }

        public IEnumerable<Point2D> Intersections(IRegion region1, IRegion region2)
        {
            var result = new UniquePointList(1, this);
            foreach (var asegment1 in region1.Paths.SelectMany(x => x.Segments))
            {
                foreach (var asegment2 in region2.Paths.SelectMany(x => x.Segments))
                {
                    result.AddRange(Intersections(asegment1, asegment2));
                }
            }

            return result;
        }

        private class UniquePointList : List<Point2D>
        {
            private readonly Geometry2DEngine geometry2DEngine;

            public UniquePointList(Geometry2DEngine geometry2DEngine)
                : base()
            {
                this.geometry2DEngine = geometry2DEngine;
            }

            public UniquePointList(int capacity, Geometry2DEngine geometry2DEngine)
                : base(capacity)
            {
                this.geometry2DEngine = geometry2DEngine;
            }

            public new void Add(Point2D point)
            {
                if (!this.Any(x => geometry2DEngine.AreCoincident(x, point)))
                    base.Add(point);
            }

            public new void AddRange(IEnumerable<Point2D> points)
            {
                foreach (var point in points)
                    Add(point);
            }
        }

        #endregion Calculating all intersection points between primitives

        #region Calculating first intersection point between primitives

        public Point2D? FirstIntersection(Point2D pt1, Point2D pt2)
        {
            return AreCoincident(pt1, pt2) ? (Point2D?)pt1 : null;
        }

        public Point2D? FirstIntersection(Point2D pt, IInfiniteLine infLine) => FirstIntersection(infLine, pt);
        public Point2D? FirstIntersection(Point2D pt, IRay ray) => FirstIntersection(ray, pt);
        public Point2D? FirstIntersection(Point2D pt, ILine line) => FirstIntersection(line, pt);
        public Point2D? FirstIntersection(Point2D pt, ICircle circle) => FirstIntersection(circle, pt);
        public Point2D? FirstIntersection(Point2D pt, IArc arc) => FirstIntersection(arc, pt);
        public Point2D? FirstIntersection(Point2D pt, IGeometryPathSegment segment) => FirstIntersection(segment, pt);
        public Point2D? FirstIntersection(Point2D pt, IGeometryPath path) => FirstIntersection(path, pt);
        public Point2D? FirstIntersection(Point2D pt, IRegion region) => FirstIntersection(region, pt);

        public Point2D? FirstIntersection(IInfiniteLine line, Point2D pt)
        {
            var p = pt.ProjectTo(line);
            return AreCoincident(p, pt) ? (Point2D?)pt : null;
        }

        public Point2D? FirstIntersection(IInfiniteLine infLine1, IInfiniteLine infLine2)
        {
            var sl = ShortestLineBetween(infLine1, infLine2);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IInfiniteLine infLine, IRay ray)
        {
            var sl = ShortestLineBetween(infLine, ray);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IInfiniteLine infLine, ILine line)
        {
            var sl = ShortestLineBetween(infLine, line);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IInfiniteLine infLine, ICircle circle)
        {
            var sl = ShortestLineBetween(infLine, circle);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IInfiniteLine infLine, IArc arc)
        {
            var sl = ShortestLineBetween(infLine, arc);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IInfiniteLine infLine, IGeometryPathSegment segment)
        {
            var sl = ShortestLineBetween(infLine, segment);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IInfiniteLine infLine, IGeometryPath path)
        {
            var sl = ShortestLineBetween(infLine, path);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IInfiniteLine infLine, IRegion region)
        {
            var sl = ShortestLineBetween(infLine, region);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRay ray, Point2D pt)
        {
            if (AreCoincident(pt, ray.Point1)) return pt;
            var p = pt.ProjectTo(ray);
            if (!AreCoincident(pt, p)) return null;
            return (p - ray.Point1) * ray.Vector >= 0 ? (Point2D?)pt : null;
        }

        public Point2D? FirstIntersection(IRay ray, IInfiniteLine infLine)
        {
            var sl = ShortestLineBetween(ray, infLine);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRay ray1, IRay ray2)
        {
            var sl = ShortestLineBetween(ray1, ray2);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRay ray, ILine line)
        {
            var sl = ShortestLineBetween(ray, line);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRay ray, ICircle circle)
        {
            var sl = ShortestLineBetween(ray, circle);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRay ray, IArc arc)
        {
            var sl = ShortestLineBetween(ray, arc);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRay ray, IGeometryPathSegment segment)
        {
            var sl = ShortestLineBetween(ray, segment);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRay ray, IGeometryPath path)
        {
            var sl = ShortestLineBetween(ray, path);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRay ray, IRegion region)
        {
            var sl = ShortestLineBetween(ray, region);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ILine line, Point2D pt)
        {
            return AreCoincident(line, pt) ? (Point2D?)pt : null;
        }

        public Point2D? FirstIntersection(ILine line, IInfiniteLine infLine)
        {
            var sl = ShortestLineBetween(line, infLine);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ILine line, IRay ray)
        {
            var sl = ShortestLineBetween(line, ray);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ILine line1, ILine line2)
        {
            var sl = ShortestLineBetween(line1, line2);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ILine line, ICircle circle)
        {
            var sl = ShortestLineBetween(line, circle);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ILine line, IArc arc)
        {
            var sl = ShortestLineBetween(line, arc);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ILine line, IGeometryPathSegment segment)
        {
            var sl = ShortestLineBetween(line, segment);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ICircle circle, Point2D pt)
        {
            return AreCoincident(circle, pt) ? (Point2D?)pt : null;
        }

        public Point2D? FirstIntersection(ICircle circle, IInfiniteLine infLine)
        {
            var sl = ShortestLineBetween(circle, infLine);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ICircle circle, IRay ray)
        {
            var sl = ShortestLineBetween(circle, ray);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ICircle circle, ILine line)
        {
            var sl = ShortestLineBetween(circle, line);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ICircle circle1, ICircle circle2)
        {
            var sl = ShortestLineBetween(circle1, circle2);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(ICircle circle, IArc arc) => FirstIntersection(arc, circle);

        public Point2D? FirstIntersection(IArc arc, Point2D pt)
        {
            return AreCoincident(arc, pt) ? (Point2D?)pt : null;
        }

        public Point2D? FirstIntersection(IArc arc, IInfiniteLine infLine)
        {
            var sl = ShortestLineBetween(arc, infLine);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IArc arc, IRay ray)
        {
            var sl = ShortestLineBetween(arc, ray);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IArc arc, ILine line)
        {
            var sl = ShortestLineBetween(arc, line);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IArc arc, ICircle circle)
        {
            var sl = ShortestLineBetween(arc, circle);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IArc arc1, IArc arc2)
        {
            var sl = ShortestLineBetween(arc1, arc2);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IGeometryPathSegment segment, Point2D point)
        {
            if (segment is ILine line)
            {
                return FirstIntersection(line, point);
            }
            else if (segment is IArc arc)
            {
                return FirstIntersection(arc, point);
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment.GetType()}", nameof(segment));
        }

        public Point2D? FirstIntersection(IGeometryPathSegment segment, IInfiniteLine infLine)
        {
            var sl = ShortestLineBetween(segment, infLine);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IGeometryPathSegment segment, IRay ray)
        {
            var sl = ShortestLineBetween(segment, ray);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IGeometryPathSegment segment1, IGeometryPathSegment segment2)
        {
            if (segment1 is ILine line1)
            {
                if (segment2 is ILine line2)
                {
                    return FirstIntersection(line1, line2);
                }
                else if (segment2 is IArc arc2)
                {
                    return FirstIntersection(arc2, line1);
                }
                else
                    throw new ArgumentException($"Unsupported path segment type: {segment2.GetType()}", nameof(segment2));
            }
            else if (segment1 is IArc arc1)
            {
                if (segment2 is ILine line2)
                {
                    return FirstIntersection(line2, arc1);
                }
                else if (segment2 is IArc arc2)
                {
                    return FirstIntersection(arc1, arc2);
                }
                else
                    throw new ArgumentException($"Unsupported path segment type: {segment2.GetType()}", nameof(segment2));
            }
            else
                throw new ArgumentException($"Unsupported path segment type: {segment1.GetType()}", nameof(segment1));
        }

        public Point2D? FirstIntersection(IGeometryPath path, Point2D point)
        {
            Point2D? result = null;
            foreach (var asegment in path.Segments)
            {
                result = FirstIntersection(asegment, point);
                if (result.HasValue) return result;
            }

            return result;
        }

        public Point2D? FirstIntersection(IGeometryPath path, IInfiniteLine infLine)
        {
            var sl = ShortestLineBetween(path, infLine);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IGeometryPath path, IRay ray)
        {
            var sl = ShortestLineBetween(path, ray);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IGeometryPath path, IGeometryPathSegment segment)
        {
            Point2D? result = null;
            foreach (var asegment in path.Segments)
            {
                result = FirstIntersection(asegment, segment);
                if (result.HasValue) return result;
            }

            return result;
        }

        public Point2D? FirstIntersection(IGeometryPath path1, IGeometryPath path2)
        {
            Point2D? result = null;
            foreach (var asegment1 in path1.Segments)
            {
                foreach (var asegment2 in path2.Segments)
                {
                    result = FirstIntersection(asegment1, asegment2);
                    if (result.HasValue) return result;
                }
            }

            return result;
        }

        public Point2D? FirstIntersection(IRegion region, Point2D point)
        {
            Point2D? result = null;
            foreach (var asegment1 in region.Paths.SelectMany(x => x.Segments))
            {
                result = FirstIntersection(asegment1, point);
                if (result.HasValue) return result;
            }

            return result;
        }

        public Point2D? FirstIntersection(IRegion region, IInfiniteLine infLine)
        {
            var sl = ShortestLineBetween(region, infLine);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRegion region, IRay ray)
        {
            var sl = ShortestLineBetween(region, ray);
            return AreCoincident(sl.StartPoint, sl.EndPoint) ? (Point2D?)sl.StartPoint : null;
        }

        public Point2D? FirstIntersection(IRegion region, IGeometryPathSegment segment)
        {
            Point2D? result = null;
            foreach (var asegment1 in region.Paths.SelectMany(x => x.Segments))
            {
                result = FirstIntersection(asegment1, segment);
                if (result.HasValue) return result;
            }

            return result;
        }

        public Point2D? FirstIntersection(IRegion region, IGeometryPath path)
        {
            Point2D? result = null;
            foreach (var asegment1 in region.Paths.SelectMany(x => x.Segments))
            {
                foreach (var asegment2 in path.Segments)
                {
                    result = FirstIntersection(asegment1, asegment2);
                    if (result.HasValue) return result;
                }
            }

            return result;
        }

        public Point2D? FirstIntersection(IRegion region1, IRegion region2)
        {
            Point2D? result = null;
            foreach (var asegment1 in region1.Paths.SelectMany(x => x.Segments))
            {
                foreach (var asegment2 in region2.Paths.SelectMany(x => x.Segments))
                {
                    result = FirstIntersection(asegment1, asegment2);
                    if (result.HasValue) return result;
                }
            }

            return result;
        }

        #endregion Calculating first intersection point between primitives

        #region Paths and regions

        public IGeometryPathBuilder CreatePathBuilder() => new GeometryPathBuilder(this);
        public IRegionBuilder CreateRegionBuilder() => new RegionBuilder(this);
        public IRegionComposer CreateRegionComposer() => new RegionComposer(this);

        public IGeometryPath Reduce(IGeometryPath path)
        {
            var pathBuilder = CreatePathBuilder();
            pathBuilder.BeginPath(allowReduce: true);
            pathBuilder.Add(path.Segments);
            return pathBuilder.EndPath(path.IsClosed);
        }

        public IRegion Reduce(IRegion region)
        {
            var regionBuilder = CreateRegionBuilder();
            regionBuilder.BeginRegion();
            regionBuilder.Add(region.Paths.Select(x => Reduce(x)));
            return regionBuilder.EndRegion();
        }

        public IEnumerable<IGeometryPath> FindCommonPaths(IGeometryPath path1, IGeometryPath path2)
        {
            Func<IGeometryPath, IGeometryPath, IReadOnlyList<IGeometryPathSegment>> addExtraVertices = (pathToExtend, otherPath) =>
            {
                // collect intersection points that are not path's vertices
                var splitPointMap = new Dictionary<IGeometryPathSegment, UniquePointList>();
                Action<IGeometryPathSegment, Point2D> addSplitPointIfNeed = (segment, splitPoint) =>
                {
                    if (AreCoincident(segment.StartPoint, splitPoint) || AreCoincident(segment.EndPoint, splitPoint)) return;
                    if (!splitPointMap.TryGetValue(segment, out var splitList))
                    {
                        splitList = new UniquePointList(this);
                        splitPointMap.Add(segment, splitList);
                    }

                    splitList.Add(splitPoint);
                };

                // intersection points
                foreach (var segment in pathToExtend.Segments)
                    foreach (var splitPoint in Intersections(otherPath, segment))
                        addSplitPointIfNeed(segment, splitPoint);

                if (splitPointMap.Count == 0) return pathToExtend.Segments;

                var extendedSegments = new List<IGeometryPathSegment>(pathToExtend.Segments.Count + splitPointMap.Values.Sum(x => x.Count));
                foreach (var segment in pathToExtend.Segments)
                {
                    var nextSegment = segment;
                    if (splitPointMap.TryGetValue(segment, out var splitList))
                    {
                        foreach (var splitPoint in splitList.OrderBy(p => segment.LengthFromStart(p)))
                        {
                            var split = nextSegment.Split(splitPoint);
                            extendedSegments.Add(split.PrevSegment);
                            nextSegment = split.NextSegment;
                        }
                    }

                    extendedSegments.Add(nextSegment);
                }

                return extendedSegments;
            };

            var pathEx1 = addExtraVertices(path1, path2);
            var pathEx2 = addExtraVertices(path2, path1);
            var commonPaths = FindCommonPaths(pathEx1, pathEx2, false);
            var reverseCommonPaths = FindCommonPaths(pathEx1, pathEx2, true);
            foreach (var reverseCommonPath in reverseCommonPaths)
            {
                var normalMatchingReverse = commonPaths.Where(p => p.All(s => reverseCommonPath.Any(rs => AreCoincident(s.Item1, rs.Item1)))).ToList();
                if (normalMatchingReverse.Count > 0)
                {
                    var normalCommonPathsToRemove = commonPaths.Where(p => normalMatchingReverse.Contains(p) && p.Count < reverseCommonPath.Count).ToList();
                    if (normalCommonPathsToRemove.Count > 0)
                    {
                        foreach (var normalCommonPathToRemove in normalCommonPathsToRemove)
                            commonPaths.Remove(normalCommonPathToRemove);
                        commonPaths.AddLast(reverseCommonPath);
                    }
                }
                else
                {
                    commonPaths.AddLast(reverseCommonPath);
                }
            }

            var result = new List<IGeometryPath>(commonPaths.Count);
            foreach (var commonPath in commonPaths)
            {
                var length1 = commonPath.Sum(x => x.Item1.Length());
                var length2 = commonPath.Sum(x => x.Item2.Length());
                var resultPath = this.CreatePath(false, commonPath.Select(x => length1 <= length2 ? x.Item1 : x.Item2));
                if (resultPath.Segments.Count > 0)
                    result.Add(resultPath);
            }

            return result;
        }

        private LinkedList<LinkedList<Tuple<IGeometryPathSegment, IGeometryPathSegment>>> FindCommonPaths(IReadOnlyList<IGeometryPathSegment> path1, IReadOnlyList<IGeometryPathSegment> path2, bool reversePath2, Func<IGeometryPathSegment, IGeometryPathSegment, bool> areCommonCheck = null)
        {
            if (areCommonCheck == null)
                areCommonCheck = (segment1, segment2) =>
                {
                    return AreCoincident(segment1.StartPoint, segment2.StartPoint) &&
                        AreCoincident(segment1.EndPoint, segment2.EndPoint) &&
                        AreCoincident(segment1.Midpoint(), segment2);
                };

            var commonSegmentGroups = new LinkedList<LinkedList<Tuple<IGeometryPathSegment, IGeometryPathSegment>>>();
            LinkedList<Tuple<IGeometryPathSegment, IGeometryPathSegment>> currentCommonSegmentGroup = null;
            var enum1 = new PathSegmentIterator(path1, null).Segments.GetEnumerator();
            IEnumerator<IGeometryPathSegment> enum2 = null;
            bool firstCommonSegmentFound;
            bool skipEnum1Move = false;
            bool enum1NotEnd = true;
            do
            {
                firstCommonSegmentFound = false;
                while (!firstCommonSegmentFound && (skipEnum1Move || enum1.MoveNext()))
                {
                    skipEnum1Move = false;
                    var enum2Start = currentCommonSegmentGroup != null ? enum2.Current : null;
                    enum2 = new PathSegmentIterator(path2, enum2Start, reversePath2).Segments.GetEnumerator();
                    while (!firstCommonSegmentFound && enum2.MoveNext())
                        firstCommonSegmentFound = areCommonCheck(enum1.Current, reversePath2 ? enum2.Current.Reverse() : enum2.Current);
                }

                if (firstCommonSegmentFound)
                {
                    enum2 = new PathSegmentIterator(path2, enum2.Current, reversePath2).Segments.GetEnumerator();
                    enum2.MoveNext();
                    if (currentCommonSegmentGroup != null)
                        commonSegmentGroups.AddLast(currentCommonSegmentGroup);
                    currentCommonSegmentGroup = new LinkedList<Tuple<IGeometryPathSegment, IGeometryPathSegment>>();
                    currentCommonSegmentGroup.AddLast(new Tuple<IGeometryPathSegment, IGeometryPathSegment>(enum1.Current, reversePath2 ? enum2.Current.Reverse() : enum2.Current));

                    while ((enum1NotEnd = enum1.MoveNext()) && enum2.MoveNext() && areCommonCheck(enum1.Current, reversePath2 ? enum2.Current.Reverse() : enum2.Current))
                        currentCommonSegmentGroup.AddLast(new Tuple<IGeometryPathSegment, IGeometryPathSegment>(enum1.Current, reversePath2 ? enum2.Current.Reverse() : enum2.Current));
                    skipEnum1Move = true;
                }
            }
            while (firstCommonSegmentFound && enum1NotEnd);
            if (currentCommonSegmentGroup != null)
                commonSegmentGroups.AddLast(currentCommonSegmentGroup);
            if (commonSegmentGroups.Count > 1 && commonSegmentGroups.First.Value.First.Value.Item1.StartPoint == commonSegmentGroups.Last.Value.Last.Value.Item1.EndPoint)
            {
                foreach (var s in commonSegmentGroups.First.Value)
                    commonSegmentGroups.Last.Value.AddLast(s);
                commonSegmentGroups.RemoveFirst();
            }

            return commonSegmentGroups;
        }

        public IEnumerable<IGeometryPath> FindCommonPaths(IRegion region1, IRegion region2)
        {
            var result = new List<IGeometryPath>();
            for (int i = 0; i < region1.Paths.Count; i++)
                for (int j = 0; j < region2.Paths.Count; j++)
                    foreach (var commonPath in FindCommonPaths(region1.Paths[i], region2.Paths[j]))
                        result.Add(commonPath);
            return result;
        }

        public IRectangle FindMinimalRectangleAroundPath(IGeometryPath path, Vector2D? direction = null)
        {
            Func<Vector2D, IGeometryPath, IRectangle> calcMinRectInDirection = (baseVector, apath) =>
            {
                var baseVector2 = baseVector.Orthogonal();
                var tempBottom = CreateInfiniteLine(apath.BoundingRect.Center - baseVector2 * apath.BoundingRect.MaxSize * 2, baseVector);
                var tempRight = CreateInfiniteLine(apath.BoundingRect.Center + baseVector * apath.BoundingRect.MaxSize * 2, baseVector2);
                var tempTop = CreateInfiniteLine(apath.BoundingRect.Center + baseVector2 * apath.BoundingRect.MaxSize * 2, baseVector);
                var tempLeft = CreateInfiniteLine(apath.BoundingRect.Center - baseVector * apath.BoundingRect.MaxSize * 2, baseVector2);
                var closestBottom = apath.Segments.Select(s => ShortestLineBetween(s, tempBottom)).OrderBy(sl => sl.Length()).First();
                var closestRight = apath.Segments.Select(s => ShortestLineBetween(s, tempRight)).OrderBy(sl => sl.Length()).First();
                var closestTop = apath.Segments.Select(s => ShortestLineBetween(s, tempTop)).OrderBy(sl => sl.Length()).First();
                var closestLeft = apath.Segments.Select(s => ShortestLineBetween(s, tempLeft)).OrderBy(sl => sl.Length()).First();
                var bottom = CreateInfiniteLine(closestBottom.StartPoint, baseVector);
                var right = CreateInfiniteLine(closestRight.StartPoint, baseVector2);
                var top = CreateInfiniteLine(closestTop.StartPoint, baseVector);
                var left = CreateInfiniteLine(closestLeft.StartPoint, baseVector2);
                var lb = Math2D.Intersection(left, bottom).Point1;
                var rt = Math2D.Intersection(right, top).Point1;
                return CreateRectangle(lb, rt, baseVector.Angle);
            };

            if (direction.HasValue)
                return calcMinRectInDirection(direction.Value, path);

            IRectangle result = null;
            double minSquare = double.MaxValue;
            foreach (var segment in path.Segments)
            {
                var rect = calcMinRectInDirection(segment.Tangent(segment.Midpoint()).Orthogonal(), path);
                if (minSquare > rect.Area())
                {
                    minSquare = rect.Area();
                    result = rect;
                }
            }

            // try axis-aligned rect
            var aarect = calcMinRectInDirection(new Vector2D(1, 0), path);
            if (minSquare > aarect.Area())
            {
                minSquare = aarect.Area();
                result = aarect;
            }

            return result;
        }

        public IPolygon FindMinimalPoly4AroundPath(IGeometryPath path)
        {
            Func<IInfiniteLine, IGeometryPath, IInfiniteLine> findClosestLine = (aline, apath) =>
            {
                var closestLine = aline;
                var error = Vector2D.Zero;
                do
                {
                    var shortestLines = new List<ILine>();
                    foreach (var s in apath.Segments)
                    {
                        var sl = ShortestLineBetween(s, closestLine);
                        var isBehindOtherSegments = apath.Segments
                            .Where(s1 => s1 != s)
                            .SelectMany(s1 => Intersections(s1, sl))
                            .Where(ip => ip != sl.StartPoint)
                            .Any();
                        if (isBehindOtherSegments) continue;
                        shortestLines.Add(sl);
                    }

                    if (shortestLines.Count == 0) return closestLine;
                    if (shortestLines.Count == 1) return CreateInfiniteLine(shortestLines[0].StartPoint, closestLine.Vector);
                    shortestLines = shortestLines.OrderBy(x => x.Length()).ToList();
                    var d1 = shortestLines.First().Length();
                    var bestLines = shortestLines.Where(x => (x.Length() - d1) < Epsilon).ToList();
                    if (bestLines.Count == 1)
                    {
                        var d2 = shortestLines.Where(x => (x.Length() - d1) >= Epsilon).First().Length();
                        bestLines.AddRange(shortestLines.Where(x => x.Length() >= d2 && (x.Length() - d2) < Epsilon));
                    }

                    var orderedBestLines = bestLines.OrderBy(x => x.StartPoint.DistanceTo(apath.BoundingRect.LB)).ToList();
                    var p1 = orderedBestLines.First().StartPoint;
                    var p2 = orderedBestLines.Last().StartPoint;
                    closestLine = CreateInfiniteLine(p1, (p2 - p1));

                    var outsidePoints = new List<Point2D>();
                    foreach (var s in apath.Segments)
                    {
                        var intersections = Intersections(s, closestLine).ToList();
                        if (s is ILine line)
                        {
                            outsidePoints.AddRange(intersections.Where(p => !AreCoincident(p, line.StartPoint) && !AreCoincident(p, line.EndPoint)));
                        }
                        else if (s is IArc arc)
                        {
                            if (intersections.Count > 1 && !AreCoincident(intersections[0], intersections[1]))
                                outsidePoints.Add(arc.Center + (arc.Center.ProjectTo(closestLine) - arc.Center).Unit() * arc.Radius);
                        }
                    }

                    error = outsidePoints.Select(x => x - x.ProjectTo(closestLine)).OrderBy(x => x.Norm()).FirstOrDefault();
                }
                while (error.Norm() > Epsilon);
                return closestLine;
            };

            var minRect = FindMinimalRectangleAroundPath(path);
            if (minRect.Width < Epsilon || minRect.Height < Epsilon) return minRect;
            var b = CreateLine(minRect.Vertices[0], minRect.Vertices[1]);
            var r = CreateLine(minRect.Vertices[1], minRect.Vertices[2]);
            var t = CreateLine(minRect.Vertices[2], minRect.Vertices[3]);
            var l = CreateLine(minRect.Vertices[3], minRect.Vertices[0]);
            var bcinf = findClosestLine(b.AsInfinite(), path);
            var rcinf = findClosestLine(r.AsInfinite(), path);
            var tcinf = findClosestLine(t.AsInfinite(), path);
            var lcinf = findClosestLine(l.AsInfinite(), path);
            var lb = Math2D.Intersection(lcinf, bcinf).Point1;
            var rb = Math2D.Intersection(rcinf, bcinf).Point1;
            var rt = Math2D.Intersection(rcinf, tcinf).Point1;
            var lt = Math2D.Intersection(lcinf, tcinf).Point1;
            var result = CreatePolygon(lb, rb, rt, lt);
            return result;
        }

        public IGeometryPath Offset(IGeometryPath path, double offset, OffsetMode mode)
        {
            if (path.Winding == 0)
                throw new ArgumentException("Path must be closed and have nonzero winding", nameof(path));
            var newSegments = new LinkedList<IGeometryPathSegment>();
            foreach (var segment in path.Segments)
            {
                var newStartPoint = SnapToGrid(segment.StartPoint - segment.Tangent(segment.StartPoint).Orthogonal().Unit() * offset);
                var newEndPoint = SnapToGrid(segment.EndPoint - segment.Tangent(segment.EndPoint).Orthogonal().Unit() * offset);
                IGeometryPathSegment newSegment;
                switch (segment)
                {
                    case ILine line:
                        newSegment = new Line(newStartPoint, newEndPoint);
                        break;
                    case IArc arc:
                        var newMidpoint = SnapToGrid(segment.Midpoint() - segment.Tangent(segment.Midpoint()).Orthogonal().Unit() * offset);
                        newSegment = new Arc(newStartPoint, newEndPoint, newMidpoint);
                        break;
                    default:
                        throw new Exception($"Unsupported path segment type: {segment.GetType()}");
                }

                newSegments.AddLast(newSegment);
            }

            var prevNewSegmentNode = newSegments.Last;
            var nextNewSegmentNode = newSegments.First;
            while (nextNewSegmentNode != null)
            {
                var prevNewSegment = prevNewSegmentNode.Value;
                var nextNewSegment = nextNewSegmentNode.Value;
                if (prevNewSegment.EndPoint != nextNewSegment.StartPoint)
                {
                    var firstIntersection = FirstIntersection(prevNewSegment, nextNewSegment);
                    if (firstIntersection.HasValue)
                    {
                        var splitPoint = SnapToGrid(firstIntersection.Value);
                        var prevSplit = prevNewSegment.Split(splitPoint);
                        var nextSplit = nextNewSegment.Split(splitPoint);
                        prevNewSegmentNode = newSegments.AddAfter(prevNewSegmentNode, prevSplit.PrevSegment);
                        nextNewSegmentNode = newSegments.AddBefore(nextNewSegmentNode, nextSplit.NextSegment);
                        newSegments.Remove(prevNewSegmentNode.Previous);
                        newSegments.Remove(nextNewSegmentNode.Next);
                    }
                    else
                    {
                        switch (mode)
                        {
                            case OffsetMode.Squared:
                                var lineJoint = new Line(prevNewSegment.EndPoint, nextNewSegment.StartPoint);
                                newSegments.AddAfter(prevNewSegmentNode, lineJoint);
                                break;
                            case OffsetMode.Rounded:
                                var arcJoint = new Arc(prevNewSegment.EndPoint, nextNewSegment.StartPoint, Math.Abs(offset), path.Winding < 0);
                                newSegments.AddAfter(prevNewSegmentNode, arcJoint);
                                break;
                        }
                    }
                }

                prevNewSegmentNode = nextNewSegmentNode;
                nextNewSegmentNode = nextNewSegmentNode.Next;
            }

            var pathBuilder = new GeometryPathBuilder(this);
            pathBuilder.BeginPath();
            pathBuilder.Add(newSegments);
            return pathBuilder.EndPath(true);
        }

        public double ComputeMaxDiff(IGeometryPath path1, IGeometryPath path2)
        {
            double result = 0;
            var pathVertexIterator1 = new GeometryPathVertexIterator(path1);
            foreach (var vertex1 in pathVertexIterator1.Vertices)
            {
                var minDistance = ShortestLineBetween(vertex1.Point, path2).Length();
                if (result < minDistance)
                    result = minDistance;
                if (vertex1.NextSegment is IArc arc1)
                {
                    minDistance = ShortestLineBetween(arc1.Midpoint(), path2).Length();
                    if (result < minDistance)
                        result = minDistance;
                }
            }

            var pathVertexIterator2 = new GeometryPathVertexIterator(path2);
            foreach (var vertex2 in pathVertexIterator2.Vertices)
            {
                var minDistance = ShortestLineBetween(vertex2.Point, path1).Length();
                if (result < minDistance)
                    result = minDistance;
                if (vertex2.NextSegment is IArc arc2)
                {
                    minDistance = ShortestLineBetween(arc2.Midpoint(), path1).Length();
                    if (result < minDistance)
                        result = minDistance;
                }
            }

            return result;
        }

        public IGeometryPath SnapPathToRefPath(IGeometryPath pathToSnap, IGeometryPath refPath, double distanceThreshold)
        {
            Func<IGeometryPath, IGeometryPath, IReadOnlyList<IGeometryPathSegment>> addExtraVertices = (pathToExtend, otherPath) =>
            {
                // collect closest & intersection points that are not path's vertices
                var splitPointMap = new Dictionary<IGeometryPathSegment, UniquePointList>();
                Action<IGeometryPathSegment, Point2D> addSplitPointIfNeed = (segment, splitPoint) =>
                {
                    if (AreCoincident(segment.StartPoint, splitPoint) || AreCoincident(segment.EndPoint, splitPoint)) return;
                    if (!splitPointMap.TryGetValue(segment, out var splitList))
                    {
                        splitList = new UniquePointList(this);
                        splitPointMap.Add(segment, splitList);
                    }

                    splitList.Add(splitPoint);
                };

                // intersection points
                foreach (var segment in pathToExtend.Segments)
                    foreach (var splitPoint in Intersections(otherPath, segment))
                        addSplitPointIfNeed(segment, splitPoint);

                // closest points
                var otherPathVertexIterator = new GeometryPathVertexIterator(otherPath);
                foreach (var otherPathVertex in otherPathVertexIterator.Vertices)
                {
                    var shortestLineToSubj = pathToExtend.Segments.Select(x => new
                    {
                        Segment = x,
                        ShortestLine = ShortestLineBetween(otherPathVertex.Point, x)
                    })
                    .Where(x => x.ShortestLine.Length() < distanceThreshold)
                    .OrderBy(x => x.ShortestLine.Length())
                    .FirstOrDefault();
                    if (shortestLineToSubj != null)
                        addSplitPointIfNeed(shortestLineToSubj.Segment, shortestLineToSubj.ShortestLine.EndPoint);
                }

                if (splitPointMap.Count == 0) return pathToExtend.Segments;

                // split path at collected points
                var extendedSegments = new List<IGeometryPathSegment>(pathToExtend.Segments.Count + splitPointMap.Values.Sum(x => x.Count));
                foreach (var segment in pathToExtend.Segments)
                {
                    var nextSegment = segment;
                    if (splitPointMap.TryGetValue(segment, out var splitList))
                    {
                        foreach (var splitPoint in splitList.OrderBy(p => segment.LengthFromStart(p)))
                        {
                            var split = nextSegment.Split(splitPoint);
                            extendedSegments.Add(split.PrevSegment);
                            nextSegment = split.NextSegment;
                        }
                    }

                    extendedSegments.Add(nextSegment);
                }

                return extendedSegments;
            };

            // find common (with distanceThreshold) paths
            var pathSegments = addExtraVertices(pathToSnap, refPath);
            var refPathSegments = addExtraVertices(refPath, pathToSnap);
            Func<IGeometryPathSegment, IGeometryPathSegment, bool> areCommonCheck = (segment1, segment2) =>
            {
                return segment1.StartPoint.DistanceTo(segment2.StartPoint) < distanceThreshold &&
                    segment1.EndPoint.DistanceTo(segment2.EndPoint) < distanceThreshold &&
                    segment1.Midpoint().DistanceTo(segment2.Midpoint()) < distanceThreshold;
            };
            var commonPaths = FindCommonPaths(pathSegments, refPathSegments, false, areCommonCheck);
            var reverseCommonPaths = FindCommonPaths(pathSegments, refPathSegments, true, areCommonCheck);
            foreach (var reverseCommonPath in reverseCommonPaths)
            {
                var normalMatchingReverse = commonPaths.Where(p => p.All(s => reverseCommonPath.Any(rs => AreCoincident(s.Item1, rs.Item1)))).ToList();
                if (normalMatchingReverse.Count > 0)
                {
                    var normalCommonPathsToRemove = commonPaths.Where(p => normalMatchingReverse.Contains(p) && p.Count < reverseCommonPath.Count).ToList();
                    if (normalCommonPathsToRemove.Count > 0)
                    {
                        foreach (var normalCommonPathToRemove in normalCommonPathsToRemove)
                            commonPaths.Remove(normalCommonPathToRemove);
                        commonPaths.AddLast(reverseCommonPath);
                    }
                }
                else if (!commonPaths.Any(p => reverseCommonPath.All(rs => p.Any(s => AreCoincident(s.Item1, rs.Item1)))))
                {
                    commonPaths.AddLast(reverseCommonPath);
                }
            }

            // build snapped path replacing common parts of path with ref parts
            var commonSegments = commonPaths.SelectMany(x => x);
            var pathBuilder = CreatePathBuilder();
            pathBuilder.BeginPath();
            foreach (var pathSegment in pathSegments)
            {
                var commonSegment = commonSegments.Where(s => s.Item1 == pathSegment).FirstOrDefault();
                pathBuilder.Add(commonSegment != null ? commonSegment.Item2 : pathSegment);
            }

            return pathBuilder.EndPath(pathToSnap.IsClosed);
        }

        #endregion Paths and regions

        private class PathSegmentIterator
        {
            private readonly IReadOnlyList<IGeometryPathSegment> path;
            private readonly bool isClosed;
            private readonly int startSegmentIndex;
            private readonly bool reverse;

            public PathSegmentIterator(IReadOnlyList<IGeometryPathSegment> path, int startSegmentIndex, bool reverse = false)
            {
                this.path = path;
                isClosed = path.Count != 0 && path.First().StartPoint == path.Last().EndPoint;
                this.startSegmentIndex = startSegmentIndex;
                this.reverse = reverse;
            }

            public PathSegmentIterator(IReadOnlyList<IGeometryPathSegment> path, IGeometryPathSegment startSegment, bool reverse = false)
            {
                this.path = path;
                isClosed = path.Count != 0 && path.First().StartPoint == path.Last().EndPoint;
                if (startSegment == null)
                    this.startSegmentIndex = reverse ? path.Count - 1 : 0;
                else
                {
                    var item = path.Select((x, i) => new
                    {
                        Segment = x,
                        Index = i
                    })
                    .Where(x => x.Segment == startSegment)
                    .FirstOrDefault();
                    if (item == null)
                        throw new ArgumentException("Segment not found", nameof(startSegment));
                    this.startSegmentIndex = item.Index;
                }

                this.reverse = reverse;
            }

            public IEnumerable<IGeometryPathSegment> Segments
            {
                get
                {
                    var n = isClosed ? path.Count : reverse ? startSegmentIndex + 1 : path.Count - startSegmentIndex;
                    for (int i = 0; i < n; i++)
                    {
                        var j = reverse ? startSegmentIndex - i : startSegmentIndex + i;
                        if (j < 0) j += path.Count;
                        else if (j >= path.Count) j -= path.Count;
                        yield return path[j];
                    }
                }
            }
        }
    }

    public class Geometry2DEngineSettings
    {
        public int Precision { get; set; } = 4;
    }
}
