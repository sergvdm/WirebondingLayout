using System;
using System.Collections.Generic;

namespace Altium.Geometry2D
{
    public class Point2DComparerYX : IComparer<Point2D>
    {
        public static readonly Point2DComparerYX Default = new Point2DComparerYX();

        public int Compare(Point2D pt1, Point2D pt2)
        {
            var result = Math.Sign(pt1.Y - pt2.Y);
            if (result == 0)
                result = Math.Sign(pt1.X - pt2.X);
            return result;
        }
    }
}
