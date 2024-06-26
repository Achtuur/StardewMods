﻿using AchtuurCore.Framework.Borders;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Objects;
internal class FishTankLabel : ObjectLabel
{
    FishTankFurniture hoverFishTank;
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = ObjectLabel.GetCursorObject(cursorTile);
        return sobj is not null && sobj is FishTankFurniture;
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        hoverFishTank = this.hoverObject as FishTankFurniture;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        Dictionary<Item, TankFish> fishLookup = AccessTools.Field(typeof(FishTankFurniture), "_fishLookup").GetValue(this.hoverFishTank) as Dictionary<Item, TankFish>;

        if (fishLookup.Count() <= 0)
            return;

        AddBorder(I18n.Contains());

        foreach(Item f in fishLookup.Keys.OrderBy(f => f.DisplayName))
        {
            AppendLabelToBorder(new ItemLabel(f));
        }
    }
}
