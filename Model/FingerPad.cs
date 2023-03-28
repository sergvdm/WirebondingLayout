using Altium.Geometry2D;

namespace Altium.Wirebonding.Model
{
    // all length dimensions are in millimeters
    // all angular dimensions are in radians
    class FingerPad : Pad
    {
        public FingerPad(string name, Point2D location, double rotation, PadStyle style, Size2D size, double thickness, double baseZ, bool isFlipped)
                : base(name, location, rotation, style, size, thickness, baseZ, isFlipped)
        {
        }
    }
}
