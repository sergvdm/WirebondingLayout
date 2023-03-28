using Altium.Geometry2D.Shapes;
using System.Collections.Generic;

namespace Altium.Wirebonding.Model
{
    // all length dimensions are in millimeters
    // all angular dimensions are in radians
    class Die
    {
        public Die(string name, IRectangle shape, double thickness, double baseZ, IReadOnlyList<DiePad> pads)
        {
            Name = name;
            Shape = shape;
            Thickness = thickness;
            BaseZ = baseZ;
            Pads = pads;
        }

        public string Name { get; }
        public IRectangle Shape { get; }
        public double Thickness { get; }
        public double BaseZ { get; }
        public IReadOnlyList<DiePad> Pads { get; }
    }
}