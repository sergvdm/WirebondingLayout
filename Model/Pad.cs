using Altium.Geometry2D;
using Altium.Geometry2D.Shapes;
using System;

namespace Altium.Wirebonding.Model
{
    // all length dimensions are in millimeters
    // all angular dimensions are in radians
    abstract class Pad
    {
        protected Pad(string name, Point2D location, double rotation, Size2D size, double thickness, double baseZ)
        {
            Name = name;
            Location = location;
            Rotation = rotation;
            Size = size;
            Thickness = thickness;
            BaseZ = baseZ;
        }

        public string Name { get; }
        public Point2D Location { get; }
        public double Rotation { get; }
        public Size2D Size { get; }
        public double Thickness { get; }
        public double BaseZ { get; }
    }
}
