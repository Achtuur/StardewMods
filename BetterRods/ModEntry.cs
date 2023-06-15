using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace BetterRods
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal ModConfig Config;

        public override void Entry(IModHelper helper)
        {

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            // HarmonyPatcher.ApplyPatches(this,
            
            // );

            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu();
        }
    }
}
