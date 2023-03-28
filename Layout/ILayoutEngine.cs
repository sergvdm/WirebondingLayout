using Altium.Wirebonding.Model;
using System.Collections.Generic;

namespace Altium.Wirebonding.Layout
{
    interface ILayoutEngine
    {
        string Name { get; }
        IReadOnlyList<WireLoop> CreateLayout(Die die, WirebondingProfile profile, double fingerPadThickness, double fingerPadBaseZ, LayoutEngineOptions options);
    }
}
