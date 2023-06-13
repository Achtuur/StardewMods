using AchtuurCore.Events;
using AchtuurCore.Patches;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MultiplayerExpShare.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace MultiplayerExpShare
{
    internal class ModEntry : Mod
    {
        public static ModEntry Instance;
        public ModConfig Config;

        /// <summary>
        /// Returns whether <paramref name="other_farmer"/> is nearby <c>Game1.player</c>, based on <see cref="ModConfig.ExpShareType"/>
        /// </summary>
        /// <param name="other_farmer"></param>
        /// <returns></returns>
        public static bool FarmerIsNearby(Farmer other_farmer)
        {
            switch (Instance.Config.ExpShareType)
            {
                case ExpShareRangeType.Tile:
                    return other_farmer.currentLocation == Game1.player.currentLocation && IsInTileRange(other_farmer);
                case ExpShareRangeType.Map:
                    return other_farmer.currentLocation == Game1.player.currentLocation;
                case ExpShareRangeType.Global:
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Calculates euclidian distance between current tile of <c> name="Game1.player" </c> and <paramref name="other_farmer"/> and returns true if that value is less than or equal to <see cref="ModConfig.NearbyPlayerTileRange"/>
        /// </summary>
        /// <param name="other_farmer"></param>
        /// <returns></returns>
        public static bool IsInTileRange(Farmer other_farmer)
        {
            int dx = Game1.player.getTileX() - other_farmer.getTileX();
            int dy = Game1.player.getTileY() - other_farmer.getTileY();

            return dx*dx + dy*dy <= Instance.Config.NearbyPlayerTileRange * Instance.Config.NearbyPlayerTileRange;
        }

        public static Farmer[] GetNearbyPlayers() 
        {
            // return all players that are close to the main player
            List<Farmer> nearbyFarmers = new List<Farmer>();
            foreach (Farmer online_farmer in Game1.getOnlineFarmers())
            {
                // Skip if player is current player
                if (online_farmer.IsLocalPlayer)
                    continue;
                
                // Add other player to list if they are close enough to main player
                if (FarmerIsNearby(online_farmer))
                {
                    nearbyFarmers.Add(online_farmer);
                }
            }

            return nearbyFarmers.ToArray();
        }

        /// <summary>
        /// Get exp percentage for actor based on settings and skill level (if setting enabled)
        /// </summary>
        /// <param name="level">Optional, skill level of current skill being evaluated for exp</param>
        /// <returns></returns>
        public static float GetActorExpPercentage(int level)
        {
            if (ModEntry.Instance.Config.ShareAllExpAtMaxLevel && level == 10)
            {
                return 0f;
            }

            return ModEntry.Instance.Config.ExpPercentageToActor;
        }

        public static float GetSharedExpPercentage(int actor_level)
        {
            if (ModEntry.Instance.Config.ShareAllExpAtMaxLevel && actor_level == 10)
            {
                return 1f;
            }

            return 1f - ModEntry.Instance.Config.ExpPercentageToActor;
        }

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            HarmonyPatcher.ApplyPatches(this,
                new GainExperiencePatch(),
                new SpaceCoreExperiencePatch()
            );

            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunch;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "SharedExpGained")
            {
                ExpGainData msg_expdata = e.ReadAs<ExpGainData>();

                // if the source is self or self was not nearby, don't add exp
                if (msg_expdata.actor_multiplayerid == Game1.player.UniqueMultiplayerID || !msg_expdata.nearby_farmer_ids.Contains(Game1.player.UniqueMultiplayerID))
                    return;

                AchtuurCore.Logger.DebugLog(Instance.Monitor, $"Received {msg_expdata.amount} exp in {AchtuurCore.Logger.GetSkillNameFromId(msg_expdata.skill_id)} from ({msg_expdata.actor_multiplayerid})!");
                GainExperiencePatch.InvokeGainExperience(Game1.player, msg_expdata);
            }
            else if (e.FromModID == this.ModManifest.UniqueID && e.Type == "SharedExpGainedSpaceCore")
            {
                ExpGainDataSpaceCore msg_expdata = e.ReadAs<ExpGainDataSpaceCore>();

                // if this farmer was not nearby, don't add exp
                if (msg_expdata.actor_multiplayerid == Game1.player.UniqueMultiplayerID || !msg_expdata.nearby_farmer_ids.Contains(Game1.player.UniqueMultiplayerID))
                    return;

                AchtuurCore.Logger.DebugLog(Instance.Monitor, $"Received {msg_expdata.amount} exp in {msg_expdata.skill_id} from ({msg_expdata.actor_multiplayerid})!");
                SpaceCoreExperiencePatch.InvokeGainExperience(Game1.player, msg_expdata);
            }
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu();
        }
    }
}
