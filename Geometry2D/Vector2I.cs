using System;
using System.Globalization;

namespace Altium.Geometry2D
{
    /// <summary>
    /// 2D vector (int)
    /// </summary>
    [Serializable]
    public struct Vector2I
    {
        public static readonly Vector2I Zero = new Vector2I();

        public static explicit operator Vector2I(Vector2F v)
        {
            return new Vector2I(v.X, v.Y);
        }

        public static explicit operator Vector2I(Vector2D v)
        {
            return new Vector2I(v.X, v.Y);
        }

        public static explicit operator Vector2I(Point2D pt)
        {
            return new Vector2I(pt.X, pt.Y);
        }

        public static bool operator ==(Vector2I v1, Vector2I v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2I v1, Vector2I v2)
        {
            return !(v1 == v2);
        }

        public static Vector2I operator -(Vector2I v)
        {
            return new Vector2I(-v.X, -v.Y);
        }

        public static Vector2I operator +(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2I operator -(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Point2D operator +(Point2D pt, Vector2I v)
        {
            return new Point2D(pt.X + v.X, pt.Y + v.Y);
        }

        public static Point2D operator +(Vector2I v, Point2D pt)
        {
            return new Point2D(v.X + pt.X, v.Y + pt.Y);
        }

        public static Point2D operator -(Point2D pt, Vector2I v)
        {
            return new Point2D(pt.X - v.X, pt.Y - v.Y);
        }

        public static double operator *(Vector2I v1, Vector2I v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static Vector2I operator *(double c, Vector2I v)
        {
            return new Vector2I(c * v.X, c * v.Y);
        }

        public static Vector2I operator *(Vector2I v, double c)
        {
            return new Vector2I(v.X * c, v.Y * c);
        }

        public static Vector2I operator /(Vector2I v, double c)
        {
            return new Vector2I(v.X / c, v.Y / c);
        }

        public static double operator ^(Vector2I v1, Vector2I v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public Vector2I(Point2I start, Point2I end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
        }

        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2I(double x, double y)
        {
            X = (int)Math.Round(x);
            Y = (int)Math.Round(y);
        }

        public Vector2I(double angle)
        {
            angle = angle.NormalizeRadAngle();
            X = (int)Math.Round(Math.Cos(angle));
            Y = (int)Math.Round(Math.Sin(angle));
        }

        public int X;
        public int Y;

        public double Angle => Math.Atan2(Y, X);

        public double Norm()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public Vector2I Unit()
        {
            if (X == 0 && Y == 0) return new Vector2I(1, 0);
            var n = Norm();
            if (n < 1E-6) return new Vector2I(Angle);
            else return this / n;
        }

        public Vector2I Ortogonal()
        {
            return new Vector2I(-Y, X);
        }

        public Vector2I ProjectTo(Vector2I s)
        {
            return (this * s) / (s * s) * s;
        }

        public Vector2I Rotate(double angle)
        {
            return new Vector2I(X * Math.Cos(angle), Y * Math.Sin(angle));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0},{1}]", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2I)) return false;
            Vector2I v = (Vector2I)obj;
            return v == this;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}
