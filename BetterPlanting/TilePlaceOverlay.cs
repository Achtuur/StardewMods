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
        internal readonly Vector2 SwitchTextOffset = new Vector2(0f, -50f);
        internal readonly Vector2 PlantAmountOffset = new Vector2(40f, 115f);

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

            if (!ModEntry.PlayerIsHoldingPlantableObject() || ModEntry.Instance.TileFiller.FillMode == FillMode.Disabled)
                return;

            Vector2 farmerTilePosition = Game1.player.getTileLocation();
            Vector2 cursorTilePosition = Game1.currentCursorTile;
            IEnumerable<Vector2> visibleTiles = Tiles.GetVisibleTiles(expand: 1);
            IEnumerable<FillTile> tiles = ModEntry.Instance.TileFiller.GetFillTiles(farmerTilePosition, cursorTilePosition)
                .Where(ft => visibleTiles.Contains(ft.Location));


            foreach (FillTile tile in tiles)
            {
                Color color = tile.GetColor();
                Texture2D texture = (tile.State == TileState.Plantable) ? GreenTilePlacementTexture : TilePlacementTexture;
                DrawTile(spriteBatch, tile.Location, color, tileTexture: texture);
            }

            DrawPlantAmountText(spriteBatch, GetPlantAmount(tiles, cursorTilePosition));
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
            Vector2 screenCoords = Drawing.GetPositionScreenCoords(mousePos + SwitchTextOffset);
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
                plantType = (amt > 1) ? "seeds" : "seed";
            else if (Game1.player.IsHoldingCategory(ModEntry.FertilizerCategory))
                plantType = "fertilizer";

            Vector2 mousePos = ModEntry.Instance.Helper.Input.GetCursorPosition().AbsolutePixels;
            Vector2 screenCoords = Drawing.GetPositionScreenCoords(mousePos + PlantAmountOffset);
            spriteBatch.DrawString(Game1.dialogueFont, $"Plant {amt} {plantType}", screenCoords, Color.White);
        }

        private int GetPlantAmount(IEnumerable<FillTile> fillTiles, Vector2 CursorTile)
        {
            int amt = fillTiles.Count(t => t.State == TileState.Plantable);

            int cursor_amt = 0;
            // if cursor tile can be planted, check if one of the fillTiles is the cursor
            if (ModEntry.IsCursorTilePlantable())
                cursor_amt = 1 - fillTiles.Where(t => t.State == TileState.Plantable && t.Location == CursorTile).Count();

            return amt + cursor_amt;
        }
    }
}
