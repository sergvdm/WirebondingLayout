using System.Collections.Generic;

namespace Altium.Geometry2D
{
    internal class RegionBuilder : IRegionBuilder
    {
        private readonly IGeometry2DEngine geometry2DEngine;

        public RegionBuilder(IGeometry2DEngine geometry2DEngine)
        {
            this.geometry2DEngine = geometry2DEngine;
        }

        private LinkedList<IGeometryPath> pathList;

        public void BeginRegion()
        {
            pathList = new LinkedList<IGeometryPath>();
        }

        public void Add(IGeometryPath path)
        {
            pathList.AddLast(path);
        }

        public void Add(params IGeometryPath[] paths) => Add((IEnumerable<IGeometryPath>)paths);
        public void Add(IEnumerable<IGeometryPath> paths)
        {
            foreach (var path in paths)
                Add(path);
        }

        public IRegion EndRegion()
        {
            var region = new Region(geometry2DEngine, pathList);
            pathList = null;
            return region;
        }
    }
}
