using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace StardewTravelSkill.Patches
{
    internal class ReduceActiveItemPatch
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        /// <summary>
        /// Postfix patch to <see cref="StardewValley.Game1.warpFarmer"/>.
        /// </summary>
        /// <param name="__result"></param>
        internal static bool Prefix_ReduceActiveItemByOne()
        {
            try
            {
                // Check if the held item that was used is a totem
                SObject held_item = Game1.player.ActiveObject;
                if (!isTotem(held_item.ParentSheetIndex))
                    return true;

                Random rnd = new Random();
                // Randomly decide if warp totem should be consumed
                // GetWarpTotemConsumeChance() returns 1 if the profession is unlocked, meaning the totem is always consumed
                if (rnd.NextDouble() > ModEntry.GetWarpTotemConsumeChance())
                {
                    Monitor.Log("Warp totem not consumed!", LogLevel.Debug);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Prefix_ReduceActiveItemByOne)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }


        static bool isTotem(int item_id)
        {
            switch (item_id)
            {
                case 261:
                case 688:
                case 689:
                case 690:
                case 886: return true;
                default: return false;
            }
        }
    }
}
