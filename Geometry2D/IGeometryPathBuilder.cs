using System.Collections.Generic;

namespace Altium.Geometry2D
{
    public interface IGeometryPathBuilder
    {
        void BeginPath(bool reverse = false, bool allowReduce = true);
        void Add(Point2D lineVertex);
        void Add(params Point2D[] lineVertices);
        void Add(IEnumerable<Point2D> lineVertices);
        void Add(IGeometryPathSegment segment);
        void Add(params IGeometryPathSegment[] segments);
        void Add(IEnumerable<IGeometryPathSegment> segments);
        bool IsClosed { get; }
        void ClosePath();
        IGeometryPath EndPath(bool close);
    }
}
