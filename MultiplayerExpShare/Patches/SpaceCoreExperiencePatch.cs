using AchtuurCore.Patches;
using HarmonyLib;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExpShare.Patches
{
    public struct ExpGainDataSpaceCore
    {
        public long actor_multiplayerid;
        public long[] nearby_farmer_ids;
        public int amount;
        public string skill_id;

        public ExpGainDataSpaceCore(long actor_multiplayerid, long[] nearby_farmer_ids, string skill_id, int amount)
        {
            this.actor_multiplayerid = actor_multiplayerid;
            this.nearby_farmer_ids = nearby_farmer_ids;
            this.skill_id = skill_id;
            this.amount = amount;
        }
    }

    /// <summary>
    /// Patch for <see cref="SpaceCore.Skills.AddExperience(Farmer, string, int)"/> to support exp sharing for SpaceCore based skills
    /// </summary>
    public class SpaceCoreExperiencePatch : GenericPatcher
    {
        /// <summary>
        /// Whether the current (patched) method is processing shared exp. If it is not, then exp should be shared
        /// </summary>
        public static bool isProcessingSharedExp;

        public override void Patch(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;
            isProcessingSharedExp = false;

            if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
                return;

            harmony.Patch(
                original: this.getOriginalMethod<SpaceCore.Skills>(nameof(SpaceCore.Skills.AddExperience)),
                prefix: this.getHarmonyMethod(nameof(Prefix_AddExperienceSpaceCore))
            );

            harmony.Patch(
                original: this.getOriginalMethod<SpaceCore.Skills>(nameof(SpaceCore.Skills.AddExperience)),
                postfix: this.getHarmonyMethod(nameof(Postfix_GainExperience))
            );
        }

        /// <summary>
        /// Calls <see cref="Farmer.gainExperience(int, int)"/> and sets <see cref="isProcessingSharedExp"/>. Use this method instead of <see cref="Farmer.gainExperience(int, int)"/> when working with shared exp
        /// </summary>
        /// <param name="farmer"></param>
        /// <param name="which"></param>
        /// <param name="howMuch"></param>
        /// <param name="isSharedExp"></param>
        public static void InvokeGainExperience(Farmer farmer, string which, int howMuch, bool isSharedExp)
        {
            isProcessingSharedExp = isSharedExp;
            farmer.AddCustomSkillExperience(which, howMuch);
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix_GainExperience()
        {
            if (isProcessingSharedExp)
            {
                isProcessingSharedExp = false;
            }
        }

        private static void Prefix_AddExperienceSpaceCore(Farmer farmer, string skillName, ref int amt)
        {
            // Skip execution if world isnt loaded
            if (!Context.IsWorldReady)
                return;

            // Skip sharing if its disabled for that skill
            if (!ExpShareEnabledForSkill(skillName))
                return;


            // If processing shared exp, then 'howMuch' already contains correct exp to add and no message should be sent
            if (isProcessingSharedExp)
                return;

            // Get nearby farmer id's
            long[] nearbyFarmerIds = ModEntry.GetNearbyPlayers().Select(f => f.UniqueMultiplayerID).ToArray();

            // If no farmers nearby to share exp with, actor gets all
            if (nearbyFarmerIds.Length == 0)
                return;

            // Calculate shared exp, with rounding
            int shared_exp = (int)Math.Round(amt * (1 - ModEntry.Instance.Config.ExpPercentageToActor) / nearbyFarmerIds.Length);

            // Send message of this instance of shared exp
            if (shared_exp > 0)
            {
                ExpGainDataSpaceCore expdata = new ExpGainDataSpaceCore(farmer.UniqueMultiplayerID, nearbyFarmerIds, skillName, shared_exp);
                ModEntry.Instance.Helper.Multiplayer.SendMessage<ExpGainDataSpaceCore>(expdata, "SharedExpGainedSpaceCore", modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
            }

            // calculate actor exp gain, with rounding
            int actor_exp = (int) Math.Round(amt * ModEntry.Instance.Config.ExpPercentageToActor);

            int rounding_loss = amt - (actor_exp + shared_exp);

            AchtuurCore.Debug.DebugLog(Monitor, $"({Game1.player.Name}) is sharing exp with {nearbyFarmerIds.Length} farmer(s) in {skillName}: {amt} -> {actor_exp + rounding_loss} / {shared_exp}");

            // Set actor exp to howMuch, so rest of method functions as if it had gotten only actor_exp
            amt = actor_exp + rounding_loss;
        }


        private static bool ExpShareEnabledForSkill(string skillName)
        {
            return true;
        }
    }
}
