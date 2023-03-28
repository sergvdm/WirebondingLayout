namespace Altium.Geometry2D.Shapes
{
    public interface ILineTrace : ITraceSegment
    {
        new ILine Guide { get; }
        new ILineTrace Clone();
    }
}
