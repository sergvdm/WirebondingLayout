using System;
using System.Globalization;

namespace Altium.Geometry2D
{
    /// <summary>
    /// 2D point (integer)
    /// </summary>
    [Serializable]
    public struct Point2I
    {
        public static readonly Point2I Zero = new Point2I();

        public static explicit operator Point2I(Point2F pt)
        {
            return new Point2I((int)pt.X, (int)pt.Y);
        }

        public static explicit operator Point2I(Vector2I v)
        {
            return new Point2I(v.X, v.Y);
        }

        public static explicit operator Point2I(Point2D pt)
        {
            return new Point2I((int)pt.X, (int)pt.Y);
        }

        public static bool operator ==(Point2I pt1, Point2I pt2)
        {
            return pt1.X == pt2.X && pt1.Y == pt2.Y;
        }

        public static bool operator !=(Point2I pt1, Point2I pt2)
        {
            return !(pt1 == pt2);
        }

        public static Point2I operator +(Point2I pt1, Point2I pt2)
        {
            return new Point2I(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }

        public static Vector2I operator -(Point2I pt1, Point2I pt2)
        {
            return new Vector2I(pt1.X - pt2.X, pt1.Y - pt2.Y);
        }

        public static Point2I operator *(int c, Point2I pt)
        {
            return new Point2I(c * pt.X, c * pt.Y);
        }

        public static Point2I operator *(Point2I pt, int c)
        {
            return new Point2I(pt.X * c, pt.Y * c);
        }

        public Point2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0:F2},{1:F2}]", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point2I)) return false;
            Point2I other = (Point2I)obj;
            return other == this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
