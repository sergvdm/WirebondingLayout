using System.Collections.Generic;

namespace Altium.Geometry2D
{
    public interface IRegionBuilder
    {
        void BeginRegion();
        void Add(IGeometryPath path);
        void Add(params IGeometryPath[] paths);
        void Add(IEnumerable<IGeometryPath> paths);
        IRegion EndRegion();
    }
}
