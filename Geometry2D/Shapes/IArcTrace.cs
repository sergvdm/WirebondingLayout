namespace Altium.Geometry2D.Shapes
{
    public interface IArcTrace : ITraceSegment
    {
        new IArc Guide { get; }
        new IArcTrace Clone();
    }
}
