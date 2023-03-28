namespace Altium.Wirebonding.Model
{
    // all length dimensions are in millimeters
    // all angular dimensions are in radians
    class WireLoop
    {
        public WireLoop(DiePad diePad, FingerPad fingerPad, WireLoopProfile profile)
        {
            DiePad = diePad;
            FingerPad = fingerPad;
            Profile = profile;
        }

        public DiePad DiePad { get; }
        public FingerPad FingerPad { get; }
        public WireLoopProfile Profile { get; }
    }
}
