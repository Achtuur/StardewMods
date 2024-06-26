﻿using AchtuurCore.Framework;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Objects;
internal class ScarecrowLabel : ObjectLabel
{
    public ScarecrowLabel(int? priority = null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = GetCursorObject(cursorTile);
        return sobj is not null &&
            sobj.IsScarecrow();
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();
        AddBorder(I18n.LabelScarecrowCropsProtected(GetScarecrowCrops().Count()));
        AddBorder(I18n.LabelShowrange(ModEntry.GetShowDetailButtonName()));
    }

    public override void DrawOnOverlay(SpriteBatch spriteBatch)
    {
        if (ModEntry.IsShowDetailButtonPressed())
        {
            IEnumerable<Vector2> ScarecrowRange = GetScarecrowRange().Intersect(Tiles.GetVisibleTiles());
            Overlay.DrawTiles(spriteBatch, ScarecrowRange, tileTexture: Overlay.GreenTilePlacementTexture);
        }

        IEnumerable<Vector2> ScarecrowCrops = GetScarecrowCrops().Intersect(Tiles.GetVisibleTiles());
        Overlay.DrawTiles(spriteBatch, ScarecrowCrops, tileTexture: Overlay.GreenTilePlacementTexture);
    }

    private IEnumerable<Vector2> GetScarecrowCrops()
    {
        foreach (Vector2 tile in GetScarecrowRange())
        {
            if (!Game1.currentLocation.terrainFeatures.ContainsKey(tile))
                continue;

            if (Game1.currentLocation.terrainFeatures[tile] is HoeDirt hoeDirt)
            {
                if (hoeDirt.crop is not null && !hoeDirt.crop.dead.Value)
                    yield return tile;
            }
        }
    }

    private IEnumerable<Vector2> GetScarecrowRange()
    {
        Vector2 pos = hoverObject.TileLocation;
        int radius = hoverObject.GetRadiusForScarecrow();

        if (radius <= 0)
            yield break;

        // scarecrow range is somehow rounded down?? idfk
        foreach (Vector2 tile in Tiles.GetTilesInRadius(pos, radius + 1))
        {
            if (Math.Floor(Vector2.Distance(tile, pos)) <= radius)
                yield return tile;
        }
    }
}
