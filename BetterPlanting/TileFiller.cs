using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using AchtuurCore.Utility;
using StardewValley;
using xTile.Tiles;
using StardewValley.TerrainFeatures;

namespace BetterPlanting
{
    /// <summary>
    /// Modes that describe what way the fill should work
    /// </summary>
    internal enum FillMode
    {
        /// <summary>
        /// Single tile, equal to no fill
        /// </summary>
        Disabled,
        /// <summary>
        /// Three tiles horizontally/vertically, depending on the Farmers orientation w.r.t. the cursor
        /// </summary>
        ThreeInARow,
        /// <summary>
        /// Five tiles horizontally/vertically, depending on the Farmers orientation w.r.t. the cursor
        /// </summary>
        FiveInARow,
        /// <summary>
        /// A square with sides of length 3
        /// </summary>
        ThreeSquare,
        /// <summary>
        /// A square with sides of length 5
        /// </summary>
        FiveSquare,
        /// <summary>
        /// A square with sides of length 7
        /// </summary>
        SevenSquare,
        /// <summary>
        /// Every possible adjacent tile to the selected one.
        /// </summary>
        All,
    }


    /// <summary>
    /// Todo: change name
    /// 
    /// Calculates which tiles to fill based on current fill setting.
    /// </summary>
    internal class TileFiller
    {
        public const int PlacementRange = 2;
        public const int FillModeNumber = 7;
        /// <summary>
        /// Maxmimum number of tiles this filler is allowed to use
        /// </summary>
        public int TileLimit { get; set; }

        private int fillModePointer;

        private FillMode fillMode;

        public TileFiller()
        {
            this.TileLimit = 500;
            this.fillModePointer = 0;
            this.fillMode = FillMode.Disabled;
        }

        public IEnumerable<FillTile> GetFillTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition)
        {
            // Dont return any tiles when out of range
            if ((CursorTilePosition - FarmerTilePosition).LengthSquared() > PlacementRange * PlacementRange)
                yield break;

            IEnumerable<FillTile> fillModeTiles;

            switch (this.fillMode)
            {
                case FillMode.Disabled:
                    fillModeTiles = new List<FillTile>();
                    break;
                case FillMode.ThreeInARow:
                    fillModeTiles = GetRowModeTiles(FarmerTilePosition, CursorTilePosition, 3);
                    break;
                case FillMode.FiveInARow:
                    fillModeTiles = GetRowModeTiles(FarmerTilePosition, CursorTilePosition, 5);
                    break;
                case FillMode.ThreeSquare:
                    fillModeTiles = GetSquareModeTiles(FarmerTilePosition, CursorTilePosition, 3);
                    break;
                case FillMode.FiveSquare:
                    fillModeTiles = GetSquareModeTiles(FarmerTilePosition, CursorTilePosition, 5);
                    break;
                case FillMode.SevenSquare:
                    fillModeTiles = GetSquareModeTiles(FarmerTilePosition, CursorTilePosition, 7);
                    break;
                case FillMode.All:
                    fillModeTiles = GetAllModeTiles(FarmerTilePosition, CursorTilePosition);
                    break;
                default: 
                    fillModeTiles = new List<FillTile>();
                    break;
            }

            foreach (FillTile tile in fillModeTiles)
                yield return tile;
        }

