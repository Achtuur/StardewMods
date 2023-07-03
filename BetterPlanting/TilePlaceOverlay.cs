using AchtuurCore.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AchtuurCore.Utility;
using System.Reflection.Metadata;
using BetterPlanting.Extensions;

namespace BetterPlanting
{
    internal class TilePlaceOverlay : Overlay
    {
        internal const int DecayingTextLifeSpan = 60;

        private DecayingText _modeSwitchText;

        public DecayingText ModeSwitchText 
        { 
            get
            {
                return _modeSwitchText;
            } 
            set
            {
                if (_modeSwitchText is not null)
                    _modeSwitchText.Destroy();

                _modeSwitchText = value;
            }
        }
        protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
        {
            DrawSwitchText(spriteBatch);

            if (!ModEntry.PlayerIsHoldingPlantableObject() || ModEntry.Instance.TileFiller.fillMode == FillMode.Disabled)
                return;


            Vector2 farmerTilePosition = Game1.player.getTileLocation();
            Vector2 cursorTilePosition = Game1.currentCursorTile;
            IEnumerable<Vector2> visibleTiles = Tiles.GetVisibleTiles(expand: 1);
            IEnumerable<FillTile> tiles = ModEntry.Instance.TileFiller.GetFillTiles(farmerTilePosition, cursorTilePosition)
                .Where(ft => visibleTiles.Contains(ft.Location));


            foreach (FillTile tile in tiles)
            {
                Color color = tile.GetColor();
                Texture2D texture = (tile.TileState == TileState.Plantable) ? GreenTilePlacementTexture : TilePlacementTexture;
                DrawTile(spriteBatch, tile.Location, color, tileTexture: texture);
            }

            DrawPlantAmountText(spriteBatch, tiles.Count(t => t.TileState == TileState.Plantable));
        }

        /// <summary>
        /// Draw text when switching fill modes
        /// </summary>
        private void DrawSwitchText(SpriteBatch spriteBatch)
        {
            if (_modeSwitchText is null)
                return;

            if (_modeSwitchText.LifeSpanOver)
            {
                this._modeSwitchText = null;
                return;
            }

            
            Vector2 mousePos = ModEntry.Instance.Helper.Input.GetCursorPosition().AbsolutePixels;
            Vector2 screenCoords = Drawing.GetPositionScreenCoords(mousePos + Vector2.UnitY * -50f);
            this._modeSwitchText.DrawToScreen(spriteBatch, screenCoords, color: Color.White);
            
        }

        private void DrawPlantAmountText(SpriteBatch spriteBatch, int totalTiles)
        {
            if (!ModEntry.PlayerIsHoldingPlantableObject())
                return;

            // Amount of things that will be planted is smallest of <number of held objects> and <tiles in fill mode's range>
            int amt = Math.Min(Game1.player.ActiveObject.Stack, totalTiles);

            if (amt <= 0)
                return;

            // Get object that will be planted
            string plantType = "";
            if (Game1.player.IsHoldingCategory(ModEntry.SeedCategory))
                plantType = "seed";
            else if (Game1.player.IsHoldingCategory(ModEntry.FertilizerCategory))
                plantType = "fertilizer";

            Vector2 mousePos = ModEntry.Instance.Helper.Input.GetCursorPosition().AbsolutePixels;
            Vector2 offset = new Vector2(40f, 115f);
            Vector2 screenCoords = Drawing.GetPositionScreenCoords(mousePos + offset);
            spriteBatch.DrawString(Game1.dialogueFont, $"Plant {amt} {plantType}", screenCoords, Color.White);
        }
    }
}
