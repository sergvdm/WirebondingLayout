using Altium.Geometry2D.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Altium.Geometry2D
{
    internal class Region : LimitedShape, IRegion
    {
        private readonly IGeometry2DEngine geometry2DEngine;

        public Region(IGeometry2DEngine geometry2DEngine, IEnumerable<IGeometryPath> paths)
        {
            this.geometry2DEngine = geometry2DEngine;
            Paths = paths.ToList();
        }

        protected Region(IGeometry2DEngine geometry2DEngine, Region other)
            : base(other)
        {
            this.geometry2DEngine = geometry2DEngine;
            Paths = other.Paths.Select(x => x.Clone()).ToList();
        }

        public IReadOnlyList<IGeometryPath> Paths { get; }

        public Region Clone() => new Region(geometry2DEngine, this);
        protected override Shape CloneImpl() => Clone();
        IRegion IRegion.Clone() => Clone();

        public Region Invert() => new Region(geometry2DEngine, Paths.Select(x => x.Reverse()));
        IRegion IRegion.Invert() => Invert();

        public WindingInfo CalcWindingInfo(Point2D point)
        {
            var result = new WindingInfo();
            foreach (var path in Paths)
            {
                var wi = path.CalcWindingInfo(point);
                if (wi.Coincident)
                    result.Coincident = true;
                else
                    result.Winding += wi.Winding;
            }

            return result;
        }

        public bool IsInnerPoint(Point2D point, bool includeCoincident) => Paths.Any(x => x.IsInnerPoint(point, includeCoincident));

        public override bool Equal(IShape obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (ReferenceEquals(obj, null) || !(obj is Region other)) return false;
            if (Paths.Count != other.Paths.Count) return false;
            for (int i = 0; i < Paths.Count; i++)
            {
                if (!Paths[i].Equal(other.Paths[i])) return false;
            }

            return true;
        }

        public override IRegion CreateRegion(IGeometry2DEngine geometry2DEngine, bool invert = false)
        {
            if (invert)
                return new Region(geometry2DEngine, Paths.Select(x => x.Reverse()));
            else
                return new Region(geometry2DEngine, this);
        }

        protected override AARectD CalcBoundingRect()
        {
            var result = AARectD.EmptyAABB;
            foreach (var path in Paths)
                result.UnionWith(path.BoundingRect);
            return result;
        }
    }
}
