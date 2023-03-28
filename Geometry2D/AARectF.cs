using System;

namespace Altium.Geometry2D
{
    /// <summary>
    /// Axis-aligned rectangle (float)
    /// </summary>
    [Serializable]
    public struct AARectF
    {
        public static readonly AARectF Zero = new AARectF();
        public static readonly AARectF EmptyAABB = new AARectF()
        {
            LB = new Point2F(float.PositiveInfinity, float.PositiveInfinity),
            RT = new Point2F(float.NegativeInfinity, float.NegativeInfinity)
        };

        public static implicit operator AARectF(AARectI rect)
        {
            return new AARectF() { LB = rect.LB, RT = rect.RT };
        }

        public static explicit operator AARectF(AARectD rect)
        {
            return new AARectF() { LB = (Point2F)rect.LB, RT = (Point2F)rect.RT };
        }

        public static bool operator ==(AARectF r1, AARectF r2)
        {
            return r1.LB == r2.LB && r1.RT == r2.RT;
        }

        public static bool operator !=(AARectF r1, AARectF r2)
        {
            return !(r1 == r2);
        }

        public AARectF(float x1, float y1, float x2, float y2)
        {
            LB = new Point2F(Math.Min(x1, x2), Math.Min(y1, y2));
            RT = new Point2F(Math.Max(x1, x2), Math.Max(y1, y2));
        }

        public AARectF(Point2F lb, Point2F rt)
        {
            LB = new Point2F(Math.Min(lb.X, rt.X), Math.Min(lb.Y, rt.Y));
            RT = new Point2F(Math.Max(lb.X, rt.X), Math.Max(lb.Y, rt.Y));
        }

        public AARectF(Point2F center, float width, float height)
        {
            var v = new Vector2F(width / 2, height / 2);
            LB = center - v;
            RT = center + v;
        }

        public Point2F LB;
        public Point2F RT;

        public Point2F LT => new Point2F(LB.X, RT.Y);
        public Point2F RB => new Point2F(RT.X, LB.Y);
        public Point2F Center => new Point2F((LB.X + RT.X) / 2, (LB.Y + RT.Y) / 2);
        public float Width => RT.X - LB.X;
        public float Height => RT.Y - LB.Y;
        public float DiagonalLength => (float)Math2D.DistanceTo(LB, RT);
        public float Area => Width * Height;
        public float MinSize => Math.Min(Width, Height);
        public float MaxSize => Math.Max(Width, Height);

        public bool Contains(Point2F pt)
        {
            return LB.X <= pt.X && pt.X <= RT.X && LB.Y <= pt.Y && pt.Y <= RT.Y;
        }

        public bool Contains(AARectF rect)
        {
            return LB.X <= rect.LB.X && rect.RT.X <= RT.X && LB.Y <= rect.LB.Y && rect.RT.Y <= RT.Y;
        }

        public bool Intersect(AARectF rect)
        {
            var x1 = LB.X >= rect.LB.X ? LB.X : rect.LB.X;
            var x2 = RT.X <= rect.RT.X ? RT.X : rect.RT.X;
            if (x2 < x1) return false;
            var y1 = LB.Y >= rect.LB.Y ? LB.Y : rect.LB.Y;
            var y2 = RT.Y <= rect.RT.Y ? RT.Y : rect.RT.Y;
            if (y2 < y1) return false;
            return true;
        }

        public AARectF? Intersection(AARectF rect)
        {
            var x1 = Math.Max(LB.X, rect.LB.X);
            var x2 = Math.Min(RT.X, rect.RT.X);
            var y1 = Math.Max(LB.Y, rect.LB.Y);
            var y2 = Math.Min(RT.Y, rect.RT.Y);
            if (x2 >= x1 && y2 >= y1) return new AARectF(new Point2F(x1, y1), new Point2F(x2, y2));
            return null;
        }

        public AARectF Union(AARectF rect)
        {
            var x1 = Math.Min(LB.X, rect.LB.X);
            var x2 = Math.Max(RT.X, rect.RT.X);
            var y1 = Math.Min(LB.Y, rect.LB.Y);
            var y2 = Math.Max(RT.Y, rect.RT.Y);
            return new AARectF(new Point2F(x1, y1), new Point2F(x2, y2));
        }

        public void UnionWith(AARectF rect)
        {
            var x1 = Math.Min(LB.X, rect.LB.X);
            var x2 = Math.Max(RT.X, rect.RT.X);
            var y1 = Math.Min(LB.Y, rect.LB.Y);
            var y2 = Math.Max(RT.Y, rect.RT.Y);
            LB = new Point2F(x1, y1);
            RT = new Point2F(x2, y2);
        }

        public AARectF Inflate(float x, float y)
        {
            return new AARectF(new Point2F(LB.X - x, LB.Y - y), new Point2F(RT.X + x, RT.Y + y));
        }

        public void InflateWith(float x, float y)
        {
            LB.X -= x;
            LB.Y -= y;
            RT.X += x;
            RT.Y += y;
        }

        public bool IsSinglePoint()
        {
            return Width == 0 && Height == 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AARectF)) return false;
            AARectF other = (AARectF)obj;
            return other == this;
        }

        public override int GetHashCode()
        {
            return LB.GetHashCode() ^ RT.GetHashCode();
        }

        public override string ToString()
        {
            return $"AARect({LB}-{RT})";
        }
    }
}
