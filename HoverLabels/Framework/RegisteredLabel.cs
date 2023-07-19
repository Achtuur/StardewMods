using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Framework;
internal record RegisteredLabel(IManifest Manifest, string Name, IHoverLabel Label)
{
    public bool Enabled { get; set; } = true;
}
