using System;

namespace Altium.Geometry2D
{
    /// <summary>
    /// Axis-aligned rectangle (double)
    /// </summary>
    [Serializable]
    public struct AARectD
    {
        public static readonly AARectD Zero = new AARectD();
        public static readonly AARectD EmptyAABB = new AARectD()
        {
            LB = new Point2D(double.PositiveInfinity, double.PositiveInfinity),
            RT = new Point2D(double.NegativeInfinity, double.NegativeInfinity)
        };

        public static implicit operator AARectD(AARectI rect)
        {
            return new AARectD() { LB = rect.LB, RT = rect.RT };
        }

        public static implicit operator AARectD(AARectF rect)
        {
            return new AARectD() { LB = rect.LB, RT = rect.RT };
        }

        public static bool operator ==(AARectD r1, AARectD r2)
        {
            return r1.LB == r2.LB && r1.RT == r2.RT;
        }

        public static bool operator !=(AARectD r1, AARectD r2)
        {
            return !(r1 == r2);
        }

        public AARectD(double x1, double y1, double x2, double y2)
        {
            LB = new Point2D(Math.Min(x1, x2), Math.Min(y1, y2));
            RT = new Point2D(Math.Max(x1, x2), Math.Max(y1, y2));
        }

        public AARectD(Point2D lb, Point2D rt)
        {
            LB = new Point2D(Math.Min(lb.X, rt.X), Math.Min(lb.Y, rt.Y));
            RT = new Point2D(Math.Max(lb.X, rt.X), Math.Max(lb.Y, rt.Y));
        }

        public AARectD(Point2D center, double width, double height)
        {
            var v = new Vector2D(width / 2, height / 2);
            LB = center - v;
            RT = center + v;
        }

        public Point2D LB;
        public Point2D RT;

        public Point2D LT => new Point2D(LB.X, RT.Y);
        public Point2D RB => new Point2D(RT.X, LB.Y);
        public Point2D Center => new Point2D((LB.X + RT.X) / 2, (LB.Y + RT.Y) / 2);
        public double Width => RT.X - LB.X;
        public double Height => RT.Y - LB.Y;
        public double DiagonalLength => LB.DistanceTo(RT);
        public double Area => Width * Height;
        public double MinSize => Math.Min(Width, Height);
        public double MaxSize => Math.Max(Width, Height);

        public bool Contains(Point2D pt)
        {
            return LB.X <= pt.X && pt.X <= RT.X && LB.Y <= pt.Y && pt.Y <= RT.Y;
        }

        public bool Contains(AARectD rect)
        {
            return LB.X <= rect.LB.X && rect.RT.X <= RT.X && LB.Y <= rect.LB.Y && rect.RT.Y <= RT.Y;
        }

        public bool Intersect(AARectD rect)
        {
            var x1 = LB.X >= rect.LB.X ? LB.X : rect.LB.X;
            var x2 = RT.X <= rect.RT.X ? RT.X : rect.RT.X;
            if (x2 < x1) return false;
            var y1 = LB.Y >= rect.LB.Y ? LB.Y : rect.LB.Y;
            var y2 = RT.Y <= rect.RT.Y ? RT.Y : rect.RT.Y;
            if (y2 < y1) return false;
            return true;
        }

        public AARectD? Intersection(AARectD rect)
        {
            var x1 = Math.Max(LB.X, rect.LB.X);
            var x2 = Math.Min(RT.X, rect.RT.X);
            var y1 = Math.Max(LB.Y, rect.LB.Y);
            var y2 = Math.Min(RT.Y, rect.RT.Y);
            if (x2 >= x1 && y2 >= y1) return new AARectD(new Point2D(x1, y1), new Point2D(x2, y2));
            return null;
        }

        public AARectD Union(AARectD rect)
        {
            var x1 = Math.Min(LB.X, rect.LB.X);
            var x2 = Math.Max(RT.X, rect.RT.X);
            var y1 = Math.Min(LB.Y, rect.LB.Y);
            var y2 = Math.Max(RT.Y, rect.RT.Y);
            return new AARectD(new Point2D(x1, y1), new Point2D(x2, y2));
        }

        public void UnionWith(AARectD rect)
        {
            var x1 = Math.Min(LB.X, rect.LB.X);
            var x2 = Math.Max(RT.X, rect.RT.X);
            var y1 = Math.Min(LB.Y, rect.LB.Y);
            var y2 = Math.Max(RT.Y, rect.RT.Y);
            LB = new Point2D(x1, y1);
            RT = new Point2D(x2, y2);
        }

        public AARectD Inflate(double x, double y)
        {
            return new AARectD(new Point2D(LB.X - x, LB.Y - y), new Point2D(RT.X + x, RT.Y + y));
        }

        public void InflateWith(double x, double y)
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
            if (!(obj is AARectD)) return false;
            AARectD other = (AARectD)obj;
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
