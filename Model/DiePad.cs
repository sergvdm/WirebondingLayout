using Altium.Geometry2D;

namespace Altium.Wirebonding.Model
{
    // all length dimensions are in millimeters
    // all angular dimensions are in radians
    class DiePad : Pad
    {
        public DiePad(string name, Point2D location, double rotation, Size2D size, double thickness, double baseZ)
            : base(name, location, rotation, size, thickness, baseZ)
        {
        }
    }
}
