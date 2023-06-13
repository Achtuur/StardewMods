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
    public class SpaceCoreExperiencePatch : BaseExpPatcher
    {
        
        public override void Patch(Harmony harmony, IMonitor monitor)
        {
            if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
                return;

            base.Patch(harmony, monitor);

            harmony.Patch(
                original: this.getOriginalMethod<SpaceCore.Skills>(nameof(SpaceCore.Skills.AddExperience)),
                prefix: this.getHarmonyMethod(nameof(Prefix_AddExperienceSpaceCore))
            );

        }

        /// <summary>
        /// Calls <see cref="Farmer.gainExperience(int, int)"/> and sets <see cref="isProcessingSharedExp"/>. Use this method instead of <see cref="Farmer.gainExperience(int, int)"/> when working with shared exp
        /// </summary>
        /// <param name="farmer"></param>
        /// <param name="which"></param>
        /// <param name="howMuch"></param>
        /// <param name="isSharedExp"></param>
        public static void InvokeGainExperience(Farmer farmer, ExpGainDataSpaceCore exp_data)
        {
            isProcessingSharedExp = true;
            farmer.AddCustomSkillExperience(exp_data.skill_id, exp_data.amount);
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
            if (!CanExpBeShared())
                return;

            // Skip sharing if its disabled for that skill
            if (!ExpShareEnabledForSkill(skillName))
                return;

            // Get nearby farmer id's
            long[] nearbyFarmerIds = ModEntry.GetNearbyPlayers()
                .Where(f => ModEntry.GetActorExpPercentage(f.GetCustomSkillLevel(skillName)) != 0f) // get all players that would actually receive exp
                .Select(f => f.UniqueMultiplayerID).ToArray();

            // If no farmers nearby to share exp with, actor gets all
            if (nearbyFarmerIds.Length == 0)
                return;

            // calculate actor exp gain, with rounding
            int level = farmer.GetCustomSkillLevel(skillName);
            int actor_exp = GetActorExp(amt, level);

            // Calculate shared exp, with rounding
            int shared_exp = (int)Math.Round(amt * ModEntry.GetSharedExpPercentage(level) / nearbyFarmerIds.Length);

            // Send message of this instance of shared exp
            if (shared_exp > 0)
            {
                ExpGainDataSpaceCore expdata = new ExpGainDataSpaceCore(farmer.UniqueMultiplayerID, nearbyFarmerIds, skillName, shared_exp);
                ModEntry.Instance.Helper.Multiplayer.SendMessage<ExpGainDataSpaceCore>(expdata, "SharedExpGainedSpaceCore", modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
            }


            AchtuurCore.Logger.DebugLog(Monitor, $"({Game1.player.Name}) is sharing exp with {nearbyFarmerIds.Length} farmer(s) in {skillName}: {amt} -> {actor_exp} / {shared_exp}");

            // Set actor exp to howMuch, so rest of method functions as if it had gotten only actor_exp
            amt = actor_exp;
        }


        private static bool ExpShareEnabledForSkill(string skillName)
        {
            return ModEntry.Instance.Config.SpaceCoreSkillEnabled[skillName];
        }
    }
}
