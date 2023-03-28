using Altium.Geometry2D.Shapes;
using System.Collections.Generic;

namespace Altium.Geometry2D
{
    public interface IGeometry2DEngine
    {
        int Precision { get; }
        double Epsilon { get; }

        #region Primitive shapes factory

        Point2D SnapToGrid(Point2D point);
        IInfiniteLine CreateInfiniteLine(Point2D point, Vector2D vector);
        IInfiniteLine CreateInfiniteLine(ILinearShape refShape);
        IRay CreateRay(Point2D point, Vector2D vector);
        IRay CreateRay(ILinearShape refShape);
        ILine CreateLine(Point2D startPoint, Point2D endPoint);
        ILine CreateLine(ILinearShape refShape);
        IArc CreateArc(Point2D startPoint, Point2D endPoint, Point2D pointOnArc);
        IArc CreateArc(Point2D startPoint, Point2D endPoint, double radius, bool cw = false, bool longest = false);
        IArc CreateArc(Point2D center, double radius, double startAngle, double sweepAngle);
        IArc CreateArc(ICircularShape refShape, double startAngle, double sweepAngle);
        ICircle CreateCircle(Point2D center, double radius);
        ICircle CreateCircle(ICircularShape refShape);
        IRectangle CreateRectangle(Point2D center, double width, double height, double rotation = 0);
        IRectangle CreateRectangle(Point2D vertex0, Point2D vertex2, double rotation = 0);
        IRectangle CreateRectangle(IRectShape refShape);
        IRoundedRectangle CreateRoundedRectangle(Point2D center, double width, double height, double rounding, ShapeRoundingMode roundingMode, double rotation = 0);
        IRoundedRectangle CreateRoundedRectangle(Point2D vertex0, Point2D vertex2, double rounding, ShapeRoundingMode roundingMode, double rotation = 0);
        IRoundedRectangle CreateRoundedRectangle(IRectShape refShape, double rounding, ShapeRoundingMode roundingMode);
        IBeveledRectangle CreateBeveledRectangle(Point2D center, double width, double height, double beveling, ShapeBevelingMode bevelingMode, double rotation = 0);
        IBeveledRectangle CreateBeveledRectangle(Point2D vertex0, Point2D vertex2, double beveling, ShapeBevelingMode bevelingMode, double rotation = 0);
        IBeveledRectangle CreateBeveledRectangle(IRectShape refShape, double beveling, ShapeBevelingMode bevelingMode);
        IPolygon CreatePolygon(params Point2D[] vertices);
        IPolygon CreatePolygon(IEnumerable<Point2D> vertices);
        IRoundedPolygon CreateRoundedPolygon(double rounding, ShapeRoundingMode roundingMode, params Point2D[] vertices);
        IRoundedPolygon CreateRoundedPolygon(double rounding, ShapeRoundingMode roundingMode, IEnumerable<Point2D> vertices);
        IBeveledPolygon CreateBeveledPolygon(double beveling, ShapeBevelingMode bevelingMode, params Point2D[] vertices);
        IBeveledPolygon CreateBeveledPolygon(double beveling, ShapeBevelingMode bevelingMode, IEnumerable<Point2D> vertices);
        ILineTrace CreateLineTrace(ILine guide, double width);
        IArcTrace CreateArcTrace(IArc guide, double width);

        #endregion Primitive shapes factory

        #region Coincident check

        bool AreCoincident(Point2D pt1, Point2D pt2);
        bool AreCoincident(Point2D pt, IInfiniteLine infLine);
        bool AreCoincident(Point2D pt, IRay ray);
        bool AreCoincident(Point2D pt, ILine line);
        bool AreCoincident(Point2D pt, ICircle circle);
        bool AreCoincident(Point2D pt, IArc arc);
        bool AreCoincident(Point2D pt, IGeometryPathSegment segment);
        bool AreCoincident(Point2D pt, IGeometryPath path);
        bool AreCoincident(Point2D pt, IRegion region);

