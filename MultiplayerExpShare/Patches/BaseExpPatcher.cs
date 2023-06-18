using AchtuurCore.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExpShare.Patches
{
    public abstract class BaseExpPatcher : GenericPatcher
    {
        /// <summary>
        /// Whether the current (patched) method is processing shared exp. If it is not, then exp should be shared
        /// </summary>
        protected static bool isProcessingSharedExp;

        public override void Patch(Harmony harmony)
        {
            isProcessingSharedExp = false;

            harmony.Patch(
                original: this.GetOriginalMethod<Farmer>(nameof(Farmer.gainExperience)),
                postfix: this.GetHarmonyMethod(nameof(Postfix_GainExperience))
            );
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix_GainExperience()
        {
            if (isProcessingSharedExp)
            {
                isProcessingSharedExp = false;
            }
        }

        /// <summary>
        /// Returns true if exp can be shared based on <see cref="isProcessingSharedExp"/>
        /// </summary>
        /// <returns></returns>
        protected static bool CanExpBeShared()
        {
            // Skip execution if world isnt loaded
            if (!Context.IsWorldReady)
                return false;

            // If processing shared exp, then 'howMuch' already contains correct exp to add and no message should be sent
            if (isProcessingSharedExp)
                return false;

            return true;
        }

        /// <summary>
        /// Calculate experience that goes to actor. If skill level is 10 and setting is enabled, this function returns 0
        /// </summary>
        /// <param name="totalExp"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected static int GetActorExp(int totalExp, int level)
        {
            // calculate actor exp gain, with rounding
            int actor_exp = (int)Math.Round(totalExp * ModEntry.GetActorExpPercentage(level));
            int total_shared_exp = (int)Math.Round(totalExp * ModEntry.GetSharedExpPercentage(level));
            int rounding_loss = totalExp - (actor_exp + total_shared_exp);
            return actor_exp + rounding_loss;
        }
    }
}
