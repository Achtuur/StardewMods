using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace AchtuurCore
{
    internal class ModEntry : Mod
    {
        internal static ModEntry Instance;
        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;

            HarmonyPatcher.ApplyPatches(this,
                new WateringPatcher()
            );
        }        
    }
}
