using Altium.Geometry2D.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Altium.Geometry2D
{
    internal class RegionComposer : IRegionComposer
    {
        public RegionComposer(IGeometry2DEngine geometry2DEngine)
        {
            this.geometry2DEngine = geometry2DEngine;
            vertexNodeMap = new SortedDictionary<Point2D, VertexNode>(Point2DComparerYX.Default);
            SetBoundingRect(AARectD.EmptyAABB);
        }

        private readonly IGeometry2DEngine geometry2DEngine;
        private readonly List<IGeometryPath> paths = new List<IGeometryPath>();
        private readonly SortedDictionary<Point2D, VertexNode> vertexNodeMap;
        private readonly Dictionary<IGeometryPathSegment, SegmentNode> segmentNodeMap = new Dictionary<IGeometryPathSegment, SegmentNode>();
        private List<AreaInfo> areas;
        private AARectD boundingRect;
        private Point2D areaSize;
        private Point2D areasOrigin;
        private int areasCountX;
        private int areasCountY;

        public void Reset()
        {
            paths.Clear();
            vertexNodeMap.Clear();
            segmentNodeMap.Clear();
            areas = null;
            SetBoundingRect(AARectD.EmptyAABB);
        }

        public void Add(IRegion region)
        {
            var newBoundingRect = boundingRect.Union(region.BoundingRect);
            if (boundingRect != newBoundingRect)
                SetBoundingRect(newBoundingRect);

            foreach (var path in region.Paths)
                Add(path);
        }

        public void Add(params IRegion[] regions) => Add((IEnumerable<IRegion>)regions);
        public void Add(IEnumerable<IRegion> regions)
        {
            foreach (var region in regions)
                Add(region);
        }

        private void Add(IGeometryPath path)
        {
            var newBoundingRect = boundingRect.Union(path.BoundingRect);
            if (boundingRect != newBoundingRect)
                SetBoundingRect(newBoundingRect);

            // add path
            paths.Add(path);
            foreach (var areaInfo in GetAreaInfosAtRect(path.BoundingRect))
                areaInfo.OriginalPaths.Add(path);

            // add segments
            foreach (var originalSegment in path.Segments)
            {
                var segmentNode = new SegmentNode(originalSegment);
                segmentNode.Add(new SegmentSplit(0, originalSegment));
                segmentNodeMap.Add(segmentNode.OriginalSegment, segmentNode);
                foreach (var areaInfo in GetAreaInfosAtRect(originalSegment.BoundingRect))
                    areaInfo.OriginalSegments.Add(originalSegment);
            }

            // add vertices
            var pathVertexIterator = new GeometryPathVertexIterator(path);
            foreach (var pathVertex in pathVertexIterator.Vertices)
            {
                if (!vertexNodeMap.TryGetValue(pathVertex.Point, out var vertexNode))
                {
                    vertexNode = new VertexNode(pathVertex.Point);
                    vertexNodeMap.Add(vertexNode.Point, vertexNode);
                }

                vertexNode.Vertices.Add(new PathVertexEx(path, pathVertex));
            }

            // split intersections with new segments
            foreach (var newSegment in path.Segments)
            {
                SplitIntersections(newSegment);
            }
        }

        public IRegion Compose(RegionCompositionMode mode)
        {
            var pathBuilder = geometry2DEngine.CreatePathBuilder();
            ClearAllFlags();
            var outlines = new LinkedList<IGeometryPath>();
            var voids = new LinkedList<IGeometryPath>();
            foreach (var startVertexNode in vertexNodeMap.Values)
            {
                foreach (var startVertex in startVertexNode.Vertices)
                {
                    // skip unclosed paths
                    if (startVertex.NextSegment == null) continue;
                    if (!FilterWindingByMode(startVertexNode, startVertex, mode)) continue;

                    var newPathVertices = new LinkedList<PathVertexEx>();
                    var newPathPoints = new HashSet<Point2D>();
                    bool newPathIsClosed = false;
                    var prevVertexNode = startVertexNode;
                    var prevSegment = startVertex.PrevSegment;
                    if (prevSegment == null)
                        prevSegment = new Line(startVertex.Point - startVertex.NextSegment.Tangent(startVertex.Point), startVertex.Point);
                    int prevPathWinding = startVertex.Path.Winding;
                    while (!newPathIsClosed)
                    {
                        var vertexInfo = SelectVertex(prevVertexNode, prevSegment, prevPathWinding, mode);
                        if (vertexInfo.NextVertexNode == null) break;

                        var vertex = vertexInfo.PathVertex;
                        newPathVertices.AddLast(vertex);
                        vertex.Mark1 = true;
                        newPathPoints.Add(vertex.NextSegment.StartPoint);
                        newPathIsClosed = newPathPoints.Contains(vertex.NextSegment.EndPoint);

                        prevVertexNode = vertexInfo.NextVertexNode;
                        prevSegment = vertex.NextSegment;
                        prevPathWinding = vertex.Path.Winding;
                    }

                    if (newPathIsClosed)
                    {
                        var endPoint = newPathVertices.Last.Value.NextSegment.EndPoint;
                        while (newPathVertices.First.Value.NextSegment.StartPoint != endPoint)
                        {
                            newPathVertices.First.Value.Mark1 = false;
                            newPathVertices.RemoveFirst();
                        }

                        pathBuilder.BeginPath();
                        pathBuilder.Add(newPathVertices.Select(x => x.NextSegment));
                        var newPath = pathBuilder.EndPath(true);
                        if (newPath.Winding > 0 && !outlines.Any(x => geometry2DEngine.AreCoincident(x, newPath)))
                            outlines.AddLast(newPath);
                        if (newPath.Winding < 0 && !voids.Any(x => geometry2DEngine.AreCoincident(x, newPath)))
                            voids.AddLast(newPath);
                    }
                    else
                    {
                        foreach (var newPathVertex in newPathVertices)
                            newPathVertex.Mark1 = false;
                    }
                }
            }

            var resultOutlines = (IEnumerable<IGeometryPath>)outlines.Where(o => !voids.Any(v => geometry2DEngine.AreCoincident(o, v))).ToList();
            var resultVoids = (IEnumerable<IGeometryPath>)voids;
            if (mode == RegionCompositionMode.AndAny)
                resultOutlines = resultOutlines.Where(o => paths.Where(p => p.Winding > 0 && p.IsInnerPoint(o.GetInnerPoint().Value, false)).Count() > 1).ToList();
            if (mode != RegionCompositionMode.Normalize)
                resultVoids = resultVoids.Where(v => resultOutlines.Any(o => o.IsInnerPoint(v.GetInnerPoint().Value, false))).ToList();

            var regionBuilder = geometry2DEngine.CreateRegionBuilder();
            regionBuilder.BeginRegion();
            regionBuilder.Add(resultOutlines);
            regionBuilder.Add(resultVoids);
            return regionBuilder.EndRegion();
        }

        private void ClearAllFlags()
        {
            foreach (var vertexNode in vertexNodeMap.Values)
            {
                foreach (var vertex in vertexNode.Vertices)
                    vertex.ClearAllFlags();
            }
        }

        private struct NextVertexInfo
        {
            public PathVertexEx PathVertex;
            public int Winding;
            public VertexNode NextVertexNode;
        }

        private NextVertexInfo SelectVertex(VertexNode vertexNode, IGeometryPathSegment prevSegment, int prevPathWinding, RegionCompositionMode mode)
        {
            var nextVertexNodes = vertexNode.Vertices
                .Where(x => !x.Mark1 && x.NextSegment != null)
                .Select(x => new NextVertexInfo
                {
                    PathVertex = x,
                    NextVertexNode = vertexNodeMap[x.NextSegment.EndPoint]
                })
                .Where(x => (x.PathVertex.NextSegment != prevSegment || x.PathVertex.NextSegment == x.PathVertex.PrevSegment) && FilterWindingByMode(vertexNode, x.PathVertex, mode))
                .Where(x => x.PathVertex.Path.Winding != 0 || x.PathVertex.NextSegment == prevSegment || !geometry2DEngine.AreCoincident(x.PathVertex.NextSegment, prevSegment))
                .ToList();

            if (nextVertexNodes.Count < 2)
                return nextVertexNodes.FirstOrDefault();

            var prevTangent = prevSegment.Tangent(vertexNode.Point).Unit();
            var nextVertexNode = nextVertexNodes.Select(x => new
            {
                x.PathVertex,
                x.Winding,
                x.NextVertexNode,
                Tangent = x.PathVertex.NextSegment.Tangent(vertexNode.Point).Unit()
            }).Select(x => new
            {
                x.PathVertex,
                x.Winding,
                x.NextVertexNode,
                x.Tangent,
                RelDir = (x.Tangent ^ prevTangent) >= 0 ? (1.0 + x.Tangent * prevTangent) : (3.0 - x.Tangent * prevTangent)
            })
            .OrderBy(x => x.PathVertex.Path.Winding != prevPathWinding ? 0 : 1)
            .ThenBy(x => x.PathVertex.Path.Winding == 0 ? 0 : 1)
            .ThenBy(x => x.PathVertex.Path.Winding > 0 && mode != RegionCompositionMode.AndAny ? x.RelDir : -x.RelDir)
            .ThenBy(x => x.PathVertex.Path.Winding > 0 && mode != RegionCompositionMode.AndAny ? x.PathVertex.NextSegment.Curvature(vertexNode.Point) : -x.PathVertex.NextSegment.Curvature(vertexNode.Point))
            .First();

            return new NextVertexInfo
            {
                PathVertex = nextVertexNode.PathVertex,
                Winding = nextVertexNode.Winding,
                NextVertexNode = nextVertexNode.NextVertexNode
            };
        }

        private double GetRelAngle(double targetAngle, double refAngle)
        {
            var result = targetAngle - refAngle;
            if (result < 0) result += Math2D.DPI;
            return result;
        }

        private int GetMultiplicity(VertexNode vertexNode, PathVertexEx pathVertex)
        {
            return vertexNode.Vertices.Where(x => x.Path.Winding > 0).Count();
            //var nextSegment = pathVertex.NextSegment;
            //return vertexNode.Vertices
            //    .Where(x => x.NextSegment != null && x.NextSegment.EndPoint == nextSegment.EndPoint &&
            //        geometry2DEngine.AreCoincident(x.NextSegment.Midpoint(), nextSegment.Midpoint()))
            //    .Count();
        }

        private bool FilterWindingByMode(VertexNode vertexNode, PathVertexEx pathVertex, RegionCompositionMode mode)
        {
            var pathWinding = pathVertex.Path.Winding;
            var winding = pathVertex.GetWinding(this);
            var multiplicity = GetMultiplicity(vertexNode, pathVertex);
            switch (mode)
            {
                case RegionCompositionMode.Or:
                    int minWinding = int.MaxValue;
                    int maxWinding = int.MinValue;
                    var acceptableVertices = vertexNode.Vertices.Where(x => x.NextSegment != null && x.GetWinding(this) >= 0);
                    foreach (var acceptableVertex in acceptableVertices)
                    {
                        var w = acceptableVertex.GetWinding(this);
                        if (minWinding > w) minWinding = w;
                        if (maxWinding < w) maxWinding = w;
                    }

                    return (pathWinding == 0 && (winding != 0 || multiplicity > 0)) ||
                        (pathWinding > 0 && winding == minWinding) ||
                        (pathWinding < 0 && winding == maxWinding);
                case RegionCompositionMode.AndAny:
                    return pathWinding != 0 && ((winding == 0 && multiplicity > 1) || winding > 0);
                case RegionCompositionMode.Normalize:
                    return pathWinding == 0 ||
                        (pathWinding > 0 && winding == 0) ||
                        (pathWinding < 0 && winding >= 0);
            }

            return false;
        }

        private void SplitIntersections(IGeometryPathSegment newSegment)
        {
            foreach (var originalSegment in FindOriginalSegmentsAtRect(newSegment.BoundingRect))
            {
                if (originalSegment == newSegment) continue;

                var intersectionPoints = geometry2DEngine.Intersections(originalSegment, newSegment);
                foreach (var intersectionPoint in intersectionPoints)
                {
                    // snap split point to grid
                    var splitPoint = geometry2DEngine.SnapToGrid(intersectionPoint);
                    // split existed segment
                    var existedSplitted = SplitSegment(originalSegment, splitPoint);
                    // split new segment
                    var newSplitted = SplitSegment(newSegment, splitPoint);
                }
            }
        }

        private bool SplitSegment(IGeometryPathSegment segment, Point2D splitPoint)
        {
            // if split point is coincident with any segment ends return false
            if (segment.StartPoint == splitPoint || segment.EndPoint == splitPoint) return false;

            var segmentNode = segmentNodeMap[segment];
            var distance = segmentNode.OriginalSegment.LengthFromStart(splitPoint);
            var refPoint = new SegmentSplit() { Distance = distance };
            var i = segmentNode.BinarySearch(refPoint, SegmentSplitComparer.Default);
            if (i < 0)
            {
                i = (~i) - 1;
                var segmentSplit = segmentNode[i];
                var segmentToSplit = segmentSplit.Segment;
                var segmentToSplitDistance = segmentSplit.Distance;
                var prevVertexNode = vertexNodeMap[segmentToSplit.StartPoint];
                var prevVertexIndex = prevVertexNode.Vertices.FindIndex(x => x.NextSegment == segmentToSplit);
                var prevVertex = prevVertexNode.Vertices[prevVertexIndex];
                var nextVertex = segmentSplit.Segment.Split(splitPoint);
                segmentNode.Insert(i, new SegmentSplit(segmentSplit.Distance, nextVertex.PrevSegment));
                segmentNode[i + 1] = new SegmentSplit(distance, nextVertex.NextSegment);
                prevVertex.NextSegment = nextVertex.PrevSegment;

                // add vertex
                vertexNodeMap.TryGetValue(splitPoint, out var vertexNode);
                if (vertexNode == null)
                {
                    vertexNode = new VertexNode(splitPoint);
                    vertexNodeMap.Add(vertexNode.Point, vertexNode);
                }

                vertexNode.Vertices.Add(new PathVertexEx(prevVertex.Path, nextVertex));
                return true;
            }
            else
            {
                // segment already splitted
                return false;
            }
        }

        private void SetBoundingRect(AARectD newBoundingRect)
        {
            if (double.IsInfinity(newBoundingRect.LB.X) ||
                double.IsInfinity(newBoundingRect.LB.Y) ||
                double.IsInfinity(newBoundingRect.RT.X) ||
                double.IsInfinity(newBoundingRect.RT.Y))
            {
                boundingRect = newBoundingRect;
                areas = new List<AreaInfo>();
                areaSize = Point2D.Zero;
                areasOrigin = Point2D.Zero;
                areasCountX = 0;
                areasCountY = 0;
                return;
            }

            var newAreasCountX = Math.Max(10, (int)(segmentNodeMap.Count / 10000));
            var newAreasCountY = newAreasCountX;
            var newAreasOrigin = newBoundingRect.LB;
            var newAreaWidth = newBoundingRect.Width / newAreasCountX * 1.01;
            var newAreaHeight = newBoundingRect.Height / newAreasCountY * 1.01;
            var newAreaSize = new Point2D(newAreaWidth, newAreaHeight);
            var newAreas = new List<AreaInfo>(newAreasCountX * newAreasCountY);
            for (int newAreaYIndex = 0; newAreaYIndex < newAreasCountY; newAreaYIndex++)
            {
                for (int newAreaXIndex = 0; newAreaXIndex < newAreasCountX; newAreaXIndex++)
                {
                    var newAreaRect = new AARectD(
                        newAreasOrigin + new Vector2D(newAreaXIndex * newAreaWidth, newAreaYIndex * newAreaHeight),
                        newAreasOrigin + new Vector2D((newAreaXIndex + 1) * newAreaWidth, (newAreaYIndex + 1) * newAreaHeight));
                    var newAreaInfo = new AreaInfo(newAreaRect);
                    newAreas.Add(newAreaInfo);

                    // fill area info
                    foreach (var originalSegment in FindOriginalSegmentsAtRect(newAreaRect))
                    {
                        if (newAreaRect.Intersect(originalSegment.BoundingRect))
                            newAreaInfo.OriginalSegments.Add(originalSegment);
                    }

                    foreach (var originalPath in FindOriginalPathsAtRect(newAreaRect))
                    {
                        if (newAreaRect.Intersect(originalPath.BoundingRect))
                            newAreaInfo.OriginalPaths.Add(originalPath);
                    }
                }
            }

            // replace old areas with new areas
            boundingRect = newBoundingRect;
            areas = newAreas;
            areaSize = newAreaSize;
            areasOrigin = newAreasOrigin;
            areasCountX = newAreasCountX;
            areasCountY = newAreasCountY;
        }

        private IEnumerable<IGeometryPathSegment> FindOriginalSegmentsAtRect(AARectD rect)
        {
            var processedOriginalSegments = new HashSet<IGeometryPathSegment>();
            foreach (var areaInfo in GetAreaInfosAtRect(rect))
            {
                foreach (var originalSegment in areaInfo.OriginalSegments)
                {
                    if (processedOriginalSegments.Contains(originalSegment)) continue;
                    yield return originalSegment;
                    processedOriginalSegments.Add(originalSegment);
                }
            }
        }

        private IEnumerable<IGeometryPath> FindOriginalPathsAtRect(AARectD rect)
        {
            var processedOriginalPaths = new HashSet<IGeometryPath>();
            foreach (var areaInfo in GetAreaInfosAtRect(rect))
            {
                foreach (var originalPath in areaInfo.OriginalPaths)
                {
                    if (processedOriginalPaths.Contains(originalPath)) continue;
                    yield return originalPath;
                    processedOriginalPaths.Add(originalPath);
                }
            }
        }

        private IEnumerable<AreaInfo> GetAreaInfosAtRect(AARectD rect)
        {
            var areaIndices = GetAreaIndicesAtRect(rect);
            for (int areaYIndex = areaIndices.LB.Y; areaYIndex <= areaIndices.RT.Y; areaYIndex++)
            {
                for (int areaXIndex = areaIndices.LB.X; areaXIndex <= areaIndices.RT.X; areaXIndex++)
                {
                    var areaInfo = areas[areaYIndex * areasCountX + areaXIndex];
                    yield return areaInfo;
                }
            }
        }

        private AARectI GetAreaIndicesAtRect(AARectD rect)
        {
            if (areasCountX == 0 || areasCountY == 0)
                return new AARectI() { LB = new Point2I(0, 0), RT = new Point2I(-1, -1) };

            int minX = ((int)Math.Floor((rect.LB.X - areasOrigin.X) / areaSize.X)).LimitRange(0, areasCountX - 1);
            int maxX = ((int)Math.Floor((rect.RT.X - areasOrigin.X) / areaSize.X)).LimitRange(0, areasCountX - 1);
            int minY = ((int)Math.Floor((rect.LB.Y - areasOrigin.Y) / areaSize.Y)).LimitRange(0, areasCountY - 1);
            int maxY = ((int)Math.Floor((rect.RT.Y - areasOrigin.Y) / areaSize.Y)).LimitRange(0, areasCountY - 1);
            return new AARectI(minX, minY, maxX, maxY);
        }

        private class PathVertexEx
        {
            public static readonly int Mark1Bit = BitVector32.CreateMask();
            public static readonly int Mark2Bit = BitVector32.CreateMask(Mark1Bit);

            public PathVertexEx(IGeometryPath path, GeometryPathVertex pathVertex)
            {
                Path = path;
                PrevSegment = pathVertex.PrevSegment;
                NextSegment = pathVertex.NextSegment;
            }

            public IGeometryPath Path { get; }
            public IGeometryPathSegment PrevSegment { get; set; }
            public IGeometryPathSegment NextSegment { get; set; }
            private BitVector32 flags;

            public Point2D Point => PrevSegment != null ? PrevSegment.EndPoint : NextSegment != null ? NextSegment.StartPoint : default(Point2D);

            public void ClearAllFlags()
            {
                flags = default(BitVector32);
            }

            public bool Mark1
            {
                get => flags[Mark1Bit];
                set => flags[Mark1Bit] = value;
            }

            public bool Mark2
            {
                get => flags[Mark2Bit];
                set => flags[Mark2Bit] = value;
            }

            private int? winding;
            public int GetWinding(RegionComposer composer)
            {
                if (!winding.HasValue)
                    winding = CalcWinding(composer);
                return winding.Value;
            }

            public void InvalidateWinding()
            {
                winding = null;
            }

            protected int CalcWinding(RegionComposer composer)
            {
                int result = 0; // Path.Winding;
                var nextSegmentMidpoint = NextSegment.Midpoint();
                foreach (var originalPath in composer.FindOriginalPathsAtRect(new AARectD(nextSegmentMidpoint, nextSegmentMidpoint)))
                {
                    if (originalPath == Path || !originalPath.BoundingRect.Contains(nextSegmentMidpoint)) continue;
                    result += originalPath.CalcWindingInfo(nextSegmentMidpoint).Winding;
                }

                return result;
            }
        }

        private class VertexNode
        {
            public VertexNode(Point2D point)
            {
                Point = point;
            }

            public Point2D Point { get; }
            public List<PathVertexEx> Vertices { get; } = new List<PathVertexEx>();

            public override string ToString()
            {
                return $"Count={Vertices.Count}";
            }
        }

        private class SegmentNode : List<SegmentSplit>
        {
            public SegmentNode(IGeometryPathSegment originalSegment)
            {
                OriginalSegment = originalSegment;
            }

            public IGeometryPathSegment OriginalSegment { get; }

            public override string ToString()
            {
                return string.Join(",", this.Select(x => x.Segment));
            }
        }

        private struct SegmentSplit
        {
            public SegmentSplit(double distance, IGeometryPathSegment segment)
            {
                Distance = distance;
                Segment = segment;
            }

            public double Distance;
            public IGeometryPathSegment Segment;
        }

        private class SegmentSplitComparer : IComparer<SegmentSplit>
        {
            public static readonly SegmentSplitComparer Default = new SegmentSplitComparer();
            public int Compare(SegmentSplit x, SegmentSplit y)
            {
                return Comparer<double>.Default.Compare(x.Distance, y.Distance);
            }
        }

        private class AreaInfo
        {
            public AreaInfo(AARectD rect)
            {
                Rect = rect;
            }

            public AARectD Rect { get; }
            public HashSet<IGeometryPathSegment> OriginalSegments { get; } = new HashSet<IGeometryPathSegment>();
            public HashSet<IGeometryPath> OriginalPaths { get; } = new HashSet<IGeometryPath>();
        }
    }
}
