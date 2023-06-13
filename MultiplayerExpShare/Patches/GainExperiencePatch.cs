using AchtuurCore.Patches;
using HarmonyLib;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerExpShare.Patches
{
    public struct ExpGainData
    {
        public long actor_multiplayerid;
        public long[] nearby_farmer_ids;
        public int amount;
        public int skill_id;

        public ExpGainData(long actor_multiplayerid, long[] nearby_farmer_ids, int skill_id, int amount)
        {
            this.actor_multiplayerid = actor_multiplayerid;
            this.nearby_farmer_ids = nearby_farmer_ids;
            this.skill_id = skill_id;
            this.amount = amount;
        }
    }
    public class GainExperiencePatch : BaseExpPatcher
    {        
        public override void Patch(Harmony harmony, IMonitor monitor)
        {
            
            base.Patch(harmony, monitor);

            harmony.Patch(
                original: this.getOriginalMethod<Farmer>(nameof(Farmer.gainExperience)),
                prefix: this.getHarmonyMethod(nameof(Prefix_GainExperience))
            );

        }

        /// <summary>
        /// Calls <see cref="Farmer.gainExperience(int, int)"/> and sets <see cref="isProcessingSharedExp"/>. Use this method instead of <see cref="Farmer.gainExperience(int, int)"/> when working with shared exp
        /// </summary>
        /// <param name="farmer"></param>
        /// <param name="which"></param>
        /// <param name="howMuch"></param>
        /// <param name="isSharedExp"></param>
        public static void InvokeGainExperience(Farmer farmer, ExpGainData exp_data)
        {
            isProcessingSharedExp = true;
            farmer.gainExperience(exp_data.skill_id, exp_data.amount);
        }
       
        private static void Prefix_GainExperience(int which, ref int howMuch, Farmer __instance)
        {
            if (!CanExpBeShared())
                return;

            // Skip sharing if its disabled for that skill
            if (!ExpShareEnabledForSkill(which))
                return;

            // Get nearby farmer id's
            long[] nearbyFarmerIds = ModEntry.GetNearbyPlayers()
                .Where(f => ModEntry.GetActorExpPercentage(f.GetSkillLevel(which)) != 0f) // get all players that would actually receive exp
                .Select(f => f.UniqueMultiplayerID).ToArray();

            // If no farmers nearby to share exp with, actor gets all
            if (nearbyFarmerIds.Length == 0)
                return;

            int level = __instance.GetSkillLevel(which);
            int actor_exp = GetActorExp(howMuch, level);

            // Calculate shared exp, with rounding
            int shared_exp = (int)Math.Round(howMuch * ModEntry.GetSharedExpPercentage(level) / nearbyFarmerIds.Length);

            // Send message of this instance of shared exp
            if (shared_exp > 0)
            {
                ExpGainData expdata = new ExpGainData(__instance.UniqueMultiplayerID, nearbyFarmerIds, which, shared_exp);
                ModEntry.Instance.Helper.Multiplayer.SendMessage<ExpGainData>(expdata, "SharedExpGained", modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
            }


            AchtuurCore.Logger.DebugLog(Monitor, $"({Game1.player.Name}) is sharing exp with {nearbyFarmerIds.Length} farmer(s): {howMuch} -> {actor_exp} / {shared_exp}");

            howMuch = actor_exp;
        }

        /// <summary>
        /// Returns true if exp sharing is enabled for skill with skill_id <paramref name="skill_id"/>.
        /// 
        /// <para>Only works for vanilla Stardew skills, where id's are the same as in vanilla (0 = farming, 1 = fishing, 2 = foraging, 3 = mining, 4 = combat)</para>. Does not work with skill_id 5 (luck)
        /// </summary>
        /// <param name="skill_id"></param>
        /// <returns></returns>
        private static bool ExpShareEnabledForSkill(int skill_id)
        {
            if (skill_id < 0 || skill_id > 4)
                return false;

            return ModEntry.Instance.Config.VanillaSkillEnabled[skill_id];
        }
    }
}
