using System;
using System.Globalization;

namespace Altium.Geometry2D
{
    /// <summary>
    /// 2D point (double)
    /// </summary>
    [Serializable]
    public struct Point2D
    {
        public static readonly Point2D Zero = new Point2D();

        public static implicit operator Point2D(Point2I pt)
        {
            return new Point2D(pt.X, pt.Y);
        }

        public static implicit operator Point2D(Point2F pt)
        {
            return new Point2D(pt.X, pt.Y);
        }

        public static explicit operator Point2D(Vector2D v)
        {
            return new Point2D(v.X, v.Y);
        }

        public static bool operator ==(Point2D pt1, Point2D pt2)
        {
            return pt1.X == pt2.X && pt1.Y == pt2.Y;
        }

        public static bool operator !=(Point2D pt1, Point2D pt2)
        {
            return !(pt1 == pt2);
        }

        public static Point2D operator +(Point2D pt1, Point2D pt2)
        {
            return new Point2D(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }

        public static Vector2D operator -(Point2D pt1, Point2D pt2)
        {
            return new Vector2D(pt1.X - pt2.X, pt1.Y - pt2.Y);
        }

        public static Point2D operator *(double c, Point2D pt)
        {
            return new Point2D(c * pt.X, c * pt.Y);
        }

        public static Point2D operator *(Point2D pt, double c)
        {
            return new Point2D(pt.X * c, pt.Y * c);
        }

        public static Point2D operator /(Point2D pt, double c)
        {
            return new Point2D(pt.X / c, pt.Y / c);
        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0:F2},{1:F2}]", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point2D)) return false;
            Point2D other = (Point2D)obj;
            return other == this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
