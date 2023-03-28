using System;
using System.Collections.Generic;

namespace Altium.Geometry2D
{
    public class Point2DComparerXY : IComparer<Point2D>
    {
        public static readonly Point2DComparerXY Default = new Point2DComparerXY();

        public int Compare(Point2D pt1, Point2D pt2)
        {
            var result = Math.Sign(pt1.X - pt2.X);
            if (result == 0)
                result = Math.Sign(pt1.Y - pt2.Y);
            return result;
        }
    }
}
