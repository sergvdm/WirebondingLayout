using System;

namespace Altium.Geometry2D.Shapes
{
    public interface IShape : ICloneable
    {
        bool Equal(IShape obj);
    }
}
