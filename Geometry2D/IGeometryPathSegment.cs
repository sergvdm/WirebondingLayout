using Altium.Geometry2D.Shapes;
using System;

namespace Altium.Geometry2D
{
    public interface IGeometryPathSegment : IOpenShape
    {
        IGeometryPathSegment SnapToGrid(IGeometry2DEngine geometry2DEngine);
        GeometryPathVertex Split(Point2D point);
        IGeometryPathSegment TryJoin(IGeometry2DEngine geometry2DEngine, IGeometryPathSegment otherSegment, PathSegmentJoinMode otherSegmentJoinMode);
        IGeometryPathSegment Reverse();
        int WindingNumber(IGeometry2DEngine geometry2DEngine, Point2D point);
        new IGeometryPathSegment Clone();
    }

    [Flags]
    public enum PathSegmentJoinMode
    {
        Start = 1,
        End = 2,
        Any = Start | End
    }
}
