using System.Collections.Generic;
using System.Linq;

namespace Altium.Geometry2D
{
    public static class Extensions
    {
        public static IGeometryPath CreatePath(this IGeometry2DEngine geometry2DEngine, bool autoClosePaths, params Point2D[] points)
        {
            return CreatePath(geometry2DEngine, autoClosePaths, (IEnumerable<Point2D>)points);
        }

        public static IGeometryPath CreatePath(this IGeometry2DEngine geometry2DEngine, bool autoClosePaths, IEnumerable<Point2D> points)
        {
            var pathBuilder = geometry2DEngine.CreatePathBuilder();
            pathBuilder.BeginPath();
            pathBuilder.Add(points);
            return pathBuilder.EndPath(autoClosePaths);
        }

        public static IGeometryPath CreatePath(this IGeometry2DEngine geometry2DEngine, bool autoClosePaths, params IGeometryPathSegment[] segments)
        {
            return CreatePath(geometry2DEngine, autoClosePaths, (IEnumerable<IGeometryPathSegment>)segments);
        }

        public static IGeometryPath CreatePath(this IGeometry2DEngine geometry2DEngine, bool autoClosePaths, IEnumerable<IGeometryPathSegment> segments)
        {
            var pathBuilder = geometry2DEngine.CreatePathBuilder();
            pathBuilder.BeginPath();
            pathBuilder.Add(segments);
            return pathBuilder.EndPath(autoClosePaths);
        }

        public static IRegion CreateRegion(this IGeometry2DEngine geometry2DEngine, bool autoClosePaths, params Point2D[] points)
        {
            return CreateRegion(geometry2DEngine, autoClosePaths, (IEnumerable<Point2D>)points);
        }

        public static IRegion CreateRegion(this IGeometry2DEngine geometry2DEngine, bool autoClosePaths, IEnumerable<Point2D> points)
        {
            var regionBuilder = geometry2DEngine.CreateRegionBuilder();
            regionBuilder.BeginRegion();
            regionBuilder.Add(CreatePath(geometry2DEngine, autoClosePaths, points));
            return regionBuilder.EndRegion();
        }

        public static IRegion CreateRegion(this IGeometry2DEngine geometry2DEngine, bool autoClosePaths, params IGeometryPathSegment[] segments)
        {
            return CreateRegion(geometry2DEngine, autoClosePaths, (IEnumerable<IGeometryPathSegment>)segments);
        }

        public static IRegion CreateRegion(this IGeometry2DEngine geometry2DEngine, bool autoClosePaths, IEnumerable<IGeometryPathSegment> segments)
        {
            var regionBuilder = geometry2DEngine.CreateRegionBuilder();
            regionBuilder.BeginRegion();
            regionBuilder.Add(CreatePath(geometry2DEngine, autoClosePaths, segments));
            return regionBuilder.EndRegion();
        }

        public static IRegion CreateRegion(this IGeometry2DEngine geometry2DEngine, params IGeometryPath[] paths)
        {
            return CreateRegion(geometry2DEngine, (IEnumerable<IGeometryPath>)paths);
        }

        public static IRegion CreateRegion(this IGeometry2DEngine geometry2DEngine, IEnumerable<IGeometryPath> paths)
        {
            var regionBuilder = geometry2DEngine.CreateRegionBuilder();
            regionBuilder.BeginRegion();
            regionBuilder.Add(paths);
            return regionBuilder.EndRegion();
        }

        public static IRegion CreateSliceRegion(this IGeometry2DEngine geometry2DEngine, params IGeometryPath[] paths)
        {
            return CreateSliceRegion(geometry2DEngine, (IEnumerable<IGeometryPath>)paths);
        }

        public static IRegion CreateSliceRegion(this IGeometry2DEngine geometry2DEngine, IEnumerable<IGeometryPath> paths)
        {
            var regionBuilder = geometry2DEngine.CreateRegionBuilder();
            var pathBuilder = geometry2DEngine.CreatePathBuilder();
            regionBuilder.BeginRegion();
            foreach (var path in paths)
            {
                if (path.IsClosed)
                {
                    // create two opened paths
                    var longestSegment = path.Segments.OrderBy(x => x.Length()).First();
                    var longestSegmentSplit = longestSegment.Split(longestSegment.Midpoint());
                    pathBuilder.BeginPath(false, false);
                    pathBuilder.Add(path.Segments.TakeWhile(x => x != longestSegment));
                    pathBuilder.Add(longestSegmentSplit.PrevSegment);
                    var path1 = pathBuilder.EndPath(false);
                    regionBuilder.Add(path1);
                    pathBuilder.BeginPath(false, false);
                    pathBuilder.Add(longestSegmentSplit.NextSegment);
                    pathBuilder.Add(path.Segments.SkipWhile(x => x != longestSegment).Skip(1));
                    var path2 = pathBuilder.EndPath(false);
                    regionBuilder.Add(path2);
                    regionBuilder.Add(path1.Reverse());
                    regionBuilder.Add(path2.Reverse());
                }
                else
                {
                    regionBuilder.Add(path);
                    regionBuilder.Add(path.Reverse());
                }
            }

            return regionBuilder.EndRegion();
        }

        public static ITransform Then(this ITransform parentTransform, ITransform transform) => new TransformChain(parentTransform, transform);
    }
}