        bool AreCoincident(IInfiniteLine infLine, Point2D pt);
        bool AreCoincident(IInfiniteLine infLine1, IInfiniteLine infLine2);
        bool AreCoincident(IInfiniteLine infLine, IRay ray);
        bool AreCoincident(IInfiniteLine infLine, ILine line);
        bool AreCoincident(IInfiniteLine infLine, ICircle circle);
        bool AreCoincident(IInfiniteLine infLine, IArc arc);
        bool AreCoincident(IInfiniteLine infLine, IGeometryPathSegment segment);
        bool AreCoincident(IInfiniteLine infLine, IGeometryPath path);
        bool AreCoincident(IInfiniteLine infLine, IRegion region);

        bool AreCoincident(IRay ray, Point2D pt);
        bool AreCoincident(IRay ray, IInfiniteLine infLine);
        bool AreCoincident(IRay ray1, IRay ray2);
        bool AreCoincident(IRay ray, ILine line);
        bool AreCoincident(IRay ray, ICircle circle);
        bool AreCoincident(IRay ray, IArc arc);
        bool AreCoincident(IRay ray, IGeometryPathSegment segment);
        bool AreCoincident(IRay ray, IGeometryPath path);
        bool AreCoincident(IRay ray, IRegion region);

        bool AreCoincident(ILine line, Point2D pt);
        bool AreCoincident(ILine line, IInfiniteLine infLine);
        bool AreCoincident(ILine line, IRay ray);
        bool AreCoincident(ILine line, ILine line2);
        bool AreCoincident(ILine line, ICircle circle);
        bool AreCoincident(ILine line, IArc arc);

        bool AreCoincident(ICircle circle, Point2D pt);
        bool AreCoincident(ICircle circle, IInfiniteLine infLine);
        bool AreCoincident(ICircle circle, IRay ray);
        bool AreCoincident(ICircle circle, ILine line);
        bool AreCoincident(ICircle circle1, ICircle circle2);
        bool AreCoincident(ICircle circle, IArc arc);
        bool AreCoincident(ICircle circle, IGeometryPathSegment segment);
        bool AreCoincident(ICircle circle, IGeometryPath path);
        bool AreCoincident(ICircle circle, IRegion region);

        bool AreCoincident(IArc arc, Point2D pt);
        bool AreCoincident(IArc arc, IInfiniteLine infLine);
        bool AreCoincident(IArc arc, IRay ray);
        bool AreCoincident(IArc arc, ILine line);
        bool AreCoincident(IArc arc, ICircle circle);
        bool AreCoincident(IArc arc1, IArc arc2);

        bool AreCoincident(IGeometryPathSegment segment, Point2D point);
        bool AreCoincident(IGeometryPathSegment segment, IInfiniteLine infLine);
        bool AreCoincident(IGeometryPathSegment segment, IRay ray);
        bool AreCoincident(IGeometryPathSegment segment1, IGeometryPathSegment segment2);
        bool AreCoincident(IGeometryPathSegment segment, IGeometryPath path);
        bool AreCoincident(IGeometryPathSegment segment, IRegion region);

        bool AreCoincident(IGeometryPath path, Point2D point);
        bool AreCoincident(IGeometryPath path, IInfiniteLine infLine);
        bool AreCoincident(IGeometryPath path, IRay ray);
        bool AreCoincident(IGeometryPath path, IGeometryPathSegment segment);
        bool AreCoincident(IGeometryPath path1, IGeometryPath path2);
        bool AreCoincident(IGeometryPath path, IRegion region);

        bool AreCoincident(IRegion region, Point2D point);
        bool AreCoincident(IRegion region, IInfiniteLine infLine);
        bool AreCoincident(IRegion region, IRay ray);
        bool AreCoincident(IRegion region, IGeometryPathSegment segment);
        bool AreCoincident(IRegion region, IGeometryPath path);
        bool AreCoincident(IRegion region1, IRegion region2);

