namespace Altium.Geometry2D.Shapes
{
    internal abstract class ClosedShape : LimitedShape, IClosedShape
    {
        protected ClosedShape()
        {
        }

        protected ClosedShape(ClosedShape other) :
            base(other)
        {
        }

        public abstract double Perimeter();
        public abstract double Area();
    }
}