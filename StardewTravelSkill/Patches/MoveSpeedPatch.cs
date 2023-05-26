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
    internal class MoveSpeedPatch
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
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