        public void IncrementFillMode()
        {
            if (!ModEntry.PlayerIsHoldingPlantableObject())
                return;

            this.fillModePointer = (this.fillModePointer + 1) % FillModeNumber;
            this.fillMode = (FillMode) this.fillModePointer;

            AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"FillMode: {this.fillMode}");
        }

        public void DecrementFillMode()
        {
            if (!ModEntry.PlayerIsHoldingPlantableObject())
                return;

            this.fillModePointer--;
            if (this.fillModePointer < 0)
                this.fillModePointer += FillModeNumber;

            this.fillMode = (FillMode)this.fillModePointer;
            AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"FillMode: {this.fillMode}");
        }

        public string GetFillModeAsString()
        {
            switch (this.fillMode)
            {
                case FillMode.Disabled:
                    return I18n.FillModeDisabled();
                case FillMode.ThreeInARow:
                    return I18n.FillModeInArow(3);
                case FillMode.FiveInARow:
                    return I18n.FillModeInArow(5);
                case FillMode.ThreeSquare:
                    return I18n.FillModeSquare(3);
                case FillMode.FiveSquare:
                    return I18n.FillModeSquare(5);
                case FillMode.SevenSquare:
                    return I18n.FillModeSquare(7);
                case FillMode.All:
                    return I18n.FillModeAll();
                default: 
                    return "";
            }
        }


        /// <summary>
        /// Get <paramref name="nTiles"/> tiles in the direction of <paramref name="FarmerTilePosition"/> - <paramref name="CursorTilePosition"/>
        /// </summary>
        /// <param name="FarmerTilePosition"></param>
        /// <param name="CursorTilePosition"></param>
        /// <returns></returns>
        private IEnumerable<FillTile> GetRowModeTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition, int nTiles)
        {
            // get vector2 in a cardinal direction
            Vector2 dir = (CursorTilePosition - FarmerTilePosition).toCardinal();
            Vector2 startTile = FarmerTilePosition + dir;

            // dir = 0 (cursor is on the farmers position)
            if (dir.LengthSquared() < 1f)
                dir = VectorHelper.GetFaceDirectionUnitVector(Game1.player.FacingDirection);

            for (int t = 0; t < nTiles; t++)
            {
                Vector2 tile = startTile + t * dir;
                yield return new FillTile(tile);
                //if (ModEntry.CanPlantHeldObject(tile))
                //    yield return tile;
            }
        }

        /// <summary>
        /// Get rows in a square in direction <paramref name="FarmerTilePosition"/> - <paramref name="CursorTilePosition"/> with sides <paramref name="sideLength"/>
        /// </summary>
        /// <param name="FarmerTilePosition"></param>
        /// <param name="CursorTilePosition"></param>
        /// <param name="sideLength"></param>
        /// <returns></returns>
        private IEnumerable<FillTile> GetSquareModeTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition, int sideLength)
        {
            Vector2 dir = (CursorTilePosition - FarmerTilePosition).toCardinal();
            Vector2 center = FarmerTilePosition + dir * (sideLength / 2 + 1);

            yield return new FillTile(center);
            // iterate outwards in rings from center
            // Start at 3 since s=1 will never return anything
            for (int s = 3; s <= sideLength; s+=2)
            {
                // iterate over a single side (except last square)
                // Last square is skipped because mirroring 4 sides takes care of that
                // (the first square of the next mirror is the last one of the previous side)
                for (int i = -s/2; i < s/2; i++)
                {
                    // Top
                    Vector2 top = center + new Vector2(i, -s/2);
                    //if (ModEntry.CanPlantHeldObject(top))
                    //    yield return top;

                    yield return new FillTile(top);

                    // Bottom (starts at bottom right)
                    Vector2 bot = center + new Vector2(-i, s/2);
                    //if (ModEntry.CanPlantHeldObject(bot))
                    //    yield return bot;

                    yield return new FillTile(bot);

                    // Left
                    Vector2 left = center + new Vector2(-s/2, -i);
                    //if (ModEntry.CanPlantHeldObject(left))
                    //    yield return left;

                    yield return new FillTile(left);

                    // Right
                    Vector2 right = center + new Vector2(s/2, i);
                    //if (ModEntry.CanPlantHeldObject(right))
                    //    yield return right;

                    yield return new FillTile(right);

                }
            }
        }

        
        private IEnumerable<FillTile> GetAllModeTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition)
        {
            Vector2 dir = (CursorTilePosition - FarmerTilePosition).toCardinal();
            Vector2 startTile = FarmerTilePosition + dir;

            List<Vector2> tileQueue = new List<Vector2>() { startTile };
            int backPointer = 0;

            Vector2[] card_dir = new Vector2[]
            {
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(0, -1),
                new Vector2(-1, 0),
            };

            // Find all adjacent tiles to startTile that are HoeDirt (and thus possible plantable)
            while (backPointer < tileQueue.Count && tileQueue.Count <= this.TileLimit)
            {
                Vector2 currentTile = tileQueue[backPointer];
                backPointer++;

                foreach (Vector2 d in card_dir)
                {
                    Vector2 newTile = currentTile + d;
                    if ((ModEntry.CanPlantHeldObject(newTile) || ModEntry.TileContainsCrop(newTile)) 
                        && !tileQueue.Contains(newTile))
                        tileQueue.Add(newTile);
                }
            }

            foreach (Vector2 tile in tileQueue)
                yield return new FillTile(tile);
        }


    }
}
