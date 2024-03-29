﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Labels;
internal class ExampleLabel : IHoverLabel
{
    public int Priority { get; set; } = -1;

    public void DrawOnOverlay(SpriteBatch spriteBatch)
    {
    }

    public void UpdateCursorTile(Vector2 cursorTile)
    {
    }
    public bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return Game1.currentLocation.terrainFeatures.ContainsKey(cursorTile)
            && Game1.currentLocation.terrainFeatures[cursorTile] is HoeDirt;
    }

    public string GetName()
    {
        return "Tilled dirt";
    }

    public IEnumerable<string> GetDescription()
    {
        yield return "This tile has been hoe'd";
        yield return "Isn't that cool?";
    }

}
