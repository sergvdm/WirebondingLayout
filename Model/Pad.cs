using Altium.Geometry2D;
using Altium.Geometry2D.Shapes;
using System;

namespace Altium.Wirebonding.Model
{
    // all length dimensions are in millimeters
    // all angular dimensions are in radians
    abstract class Pad
    {
        protected Pad(string name, Point2D location, double rotation, PadStyle style, Size2D size, double thickness, double baseZ, bool isFlipped)
        {
            Name = name;
            Location = location;
            Rotation = rotation;
            Style = style;
            Size = size;
            Thickness = thickness;
            BaseZ = baseZ;
            IsFlipped = isFlipped;
            shape = new Lazy<IClosedShape>(CreateShape);
        }

        public string Name { get; }
        public Point2D Location { get; }
        public double Rotation { get; }
        public PadStyle Style { get; }
        public Size2D Size { get; }
        public double Thickness { get; }
        public double BaseZ { get; }
        public bool IsFlipped { get; }

        private Lazy<IClosedShape> shape;
        public IClosedShape Shape => shape.Value;

        protected virtual IClosedShape CreateShape()
        {
            switch (Style)
            {
                case PadStyle.Circle:
                    return new Circle(Location, Size.Width);
                case PadStyle.Rect:
                    return new Rectangle(Location, Size.Width, Size.Height, Rotation);
                case PadStyle.ChamferedRect:
                    return new BeveledRectangle(Location, Size.Width, Size.Height, 50, ShapeBevelingMode.Percent, Rotation);
                case PadStyle.RoundedRect:
                    return new RoundedRectangle(Location, Size.Width, Size.Height, 50, ShapeRoundingMode.Percent, Rotation);
                default:
                    throw new NotSupportedException($"Unsupported pad style: {Style}");
            }
        }
    }
}
