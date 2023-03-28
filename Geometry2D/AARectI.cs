using System;

namespace Altium.Geometry2D
{
    /// <summary>
    /// Axis-aligned rectangle (integer)
    /// </summary>
    [Serializable]
    public struct AARectI
    {
        public static readonly AARectI Zero = new AARectI();

        public static explicit operator AARectI(AARectF rect)
        {
            return new AARectI() { LB = (Point2I)rect.LB, RT = (Point2I)rect.RT };
        }

        public static explicit operator AARectI(AARectD rect)
        {
            return new AARectI() { LB = (Point2I)rect.LB, RT = (Point2I)rect.RT };
        }

        public static bool operator ==(AARectI r1, AARectI r2)
        {
            return r1.LB == r2.LB && r1.RT == r2.RT;
        }

        public static bool operator !=(AARectI r1, AARectI r2)
        {
            return !(r1 == r2);
        }

        public AARectI(int x1, int y1, int x2, int y2)
        {
            LB = new Point2I(Math.Min(x1, x2), Math.Min(y1, y2));
            RT = new Point2I(Math.Max(x1, x2), Math.Max(y1, y2));
        }

        public AARectI(Point2I lb, Point2I rt)
        {
            LB = new Point2I(Math.Min(lb.X, rt.X), Math.Min(lb.Y, rt.Y));
            RT = new Point2I(Math.Max(lb.X, rt.X), Math.Max(lb.Y, rt.Y));
        }

        public Point2I LB;
        public Point2I RT;

        public Point2I LT => new Point2I(LB.X, RT.Y);
        public Point2I RB => new Point2I(RT.X, LB.Y);
        public Point2I Center => new Point2I((LB.X + RT.X) / 2, (LB.Y + RT.Y) / 2);
        public int Width => RT.X - LB.X;
        public int Height => RT.Y - LB.Y;
        public int Area => Width * Height;
        public int MinSize => Math.Min(Width, Height);
        public int MaxSize => Math.Max(Width, Height);

        public bool Contains(Point2I pt)
        {
            return LB.X <= pt.X && pt.X <= RT.X && LB.Y <= pt.Y && pt.Y <= RT.Y;
        }

        public bool Contains(AARectI rect)
        {
            return LB.X <= rect.LB.X && rect.RT.X <= RT.X && LB.Y <= rect.LB.Y && rect.RT.Y <= RT.Y;
        }

        public bool Intersect(AARectI rect)
        {
            var x1 = LB.X >= rect.LB.X ? LB.X : rect.LB.X;
            var x2 = RT.X <= rect.RT.X ? RT.X : rect.RT.X;
            if (x2 < x1) return false;
            var y1 = LB.Y >= rect.LB.Y ? LB.Y : rect.LB.Y;
            var y2 = RT.Y <= rect.RT.Y ? RT.Y : rect.RT.Y;
            if (y2 < y1) return false;
            return true;
        }

        public AARectI? Intersection(AARectI rect)
        {
            var x1 = Math.Max(LB.X, rect.LB.X);
            var x2 = Math.Min(RT.X, rect.RT.X);
            var y1 = Math.Max(LB.Y, rect.LB.Y);
            var y2 = Math.Min(RT.Y, rect.RT.Y);
            if (x2 >= x1 && y2 >= y1) return new AARectI(new Point2I(x1, y1), new Point2I(x2, y2));
            return null;
        }

        public AARectI Union(AARectI rect)
        {
            var x1 = Math.Min(LB.X, rect.LB.X);
            var x2 = Math.Max(RT.X, rect.RT.X);
            var y1 = Math.Min(LB.Y, rect.LB.Y);
            var y2 = Math.Max(RT.Y, rect.RT.Y);
            return new AARectI(new Point2I(x1, y1), new Point2I(x2, y2));
        }

        public void UnionWith(AARectI rect)
        {
            var x1 = Math.Min(LB.X, rect.LB.X);
            var x2 = Math.Max(RT.X, rect.RT.X);
            var y1 = Math.Min(LB.Y, rect.LB.Y);
            var y2 = Math.Max(RT.Y, rect.RT.Y);
            LB = new Point2I(x1, y1);
            RT = new Point2I(x2, y2);
        }

        public AARectI Inflate(int x, int y)
        {
            return new AARectI(new Point2I(LB.X - x, LB.Y - y), new Point2I(RT.X + x, RT.Y + y));
        }

        public void InflateWith(int x, int y)
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
            if (!(obj is AARectI)) return false;
            AARectI other = (AARectI)obj;
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