        #endregion Coincident check

        #region Calculating shortest line between objects

        ILine ShortestLineBetween(Point2D pt1, Point2D pt2);
        ILine ShortestLineBetween(Point2D pt, IInfiniteLine infLine);
        ILine ShortestLineBetween(Point2D pt, IRay ray);
        ILine ShortestLineBetween(Point2D pt, ILine line);
        ILine ShortestLineBetween(Point2D pt, ICircle circle);
        ILine ShortestLineBetween(Point2D pt, IArc arc);
        ILine ShortestLineBetween(Point2D pt, IGeometryPathSegment segment);
        ILine ShortestLineBetween(Point2D pt, IGeometryPath path);
        ILine ShortestLineBetween(Point2D pt, IRegion region);

        ILine ShortestLineBetween(IInfiniteLine infLine, Point2D pt);
        ILine ShortestLineBetween(IInfiniteLine infLine1, IInfiniteLine infLine2);
        ILine ShortestLineBetween(IInfiniteLine infLine, IRay ray);
        ILine ShortestLineBetween(IInfiniteLine infLine, ILine line);
        ILine ShortestLineBetween(IInfiniteLine infLine, ICircle circle);
        ILine ShortestLineBetween(IInfiniteLine infLine, IArc arc);
        ILine ShortestLineBetween(IInfiniteLine infLine, IGeometryPathSegment segment);
        ILine ShortestLineBetween(IInfiniteLine infLine, IGeometryPath path);
        ILine ShortestLineBetween(IInfiniteLine infLine, IRegion region);

        ILine ShortestLineBetween(IRay ray, Point2D pt);
        ILine ShortestLineBetween(IRay ray, IInfiniteLine infLine);
        ILine ShortestLineBetween(IRay ray1, IRay ray2);
        ILine ShortestLineBetween(IRay ray, ILine line);
        ILine ShortestLineBetween(IRay ray, ICircle circle);
        ILine ShortestLineBetween(IRay ray, IArc arc);
        ILine ShortestLineBetween(IRay ray, IGeometryPathSegment segment);
        ILine ShortestLineBetween(IRay ray, IGeometryPath path);
        ILine ShortestLineBetween(IRay ray, IRegion region);

        ILine ShortestLineBetween(ILine line, Point2D pt);
        ILine ShortestLineBetween(ILine line, IInfiniteLine infLine);
        ILine ShortestLineBetween(ILine line, IRay ray);
        ILine ShortestLineBetween(ILine line1, ILine line2);
        ILine ShortestLineBetween(ILine line, ICircle circle);
        ILine ShortestLineBetween(ILine line, IArc arc);

        ILine ShortestLineBetween(ICircle circle, Point2D pt);
        ILine ShortestLineBetween(ICircle circle, IInfiniteLine infLine);
        ILine ShortestLineBetween(ICircle circle, IRay ray);
        ILine ShortestLineBetween(ICircle circle, ILine line);
        ILine ShortestLineBetween(ICircle circle1, ICircle circle2);
        ILine ShortestLineBetween(ICircle circle, IArc arc);

        ILine ShortestLineBetween(IArc arc, Point2D pt);
        ILine ShortestLineBetween(IArc arc, IInfiniteLine infLine);
        ILine ShortestLineBetween(IArc arc, IRay ray);
        ILine ShortestLineBetween(IArc arc, ILine line);
        ILine ShortestLineBetween(IArc arc, ICircle circle);
        ILine ShortestLineBetween(IArc arc1, IArc arc2);

        ILine ShortestLineBetween(IGeometryPathSegment segment, Point2D point);
        ILine ShortestLineBetween(IGeometryPathSegment segment, IInfiniteLine infLine);
        ILine ShortestLineBetween(IGeometryPathSegment segment, IRay ray);
        ILine ShortestLineBetween(IGeometryPathSegment segment1, IGeometryPathSegment segment2);

