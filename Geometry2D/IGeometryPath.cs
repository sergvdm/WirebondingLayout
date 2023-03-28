using Altium.Geometry2D.Shapes;
using System.Collections.Generic;

namespace Altium.Geometry2D
{
    public interface IGeometryPath : ILimitedShape
    {
        IReadOnlyList<IGeometryPathSegment> Segments { get; }
        bool IsClosed { get; }
        Point2D? GetInnerPoint();
        int Winding { get; }
        WindingInfo CalcWindingInfo(Point2D point);
        bool IsInnerPoint(Point2D point, bool includeCoincident);
        new IGeometryPath Clone();
        IGeometryPath Reverse();
    }
}
