using System.Collections.Generic;
using System.Linq;

namespace Altium.Geometry2D
{
    public class GeometryPathVertexIterator
    {
        private readonly IReadOnlyList<IGeometryPathSegment> segments;
        private readonly bool isClosed;

        public GeometryPathVertexIterator(IGeometryPath path)
            : this(path.Segments)
        {
        }

        public GeometryPathVertexIterator(IReadOnlyList<IGeometryPathSegment> segments)
        {
            this.segments = segments;
            this.isClosed = segments.Count > 0 && segments[0].StartPoint == segments[segments.Count - 1].EndPoint;
        }

        public IEnumerable<GeometryPathVertex> Vertices
        {
            get
            {
                var prevSegment = isClosed ? segments.Last() : null;
                foreach (var nextSegment in segments)
                {
                    yield return new GeometryPathVertex(prevSegment, nextSegment);
                    prevSegment = nextSegment;
                }

                if (!isClosed)
                    yield return new GeometryPathVertex(prevSegment, null);
            }
        }
    }
}
