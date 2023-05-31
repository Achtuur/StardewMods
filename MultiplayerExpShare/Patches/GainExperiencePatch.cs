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

        public ExpGainData (long actor_multiplayerid, long[] nearby_farmer_ids, int skill_id, int amount)
        {
            this.actor_multiplayerid = actor_multiplayerid;
            this.nearby_farmer_ids = nearby_farmer_ids;
            this.skill_id = skill_id;
            this.amount = amount;
        }
    }
    public class GainExperiencePatch : GenericPatcher
    {
        private static IMonitor Monitor;
        
        /// <summary>
        /// Whether the current (patched) method is processing shared exp. If it is not, then exp should be shared
        /// </summary>
        public static bool isProcessingSharedExp;

        public override void Patch(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;
            isProcessingSharedExp = false;

            harmony.Patch(
                original: this.getOriginalMethod<Farmer>(nameof(Farmer.gainExperience)),
                postfix: this.getHarmonyMethod(nameof(Postfix_GainExperience_ShareExpMessage))
            );

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
        public static void InvokeGainExperience(Farmer farmer, int which, int howMuch, bool isSharedExp)
        {
            isProcessingSharedExp = isSharedExp;
            farmer.gainExperience(which, howMuch);
        }
       

        [HarmonyPriority(Priority.Last)]
        private static void Postfix_GainExperience_ShareExpMessage()
        {
            if (isProcessingSharedExp)
            {
                isProcessingSharedExp = false;
            }
        }
        private static void Prefix_GainExperience(int which, ref int howMuch, Farmer __instance)
        {
            // Skip execution if world isnt loaded or farmer is not local player
            if (!Context.IsWorldReady || !__instance.IsLocalPlayer)
                return;

            // Skip sharing if its disabled for that skill
            if (!ExpShareEnabledForSkill(which))
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
            int shared_exp = (int) Math.Round(howMuch * (1 - ModConfig.ExpPercentageToActor) / nearbyFarmerIds.Length);

            // Send message of this instance of shared exp
            if (shared_exp > 0)
            {
                ExpGainData expdata = new ExpGainData(__instance.UniqueMultiplayerID, nearbyFarmerIds, which, shared_exp);
                ModEntry.Instance.Helper.Multiplayer.SendMessage<ExpGainData>(expdata, "SharedExpGained", modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });
            }

            // calculate actor exp gain, with rounding
            int actor_exp = (int) Math.Round(howMuch * ModConfig.ExpPercentageToActor);


            AchtuurCore.Debug.DebugLog(Monitor, $"({Game1.player.Name}) is sharing exp with {nearbyFarmerIds.Length} farmer(s): {howMuch} -> {actor_exp} / {shared_exp}");
            
            // Set actor exp to howMuch, so rest of method functions as if it had gotten only actor_exp
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
            return ModConfig.VanillaSkillEnabled[skill_id];
        }
    }
}
