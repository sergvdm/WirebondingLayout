using Altium.Geometry2D;
using Altium.Geometry2D.Shapes;
using System.Collections.Generic;

namespace Altium.Wirebonding.Model
{
    // all length dimensions are in millimeters
    // all angular dimensions are in radians
    class Die
    {
        public Die(string name, Point2D location, double rotation, IRectangle shape, double thickness, double baseZ, bool isFlipped, IReadOnlyList<DiePad> pads)
        {
            Name = name;
            Location = location;
            Rotation = rotation;
            Shape = shape;
            Thickness = thickness;
            BaseZ = baseZ;
            IsFlipped = isFlipped;
            Pads = pads;
        }

        public string Name { get; }
        public Point2D Location { get; }
        public double Rotation { get; }
        public IRectangle Shape { get; }
        public double Thickness { get; }
        public double BaseZ { get; }
        public bool IsFlipped { get; }
        public IReadOnlyList<DiePad> Pads { get; }
    }
}