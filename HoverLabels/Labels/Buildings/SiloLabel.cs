﻿using AchtuurCore.Extensions;
using AchtuurCore.Framework.Borders;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Labels.Buildings;
internal class SiloLabel : BuildingLabel
{
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return Game1.currentLocation.IsFarm
            && Game1.currentLocation.IsOutdoors
            && Game1.getFarm().buildings.Any(b => b.buildingType.Value == "Silo" && b.GetRect().Contains(cursorTile));
    }
    public override void GenerateLabel()
    {
        base.GenerateLabel();
        int maxHay = 0;
        Utility.ForEachLocation((loc) => {
            maxHay += loc.GetHayCapacity();
            return true;
        });
        Item hay = ItemRegistry.Create("178", amount: Game1.getFarm().piecesOfHay.Value);
        AddBorder(new ItemLabel(hay, I18n.LabelSiloMaxHay(maxHay)));
    }
}
