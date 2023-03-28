using System;
using System.Globalization;

namespace Altium.Geometry2D
{
    /// <summary>
    /// 2D vector (double)
    /// </summary>
    [Serializable]
    public struct Vector2D
    {
        public static readonly Vector2D Zero = new Vector2D();
        public static readonly Vector2D XUnit = new Vector2D(1, 0);
        public static readonly Vector2D YUnit = new Vector2D(0, 1);

        public static implicit operator Vector2D(Vector2F v)
        {
            return new Vector2D(v.X, v.Y);
        }

        public static explicit operator Vector2D(Point2D pt)
        {
            return new Vector2D(pt.X, pt.Y);
        }

        public static explicit operator Vector2D(Point2I pt)
        {
            return new Vector2D(pt.X, pt.Y);
        }

        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return !(v1 == v2);
        }

        public static Vector2D operator -(Vector2D v)
        {
            return new Vector2D(-v.X, -v.Y);
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Point2D operator +(Point2D pt, Vector2D v)
        {
            return new Point2D(pt.X + v.X, pt.Y + v.Y);
        }

        public static Point2D operator +(Vector2D v, Point2D pt)
        {
            return new Point2D(v.X + pt.X, v.Y + pt.Y);
        }

        public static Point2D operator -(Point2D pt, Vector2D v)
        {
            return new Point2D(pt.X - v.X, pt.Y - v.Y);
        }

        public static double operator *(Vector2D v1, Vector2D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static Vector2D operator *(double c, Vector2D v)
        {
            return new Vector2D(c * v.X, c * v.Y);
        }

        public static Vector2D operator *(Vector2D v, double c)
        {
            return new Vector2D(v.X * c, v.Y * c);
        }

        public static Vector2D operator /(Vector2D v, double c)
        {
            return new Vector2D(v.X / c, v.Y / c);
        }

        public static double operator ^(Vector2D v1, Vector2D v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public Vector2D(Point2D start, Point2D end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
        }

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2D(double angle)
        {
            angle = angle.NormalizeRadAngle();
            X = Math.Cos(angle);
            Y = Math.Sin(angle);
        }

        public double X;
        public double Y;

        public double Angle => Math.Atan2(Y, X);

        public double Norm()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public Vector2D Unit()
        {
            if (X == 0 && Y == 0) return new Vector2D(1, 0);
            var n = Norm();
            if (n < 1E-6) return new Vector2D(Angle);
            else return this / n;
        }

        public Vector2D Ortogonal()
        {
            return new Vector2D(-Y, X);
        }

        public Vector2D ProjectTo(Vector2D s)
        {
            return (this * s) / (s * s) * s;
        }

        public Vector2D Rotate(double angle)
        {
            return new Vector2D(X * Math.Cos(angle), Y * Math.Sin(angle));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0:F2},{1:F2}]", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2D)) return false;
            Vector2D v = (Vector2D)obj;
            return v == this;
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y;
        }
    }
}
