using BetterPlanting.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;

namespace BetterPlanting
{
    public enum TileState
    {
        Plantable,
        AlreadyPlanted,
        NonPlantable,
    }

    internal class FillTile
    {
        public Vector2 Location { get; set; }
        public TileState TileState { get; set; }

        public FillTile(Vector2 location)
        {
            this.Location = location;
            this.TileState = this.GetTileState();
        }

        private TileState GetTileState()
        {
            // Doing this so if statements are more readable
            bool canPlant = ModEntry.CanPlantHeldObject(this.Location);
            bool hasObject = Game1.currentLocation.isObjectAtTile((int)this.Location.X, (int)this.Location.Y);
            bool hasAliveCrop = ModEntry.TileContainsAliveCrop(this.Location);
            bool isHoldingSeed = Game1.player.IsHoldingCategory(ModEntry.SeedCategory);
            

            if (canPlant && !hasObject)
                return TileState.Plantable;

            if (hasAliveCrop && isHoldingSeed)
                return TileState.AlreadyPlanted;

            return TileState.NonPlantable;
        }

        public Color GetColor()
        {
            // keep in mind the green selection texture is used, meaning that blue/red colors will look kind of murky
            switch (TileState)
            {
                case TileState.Plantable:
                    return Color.White;

                case TileState.AlreadyPlanted:
                    return Color.Yellow;

                case TileState.NonPlantable:
                    return Color.Transparent;

                default:
                    return new Color(0, 0, 0, 0);
            }
        }
    }
}
