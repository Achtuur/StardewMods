using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            bool hasCrop = Game1.currentLocation.isCropAtTile((int)this.Location.X, (int)this.Location.Y);

            if (canPlant && !hasObject)
                return TileState.Plantable;

            if (hasCrop)
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
                    return Color.LightGray;

                case TileState.NonPlantable:
                    return Color.Transparent;

                default:
                    return new Color(0, 0, 0, 0);
            }
        }
    }
}
