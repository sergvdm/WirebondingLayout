using System;
using System.Globalization;

namespace Altium.Geometry2D
{
    /// <summary>
    /// 2D vector (float)
    /// </summary>
    [Serializable]
    public struct Vector2F
    {
        public static readonly Vector2F Zero = new Vector2F();

        public static explicit operator Vector2F(Vector2D v)
        {
            return new Vector2F((float)v.X, (float)v.Y);
        }

        public static explicit operator Vector2F(Point2F pt)
        {
            return new Vector2F(pt.X, pt.Y);
        }

        public static bool operator ==(Vector2F v1, Vector2F v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2F v1, Vector2F v2)
        {
            return !(v1 == v2);
        }

        public static Vector2F operator -(Vector2F v)
        {
            return new Vector2F(-v.X, -v.Y);
        }

        public static Vector2F operator +(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2F operator -(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Point2F operator +(Point2F pt, Vector2F v)
        {
            return new Point2F(pt.X + v.X, pt.Y + v.Y);
        }

        public static Point2F operator +(Vector2F v, Point2F pt)
        {
            return new Point2F(v.X + pt.X, v.Y + pt.Y);
        }

        public static Point2F operator -(Point2F pt, Vector2F v)
        {
            return new Point2F(pt.X - v.X, pt.Y - v.Y);
        }

        public static float operator *(Vector2F v1, Vector2F v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static Vector2F operator *(float c, Vector2F v)
        {
            return new Vector2F(c * v.X, c * v.Y);
        }

        public static Vector2F operator *(Vector2F v, float c)
        {
            return new Vector2F(v.X * c, v.Y * c);
        }

        public static Vector2F operator /(Vector2F v, float c)
        {
            return new Vector2F(v.X / c, v.Y / c);
        }

        public static float operator ^(Vector2F v1, Vector2F v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public Vector2F(Point2F start, Point2F end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
        }

        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2F(double angle)
        {
            angle = angle.NormalizeRadAngle();
            X = (float)Math.Cos(angle);
            Y = (float)Math.Sin(angle);
        }

        public float X;
        public float Y;

        public float Angle => (float)Math.Atan2(Y, X);

        public float Norm()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public Vector2F Unit()
        {
            if (X == 0 && Y == 0) return new Vector2F(1, 0);
            var n = Norm();
            if (n < 1E-6) return new Vector2F(Angle);
            else return this / n;
        }

        public Vector2F Ortogonal()
        {
            return new Vector2F(-Y, X);
        }

        public Vector2F ProjectTo(Vector2F s)
        {
            return (this * s) / (s * s) * s;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0:F2},{1:F2}]", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2F)) return false;
            Vector2F v = (Vector2F)obj;
            return v == this;
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y;
        }
    }
}
