using System;

namespace Altium.Geometry2D.Shapes
{
    internal abstract class Shape : IShape
    {
        protected Shape()
        {
        }

        protected Shape(Shape other)
        {
        }

        protected abstract Shape CloneImpl();
        object ICloneable.Clone() => CloneImpl();

        public abstract bool Equal(IShape other);
    }
}
