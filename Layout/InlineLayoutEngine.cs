using Altium.Geometry2D;
using Altium.Geometry2D.Shapes;
using Altium.Wirebonding.Model;
using System;
using System.Collections.Generic;

namespace Altium.Wirebonding.Layout
{
    class InlineLayoutEngine : ILayoutEngine
    {
        public string Name => "Default";

        public IReadOnlyList<WireLoop> CreateLayout(Die die, WirebondingProfile profile, LayoutEngineOptions options)
        {
        }
    }
}
