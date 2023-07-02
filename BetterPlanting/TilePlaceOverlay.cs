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

            if (Game1.player.CurrentItem is null)
                return;

            int itemGroup = Game1.player.CurrentItem.Category;
            if (itemGroup != ModEntry.SeedCategory && itemGroup != ModEntry.FertilizerCategory)
                return;


            Vector2 farmerTilePosition = Game1.player.getTileLocation();
            Vector2 cursorTilePosition = Game1.currentCursorTile;
            IEnumerable<Vector2> visibleTiles = Tiles.GetVisibleTiles(expand: 1);
            IEnumerable<FillTile> tiles = ModEntry.Instance.TileFiller.GetFillTiles(farmerTilePosition, cursorTilePosition)
                .Where(ft => visibleTiles.Contains(ft.Location));


            foreach (FillTile tile in tiles)
            {
                Color color = tile.GetColor();
                DrawTile(spriteBatch, tile.Location, color, tileTexture: GreenTilePlacementTexture);
            }

            //DrawTiles(spriteBatch, tiles, tileTexture: Overlay.GreenTilePlacementTexture);
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
    }
}
