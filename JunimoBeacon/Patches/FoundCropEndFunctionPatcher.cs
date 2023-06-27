using AchtuurCore.Patches;
using HarmonyLib;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunimoBeacon.Patches;

/// <summary>
/// Make <see cref="JunimoHarvester.foundCropEndFunction"/> smarter by selecting a tile that has not been taken by other junimo
/// </summary>
internal class FoundCropEndFunctionPatcher : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: GetOriginalMethod<JunimoHarvester>(nameof(JunimoHarvester.foundCropEndFunction)),
            postfix: GetHarmonyMethod(nameof(postfix))
        );
    }


    private static void postfix(PathNode currentNode, Point endPoint, GameLocation location, Character c, JunimoHarvester __instance, ref bool __result)
    {
        JunimoHut homeHut = (JunimoHut) AccessTools.Property(typeof(JunimoHarvester), "home").GetValue(__instance, null);
        foreach(JunimoHarvester junimo in homeHut.myJunimos) 
        {
            if (junimo.Equals(__instance) || junimo.controller is null || junimo.controller.pathToEndPoint is null)
                continue;

            Point junimoEndPoint = junimo.controller.pathToEndPoint.Last();
            // Declare foundEnd invalid if another junimo is already trying to go there
            if (junimoEndPoint.X == currentNode.x && junimoEndPoint.Y == currentNode.y)
            {
                __result = false;
                return;
            }
        }
    }
}