        ILine ShortestLineBetween(IGeometryPath path, Point2D point);
        ILine ShortestLineBetween(IGeometryPath path, IInfiniteLine infLine);
        ILine ShortestLineBetween(IGeometryPath path, IRay ray);
        ILine ShortestLineBetween(IGeometryPath path, IGeometryPathSegment segment);
        ILine ShortestLineBetween(IGeometryPath path1, IGeometryPath path2);

        ILine ShortestLineBetween(IRegion region, Point2D point);
        ILine ShortestLineBetween(IRegion region, IInfiniteLine infLine);
        ILine ShortestLineBetween(IRegion region, IRay ray);
        ILine ShortestLineBetween(IRegion region, IGeometryPathSegment segment);
        ILine ShortestLineBetween(IRegion region, IGeometryPath path);
        ILine ShortestLineBetween(IRegion region1, IRegion region2);

        #endregion Calculating shortest line between objects

        #region Calculating all intersections between objects

        IEnumerable<Point2D> Intersections(Point2D pt1, Point2D pt2);
        IEnumerable<Point2D> Intersections(Point2D pt, IInfiniteLine infLine);
        IEnumerable<Point2D> Intersections(Point2D pt, IRay ray);
        IEnumerable<Point2D> Intersections(Point2D pt, ILine line);
        IEnumerable<Point2D> Intersections(Point2D pt, ICircle circle);
        IEnumerable<Point2D> Intersections(Point2D pt, IArc arc);
        IEnumerable<Point2D> Intersections(Point2D pt, IGeometryPathSegment segment);
        IEnumerable<Point2D> Intersections(Point2D pt, IGeometryPath path);
        IEnumerable<Point2D> Intersections(Point2D pt, IRegion region);

