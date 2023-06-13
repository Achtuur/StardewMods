using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace AchtuurCore.Utility
{
    public static class Tiles
    {
        /// <summary>
        /// <para>Get visible tiles, taken from Pathoschild's Tilehelper.GetVisibleTiles</para>
        /// 
        /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/TileHelper.cs#L95"/>
        /// </summary>
        /// <returns></returns>
        public static Rectangle GetVisibleArea(int expand = 0)
        {
            return new Rectangle (
                x: (Game1.viewport.X / Game1.tileSize) - expand,
                y: (Game1.viewport.Y / Game1.tileSize) - expand,
                width: (int)Math.Ceiling(Game1.viewport.Width / (decimal)Game1.tileSize) + (expand * 2),
                height: (int)Math.Ceiling(Game1.viewport.Height / (decimal)Game1.tileSize) + (expand * 2)
            );
        }

        public static IEnumerable<Vector2> GetVisibleTiles(int expand = 0)
        {
            return Tiles.GetVisibleArea(expand).GetTiles();
        }


        /// <summary>
        /// <para> Get tiles in a rectangle. Taken from Pathoschild's TileHelper.GetTiles </para>
        /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/TileHelper.cs#L21"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2> GetTiles(int x, int y, int width, int height)
        {
            for (int curX = x, maxX = x + width - 1; curX <= maxX; curX++)
            {
                for (int curY = y, maxY = y + height - 1; curY <= maxY; curY++)
                    yield return new Vector2(curX, curY);
            }
        }

        public static IEnumerable<Vector2> GetTiles(this Rectangle rect)
        {
            return Tiles.GetTiles(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Checks whether object with <paramref name="object_id"/> is on <paramref name="tile"/>
        /// </summary>
        /// <param name="tile">Tile to check for object</param>
        /// <param name="object_id">Id of object that is checked whether it is on this tile</param>
        /// <returns></returns>
        public static bool ContainsObject(this Vector2 tile, int object_id, GameLocation location=null)
        {
            location ??= Game1.currentLocation;
            SObject obj = location.getObjectAtTile((int)tile.X, (int)tile.Y);
            return obj != null && obj.ParentSheetIndex == object_id;
        }

    }
}
