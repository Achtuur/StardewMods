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
    /// <summary>
    /// State of a tile
    /// 
    /// <remark>IMPORTANT: keep Plantable as the first tilestate, since TileFiller sorts based on state</remark>
    /// </summary>
    public enum TileState
    {
        Plantable,
        NotEnoughSeeds,
        AlreadyPlanted,
        NonPlantable,
    }

    internal class FillTile
    {
        /// <summary>
        /// Tile location of this <see cref="FillTile"/> in <see cref="Game1.currentLocation"/>
        /// </summary>
        public Vector2 Location { get; set; }

        /// <summary>
        /// <see cref="TileState"/> of this tile. Used to determine overlay color and whether it should be taken into account for planting
        /// </summary>
        public TileState State { get; set; }

        /// <summary>
        /// Priority this tile has. Tiles with higher priority will be chosen over tiles with lower priority if there are not enough seeds for the total shape.
        /// </summary>
        public int Priority { get; set; }

        public FillTile(Vector2 location, int priority=0)
        {
            this.Location = location;
            this.Priority = priority;
            this.State = this.DetermineTileState();
        }

        private TileState DetermineTileState()
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
            switch (State)
            {
                case TileState.Plantable:
                    return Color.White;

                case TileState.AlreadyPlanted:
                    return Color.Yellow;

                case TileState.NotEnoughSeeds:
                    return Color.Red;

                case TileState.NonPlantable:
                    return Color.Transparent;

                default:
                    return new Color(0, 0, 0, 0);
            }
        }
    }
}
