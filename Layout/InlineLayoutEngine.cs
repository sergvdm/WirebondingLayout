using Altium.Geometry2D;
using Altium.Geometry2D.Shapes;
using Altium.Wirebonding.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altium.Wirebonding.Layout
{
    class InlineLayoutEngine : ILayoutEngine
    {
        public string Name => "Default";

        public IReadOnlyList<WireLoop> CreateLayout(Die die, WirebondingProfile profile, double fingerPadThickness, double fingerPadBaseZ, LayoutEngineOptions options)
        {
            // create a list to store the wire loops
            var wireLoops = new List<WireLoop>(die.Pads.Count);

            // collect die sides
            var dieSides = new List<Line>();
            for (int i = 0; i < die.Shape.Vertices.Count; i++)
                dieSides.Add(new Line(die.Shape.Vertices[i], die.Shape.Vertices[(i + 1) % die.Shape.Vertices.Count]));

            // group pads by sides
            var diePadsBySide = new Dictionary<Line, List<DiePad>>();
            foreach (var dieSide in dieSides)
                diePadsBySide.Add(dieSide, new List<DiePad>());
            foreach (var diePad in die.Pads)
            {
                var minDist = double.MaxValue;
                Line minSide = null;
                foreach (var dieSide in dieSides)
                {
                    var dist = diePad.Location.ProjectTo(dieSide).DistanceTo(diePad.Location);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minSide = dieSide;
                    }
                }
                diePadsBySide[minSide].Add(diePad);
            }

            // order pads by distance from die shape center
            foreach (var kv in diePadsBySide)
                kv.Value.Sort((a, b) =>
                {
                    var ad = a.Location.DistanceTo(die.Shape.Center);
                    var bd = b.Location.DistanceTo(die.Shape.Center);
                    return Comparer<double>.Default.Compare(ad, bd);
                });

            // create a list to store wire loops for side
            var wireLoopsForSide = new List<WireLoop>(diePadsBySide.Values.Max(x => x.Count));

            // loop through the die sides and create a corresponding finger pad for each group of die pads
            foreach (var dieSide in dieSides)
            {
                var diePadsForSide = diePadsBySide[dieSide];
                if (diePadsForSide.Count == 0) continue;

                // projection of die shape center onto die side
                var dieShapeCenterOnSide = die.Shape.Center.ProjectTo(dieSide);

                wireLoopsForSide.Clear();
                Point2D fingerPadLocation;

                // create finger pads one by one starting from center pad
                foreach (var diePad in diePadsForSide)
                {
                    // projection of die pad onto die side
                    var diePadOnSide = diePad.Location.ProjectTo(dieSide);

                    // unit direction along die side from center
                    var directionAlongDieSide = new Vector2D(dieShapeCenterOnSide, diePadOnSide).Unit();

                    // orthogonal unit direction from die pad
                    var orthogonalDirection = dieSide.Vector.Orthogonal();

                    // try to place it on the orthogonal line from the current die pad
                    fingerPadLocation = diePadOnSide + orthogonalDirection * (profile.FingerPadToDieClearance + profile.FingerPadSize.Width / 2);

                    if (wireLoopsForSide.Count > 0)
                    {
                        // find previously created finger pad which is last in same direction as directionAlongDieSide
                        var previousFingerPad = (wireLoopsForSide as IEnumerable<WireLoop>).Reverse()
                            .Where(x => new Vector2D(dieShapeCenterOnSide, x.FingerPad.Location) * directionAlongDieSide > 0)
                            .FirstOrDefault()?.FingerPad;
                        if (previousFingerPad == null) previousFingerPad = wireLoopsForSide.First().FingerPad;

                        // check if current finger pad location overlaps with or violates the clearance with closest previous finger pad
                        var fingerPadDistanceToCenterOnSide = fingerPadLocation.ProjectTo(dieSide).DistanceTo(dieShapeCenterOnSide);
                        var previousFingerPadDistanceToCenterOnSide = previousFingerPad.Location.ProjectTo(dieSide).DistanceTo(dieShapeCenterOnSide);
                        var overlapDistance = fingerPadDistanceToCenterOnSide - previousFingerPadDistanceToCenterOnSide - profile.FingerPadSize.Height / 2 - previousFingerPad.Size.Height / 2;
                        if (overlapDistance < profile.FingerPadToFingerPadClearance)
                        {
                            // adjust the position of the finger pad to avoid the overlap or violation
                            var adjustmentDistance = profile.FingerPadToFingerPadClearance - overlapDistance;
                            fingerPadLocation += adjustmentDistance * directionAlongDieSide;
                        }
                    }

                    // create new finger pad, new wire loop and add to both wireLoopsForSide list and result wireLoops list
                    var fingerPad = new FingerPad(FormatFingerPadName(diePad), fingerPadLocation, dieSide.Vector.Angle, profile.FingerPadStyle, profile.FingerPadSize, fingerPadThickness, fingerPadBaseZ, diePad.IsFlipped);
                    var wireLoop = new WireLoop(diePad, fingerPad, profile.WireLoopProfile);
                    wireLoopsForSide.Add(wireLoop);
                    wireLoops.Add(wireLoop);
                }
            }

            return wireLoops;
        }

        private string FormatFingerPadName(DiePad diePad) => $"{diePad.Name}-FP";
    }
}
