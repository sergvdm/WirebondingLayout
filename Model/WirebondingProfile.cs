using Altium.Geometry2D;

namespace Altium.Wirebonding.Model
{
    // all length dimensions are in millimeters
    // all angular dimensions are in radians
    class WirebondingProfile
    {
        public PadStyle FingerPadShape { get; }
        public Point2D FingerPadSize { get; }
        public double FingerPadToFingerPadClearance { get; }
        public double FingerPadToDieClearance { get; }
        public double FingerPadAlignmentToWireTolerance { get; }
        public double WireToWireClearance { get; }
        public WireLoopProfile WireLoopProfile { get; }
        public string LayoutEngineName { get; }
    }
}
