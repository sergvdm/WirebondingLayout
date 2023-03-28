using System;
using System.Collections.Generic;
using System.Linq;

namespace Altium.Geometry2D.Shapes
{
    internal class BeveledPolygon : Polygon, IBeveledPolygon
    {
        public BeveledPolygon(double beveling, ShapeBevelingMode bevelingMode, params Point2D[] vertices)
            : this(new BeveledPolyInitData(beveling, bevelingMode, vertices))
        {
        }

        internal BeveledPolygon(BeveledPolyInitData beveledPolyInitData)
            : base(beveledPolyInitData.Vertices)
        {
            Beveling = beveledPolyInitData.Beveling;
            BevelingMode = beveledPolyInitData.BevelingMode;
            ActualVertices = beveledPolyInitData.ActualVertices.ToList();
        }

        protected BeveledPolygon(BeveledPolygon other)
            : base(other)
        {
            Beveling = other.Beveling;
            BevelingMode = other.BevelingMode;
            ActualVertices = other.ActualVertices.ToList();
        }

        public double Beveling { get; }
        public ShapeBevelingMode BevelingMode { get; }
        public IReadOnlyList<Point2D> ActualVertices { get; }

        public override bool Equal(IShape obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (ReferenceEquals(obj, null) || !(obj is BeveledPolygon other)) return false;
            return Beveling == other.Beveling && Vertices.SequenceEqual(other.Vertices);
        }

        public new BeveledPolygon Clone() => new BeveledPolygon(this);
        protected override Shape CloneImpl() => Clone();
        IBeveledPolygon IBeveledPolygon.Clone() => Clone();

        protected override AARectD CalcBoundingRect()
        {
            double left = 0, top = 0, right = 0, bottom = 0;
            foreach (var vertex in ActualVertices)
            {
                if (left > vertex.X) left = vertex.X;
                if (top < vertex.Y) top = vertex.Y;
                if (right < vertex.X) right = vertex.X;
                if (bottom > vertex.Y) bottom = vertex.Y;
            }

            return new AARectD(new Point2D(left, bottom), new Point2D(right, top));
        }

        protected override void BuildRegion(IGeometry2DEngine geometry2DEngine, IRegionBuilder regionBuilder, IGeometryPathBuilder pathBuilder, bool invert)
        {
            pathBuilder.BeginPath(invert);
            pathBuilder.Add(ActualVertices);
            regionBuilder.Add(pathBuilder.EndPath(true));
        }
    }

    internal class BeveledPolyInitData
    {
        public BeveledPolyInitData(double beveling, ShapeBevelingMode bevelingMode, params Point2D[] vertices)
        {
            BevelingMode = bevelingMode;
            Beveling = BevelingMode == ShapeBevelingMode.Percent ? beveling.LimitRange(0, 100) : Math.Abs(beveling);
            Vertices = vertices;
            ActualVertices = new Point2D[Vertices.Length * 2];
            for (int i = 0; i < Vertices.Length; i++)
            {
                var prevVertex = i == 0 ? Vertices[Vertices.Length - 1] : Vertices[i - 1];
                var nextVertex = i == (Vertices.Length - 1) ? Vertices[0] : Vertices[i + 1];
                var pv = new Vector2D(Vertices[i], prevVertex);
                var nv = new Vector2D(Vertices[i], nextVertex);
                var bevelMaxSize = Math.Min(pv.Norm(), nv.Norm()) / 2;
                var bevelSize = BevelingMode == ShapeBevelingMode.Percent ? bevelMaxSize * Beveling / 100 : Math.Min(Beveling, bevelMaxSize);
                var bevelStartPoint = Vertices[i] + pv.Unit() * bevelSize;
                var bevelEndPoint = Vertices[i] + nv.Unit() * bevelSize;
                ActualVertices[i * 2] = bevelStartPoint;
                ActualVertices[i * 2 + 1] = bevelEndPoint;
            }
        }

        public double Beveling { get; private set; }
        public ShapeBevelingMode BevelingMode { get; private set; }
        public Point2D[] Vertices { get; private set; }
        public Point2D[] ActualVertices { get; private set; }
    }
}