        IEnumerable<Point2D> Intersections(IInfiniteLine infLine, Point2D pt);
        IEnumerable<Point2D> Intersections(IInfiniteLine infLine1, IInfiniteLine infLine2);
        IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IRay ray);
        IEnumerable<Point2D> Intersections(IInfiniteLine infLine, ILine line);
        IEnumerable<Point2D> Intersections(IInfiniteLine infLine, ICircle circle);
        IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IArc arc);
        IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IGeometryPathSegment segment);
        IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IGeometryPath path);
        IEnumerable<Point2D> Intersections(IInfiniteLine infLine, IRegion region);

        IEnumerable<Point2D> Intersections(IRay ray, Point2D pt);
        IEnumerable<Point2D> Intersections(IRay ray, IInfiniteLine infLine);
        IEnumerable<Point2D> Intersections(IRay ray1, IRay ray2);
        IEnumerable<Point2D> Intersections(IRay ray, ILine line);
        IEnumerable<Point2D> Intersections(IRay ray, ICircle circle);
        IEnumerable<Point2D> Intersections(IRay ray, IArc arc);
        IEnumerable<Point2D> Intersections(IRay ray, IGeometryPathSegment segment);
        IEnumerable<Point2D> Intersections(IRay ray, IGeometryPath path);
        IEnumerable<Point2D> Intersections(IRay ray, IRegion region);

        IEnumerable<Point2D> Intersections(ILine line, Point2D pt);
        IEnumerable<Point2D> Intersections(ILine line, IInfiniteLine infLine);
        IEnumerable<Point2D> Intersections(ILine line, IRay ray);
        IEnumerable<Point2D> Intersections(ILine line1, ILine line2);
        IEnumerable<Point2D> Intersections(ILine line, ICircle circle);
        IEnumerable<Point2D> Intersections(ILine line, IArc arc);

        IEnumerable<Point2D> Intersections(ICircle circle, Point2D pt);
        IEnumerable<Point2D> Intersections(ICircle circle, IInfiniteLine infLine);
        IEnumerable<Point2D> Intersections(ICircle circle, IRay ray);
        IEnumerable<Point2D> Intersections(ICircle circle, ILine line);
        IEnumerable<Point2D> Intersections(ICircle circle1, ICircle circle2);
        IEnumerable<Point2D> Intersections(ICircle circle, IArc arc);

        IEnumerable<Point2D> Intersections(IArc arc, Point2D pt);
        IEnumerable<Point2D> Intersections(IArc arc, IInfiniteLine infLine);
        IEnumerable<Point2D> Intersections(IArc arc, IRay ray);
        IEnumerable<Point2D> Intersections(IArc arc, ILine line);
        IEnumerable<Point2D> Intersections(IArc arc, ICircle circle);
        IEnumerable<Point2D> Intersections(IArc arc1, IArc arc2);

        IEnumerable<Point2D> Intersections(IGeometryPathSegment segment, Point2D point);
        IEnumerable<Point2D> Intersections(IGeometryPathSegment segment, IInfiniteLine infLine);
        IEnumerable<Point2D> Intersections(IGeometryPathSegment segment, IRay ray);
        IEnumerable<Point2D> Intersections(IGeometryPathSegment segment1, IGeometryPathSegment segment2);

        IEnumerable<Point2D> Intersections(IGeometryPath path, Point2D point);
        IEnumerable<Point2D> Intersections(IGeometryPath path, IInfiniteLine infLine);
        IEnumerable<Point2D> Intersections(IGeometryPath path, IRay ray);
        IEnumerable<Point2D> Intersections(IGeometryPath path, IGeometryPathSegment segment);
        IEnumerable<Point2D> Intersections(IGeometryPath path1, IGeometryPath path2);

        IEnumerable<Point2D> Intersections(IRegion region, Point2D point);
        IEnumerable<Point2D> Intersections(IRegion region, IInfiniteLine infLine);
        IEnumerable<Point2D> Intersections(IRegion region, IRay ray);
        IEnumerable<Point2D> Intersections(IRegion region, IGeometryPathSegment segment);
        IEnumerable<Point2D> Intersections(IRegion region, IGeometryPath path);
        IEnumerable<Point2D> Intersections(IRegion region1, IRegion region2);

        #endregion Calculating all intersections between objects

        #region Calculating first intersection point between objects

        Point2D? FirstIntersection(Point2D pt1, Point2D pt2);
        Point2D? FirstIntersection(Point2D pt, IInfiniteLine infLine);
        Point2D? FirstIntersection(Point2D pt, IRay ray);
        Point2D? FirstIntersection(Point2D pt, ILine line);
        Point2D? FirstIntersection(Point2D pt, ICircle circle);
        Point2D? FirstIntersection(Point2D pt, IArc arc);
        Point2D? FirstIntersection(Point2D pt, IGeometryPathSegment segment);
        Point2D? FirstIntersection(Point2D pt, IGeometryPath path);
        Point2D? FirstIntersection(Point2D pt, IRegion region);

        Point2D? FirstIntersection(IInfiniteLine infLine, Point2D pt);
        Point2D? FirstIntersection(IInfiniteLine infLine1, IInfiniteLine infLine2);
        Point2D? FirstIntersection(IInfiniteLine infLine, IRay ray);
        Point2D? FirstIntersection(IInfiniteLine infLine, ILine line);
        Point2D? FirstIntersection(IInfiniteLine infLine, ICircle circle);
        Point2D? FirstIntersection(IInfiniteLine infLine, IArc arc);
        Point2D? FirstIntersection(IInfiniteLine infLine, IGeometryPathSegment segment);
        Point2D? FirstIntersection(IInfiniteLine infLine, IGeometryPath path);
        Point2D? FirstIntersection(IInfiniteLine infLine, IRegion region);

        Point2D? FirstIntersection(IRay ray, Point2D pt);
        Point2D? FirstIntersection(IRay ray, IInfiniteLine infLine2);
        Point2D? FirstIntersection(IRay ray1, IRay ray2);
        Point2D? FirstIntersection(IRay ray, ILine line);
        Point2D? FirstIntersection(IRay ray, ICircle circle);
        Point2D? FirstIntersection(IRay ray, IArc arc);
        Point2D? FirstIntersection(IRay ray, IGeometryPathSegment segment);
        Point2D? FirstIntersection(IRay ray, IGeometryPath path);
        Point2D? FirstIntersection(IRay ray, IRegion region);

        Point2D? FirstIntersection(ILine line, Point2D pt);
        Point2D? FirstIntersection(ILine line, IInfiniteLine infLine);
        Point2D? FirstIntersection(ILine line, IRay ray);
        Point2D? FirstIntersection(ILine line1, ILine line2);
        Point2D? FirstIntersection(ILine line, ICircle circle);
        Point2D? FirstIntersection(ILine line, IArc arc);

        Point2D? FirstIntersection(ICircle circle, Point2D pt);
        Point2D? FirstIntersection(ICircle circle, IInfiniteLine infLine);
        Point2D? FirstIntersection(ICircle circle, IRay ray);
        Point2D? FirstIntersection(ICircle circle, ILine line);
        Point2D? FirstIntersection(ICircle circle1, ICircle circle2);
        Point2D? FirstIntersection(ICircle circle, IArc arc);

        Point2D? FirstIntersection(IArc arc, Point2D pt);
        Point2D? FirstIntersection(IArc arc, IInfiniteLine infLine);
        Point2D? FirstIntersection(IArc arc, IRay ray);
        Point2D? FirstIntersection(IArc arc, ILine line);
        Point2D? FirstIntersection(IArc arc, ICircle circle);
        Point2D? FirstIntersection(IArc arc1, IArc arc2);

        Point2D? FirstIntersection(IGeometryPathSegment segment, Point2D point);
        Point2D? FirstIntersection(IGeometryPathSegment segment, IInfiniteLine infLine);
        Point2D? FirstIntersection(IGeometryPathSegment segment, IRay ray);
        Point2D? FirstIntersection(IGeometryPathSegment segment1, IGeometryPathSegment segment2);

        Point2D? FirstIntersection(IGeometryPath path, Point2D point);
        Point2D? FirstIntersection(IGeometryPath path, IInfiniteLine infLine);
        Point2D? FirstIntersection(IGeometryPath path, IRay ray);
        Point2D? FirstIntersection(IGeometryPath path, IGeometryPathSegment segment);
        Point2D? FirstIntersection(IGeometryPath path1, IGeometryPath path2);

        Point2D? FirstIntersection(IRegion region, Point2D point);
        Point2D? FirstIntersection(IRegion region, IInfiniteLine infLine);
        Point2D? FirstIntersection(IRegion region, IRay ray);
        Point2D? FirstIntersection(IRegion region, IGeometryPathSegment segment);
        Point2D? FirstIntersection(IRegion region, IGeometryPath path);
        Point2D? FirstIntersection(IRegion region1, IRegion region2);

        #endregion Calculating first intersection point between objects

        #region Paths and regions

        IGeometryPathBuilder CreatePathBuilder();
        IRegionBuilder CreateRegionBuilder();
        IRegionComposer CreateRegionComposer();
        IGeometryPath Reduce(IGeometryPath path);
        IRegion Reduce(IRegion region);
        IEnumerable<IGeometryPath> FindCommonPaths(IGeometryPath path1, IGeometryPath path2);
        IEnumerable<IGeometryPath> FindCommonPaths(IRegion region1, IRegion region2);
        IRectangle FindMinimalRectangleAroundPath(IGeometryPath path, Vector2D? direction = null);
        IPolygon FindMinimalPoly4AroundPath(IGeometryPath path);
        IGeometryPath Offset(IGeometryPath path, double offset, OffsetMode mode);
        double ComputeMaxDiff(IGeometryPath path1, IGeometryPath path2);
        IGeometryPath SnapPathToRefPath(IGeometryPath pathToSnap, IGeometryPath refPath, double distanceThreshold);

        #endregion Paths and regions
    }
}
