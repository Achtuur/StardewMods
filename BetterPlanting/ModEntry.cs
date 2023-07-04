using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using StardewValley.TerrainFeatures;
using BetterPlanting.Extensions;
using System.Linq;
using AchtuurCore.Framework;

namespace BetterPlanting
{
    public class ModEntry : Mod
    {

        internal const int FertilizerCategory = -19;
        internal const int SeedCategory = -74;

        internal static ModEntry Instance;
        internal ModConfig Config;

        internal TilePlaceOverlay UIOverlay;
        internal TileFiller TileFiller;

        internal static bool PlayerIsHoldingPlantableObject()
        {
            return Game1.player.IsHoldingCategory(SeedCategory) || Game1.player.IsHoldingCategory(FertilizerCategory);
        }

        internal static bool CanPlantHeldObject(Vector2 tile)
        {
            if (Context.IsWorldReady && !Game1.currentLocation.isTileHoeDirt(tile))
                return false;

            if (!PlayerIsHoldingPlantableObject())
                return false;

            StardewValley.Item held_object = Game1.player.CurrentItem;
            bool isFertilizer = held_object.Category == ModEntry.FertilizerCategory;

            HoeDirt tileFeature = Game1.currentLocation.terrainFeatures[tile] as HoeDirt;
            return tileFeature.canPlantThisSeedHere(held_object.ParentSheetIndex, (int)tile.X, (int)tile.Y, isFertilizer)
                && !Game1.currentLocation.isObjectAtTile((int)tile.X, (int)tile.Y);
        }

        internal static bool TileContainsAliveCrop(Vector2 tile)
        {
            // If no crop -> no alive crop
            if (!Game1.currentLocation.isCropAtTile((int)tile.X, (int)tile.Y))
                return false;

            // Check if crop is dead
            HoeDirt tileFeature = Game1.currentLocation.terrainFeatures[tile] as HoeDirt;
            return !tileFeature.crop.dead.Value;
        }

        internal static bool IsCursorTilePlantable()
        {
            // cursor has to be on ring of 8 tiles around player and object must be plantable
            float player_cursor_distance = (Game1.currentCursorTile - Game1.player.getTileLocation()).Length();
            return player_cursor_distance < 1.5f && CanPlantHeldObject(Game1.currentCursorTile);
        }

        public override void Entry(IModHelper helper)
        {

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            UIOverlay = new TilePlaceOverlay();
            UIOverlay.Enable();
            TileFiller = new TileFiller();

            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Player.Warped += this.OnPlayerWarped;
        }

        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            this.TileFiller.SetFillMode(FillMode.Disabled);
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            this.UIOverlay.DrawOverlay(e.SpriteBatch);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            if (e.Button == SButton.MouseLeft)
            {
                this.TryPlantSeed(Game1.player.CurrentItem, Game1.currentCursorTile);
            }
            else if (e.Button == Config.IncrementModeKey)
            {
                this.TileFiller.IncrementFillMode(1);
                
            }
            else if (e.Button == Config.DecrementModeKey)
            {
                this.TileFiller.IncrementFillMode(-1);
            }
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu();
        }

        private void TryPlantSeed(StardewValley.Item held_object, Vector2 CursorTile)
        {
            if (!PlayerIsHoldingPlantableObject() || !Context.IsPlayerFree)
                return;

            IEnumerable<FillTile> tiles = TileFiller.GetFillTiles(Game1.player.getTileLocation(), CursorTile)
                .Where(t => t.State == TileState.Plantable);

            if (IsCursorTilePlantable())
                tiles = tiles.Where(t => t.Location != CursorTile);

            foreach (FillTile tile in tiles)
            {
                if (!Game1.currentLocation.terrainFeatures.ContainsKey(tile.Location))
                    continue;

                HoeDirt tileFeature = Game1.currentLocation.terrainFeatures[tile.Location] as HoeDirt;
                bool isFertilizer = held_object.Category == FertilizerCategory;
                
                bool planted = tileFeature.plant(held_object.ParentSheetIndex, (int)tile.Location.X, (int)tile.Location.Y, Game1.player, isFertilizer, Game1.currentLocation);

                if (planted)
                    Game1.player.ActiveObject.ConsumeInventoryItem(Game1.player, held_object.ParentSheetIndex, 1);
                    //Game1.player.removeItemFromInventory(held_object.ParentSheetIndex);
                
            }

        }
    }
}
