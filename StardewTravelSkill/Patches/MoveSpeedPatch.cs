using AchtuurCore.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StardewTravelSkill.Patches
{
    internal class MoveSpeedPatch : GenericPatcher
    {
        private static IMonitor Monitor;

        public override void Patch(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;
            harmony.Patch(
                original: this.getOriginalMethod<Farmer>(nameof(Farmer.getMovementSpeed)),
                postfix: this.getHarmonyMethod(nameof(Postfix_GetMoveSpeed))
            );
        }
        /// <summary>
        /// Postfix patch to <see cref="StardewValley.Farmer.getMovementSpeed"/>. Multiplies result of that method by <see cref="ModEntry.GetMovespeedMultiplier"/>, which is based on <see cref="TravelSkill"/> level.
        /// </summary>
        /// <param name="__result"></param>
        internal static void Postfix_GetMoveSpeed(ref float __result)
        {
            try
            {
                __result *= ModEntry.GetMovespeedMultiplier();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Postfix_GetMoveSpeed)}:\n{ex}", LogLevel.Error);
            }
        }

        
    }
}
