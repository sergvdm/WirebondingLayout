using Altium.Geometry2D.Shapes;
using System.Collections.Generic;

namespace Altium.Geometry2D
{
    public interface IRegion : ILimitedShape
    {
        IReadOnlyList<IGeometryPath> Paths { get; }
        WindingInfo CalcWindingInfo(Point2D point);
        bool IsInnerPoint(Point2D point, bool includeCoincident);
        IRegion Invert();
        new IRegion Clone();
    }
}
