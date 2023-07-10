using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace HoverLabels
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal ModConfig Config;

        /// <summary>
        /// Tile the cursor is hovering over
        /// </summary>
        internal Vector2 cursorHoverTile;

        /// <summary>
        /// Number of ticks the cursor is hovering over <see cref="cursorHoverTile"/>
        /// </summary>
        internal int cursorHoverCount;

        internal LabelOverlay labelOverlay;

        public override void Entry(IModHelper helper)
        {

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            // HarmonyPatcher.ApplyPatches(this,
            
            // );

            this.Config = this.Helper.ReadConfig<ModConfig>();

            this.cursorHoverCount = 0;
            this.cursorHoverTile = Vector2.Zero;
            this.labelOverlay = new LabelOverlay();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            this.labelOverlay.DrawOverlay(e.SpriteBatch);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            UpdateCursorHover();
        }

        private void UpdateCursorHover()
        {

            // Reset cursor tile position if it shouldn't be enabled
            Vector2 cursorTile = Game1.currentCursorTile;
            if (!Context.IsWorldReady || !Context.IsPlayerFree || cursorTile != cursorHoverTile)
            {
                ResetCursorHoverState(cursorTile);
                return;
            }

            // Enable overlay when tick count is larger than delay
            cursorHoverCount++;
            if (cursorHoverCount >= Config.LabelPopupDelayTicks)
            {
                labelOverlay.SetCursorTile(this.cursorHoverTile);
                labelOverlay.Enable();
            }

        }

        private void ResetCursorHoverState(Vector2 cursorTile)
        {
            this.cursorHoverTile = cursorTile;
            this.cursorHoverCount = 0;
            labelOverlay.Disable();
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu();
        }
    }
}
