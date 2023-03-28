using System;
using System.Globalization;

namespace Altium.Geometry2D
{
    /// <summary>
    /// 2D point (float)
    /// </summary>
    [Serializable]
    public struct Point2F
    {
        public static readonly Point2F Zero = new Point2F();

        public static implicit operator Point2F(Point2I pt)
        {
            return new Point2F(pt.X, pt.Y);
        }

        public static explicit operator Point2F(Point2D pt)
        {
            return new Point2F((float)pt.X, (float)pt.Y);
        }

        public static bool operator ==(Point2F pt1, Point2F pt2)
        {
            return pt1.X == pt2.X && pt1.Y == pt2.Y;
        }

        public static bool operator !=(Point2F pt1, Point2F pt2)
        {
            return !(pt1 == pt2);
        }

        public static Point2F operator +(Point2F pt1, Point2F pt2)
        {
            return new Point2F(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }

        public static Vector2F operator -(Point2F pt1, Point2F pt2)
        {
            return new Vector2F(pt1.X - pt2.X, pt1.Y - pt2.Y);
        }

        public static Point2F operator *(float c, Point2F pt)
        {
            return new Point2F(c * pt.X, c * pt.Y);
        }

        public static Point2F operator *(Point2F pt, float c)
        {
            return new Point2F(pt.X * c, pt.Y * c);
        }

        public static Point2F operator /(Point2F pt, float c)
        {
            return new Point2F(pt.X / c, pt.Y / c);
        }

        public Point2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X;
        public float Y;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0:F2},{1:F2}]", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point2F)) return false;
            Point2F other = (Point2F)obj;
            return other == this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
