using AchtuurCore.Events;
using AchtuurCore.Patches;
using WateringCanGiveExp.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace WateringCanGiveExp
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance;
        public const int FarmingSkillID = 0;

        internal ModConfig Config;
        private float wateringExpTotal;

        public override void Entry(IModHelper helper)
        {

            HarmonyPatcher.ApplyPatches(this,
                new CropHarvestPatcher()
            );

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.wateringExpTotal = 0f;

            AchtuurCore.Events.EventPublisher.onFinishedWateringSoil += OnFinishedWateringSoil;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu();
        }

        private void OnFinishedWateringSoil(object sender, WateringFinishedArgs e)
        {
            // Quit if world is not loaded
            if (!Context.IsWorldReady)
                return;

            // Only add exp if farmer who watered is current player
            if (!e.farmer.IsLocalPlayer)
                return;

            // Add exp
            this.wateringExpTotal += this.Config.ExpforWateringSoil;
            int floored_total = (int) Math.Floor(wateringExpTotal);
            e.farmer.gainExperience(FarmingSkillID, floored_total);
            this.wateringExpTotal -= floored_total;
        }
    }
}
