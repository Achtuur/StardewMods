using AchtuurCore.Patches;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrench.Patches
{
    internal class DrawInMenuPatch : GenericPatcher
    {
        public override void Patch(Harmony harmony)
        {
            
        }



        public bool Prefix_DrawInMenu()
        {
            // if tool is wrench
            // draw

            return true;
        }
    }
}
