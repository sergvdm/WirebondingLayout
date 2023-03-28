using System;
using System.Globalization;

namespace Altium.Geometry2D
{
    /// <summary>
    /// 2D size (double)
    /// </summary>
    [Serializable]
    public struct Size2D
    {
        public static readonly Size2D Zero = new Size2D();

        public static bool operator ==(Size2D s1, Size2D s2)
        {
            return s1.Width == s2.Width && s1.Height == s2.Height;
        }

        public static bool operator !=(Size2D pt1, Size2D pt2)
        {
            return !(pt1 == pt2);
        }

        public Size2D(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Width;
        public double Height;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0:F2},{1:F2}]", Width, Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size2D)) return false;
            Size2D other = (Size2D)obj;
            return other == this;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }
    }
}
