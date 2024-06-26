﻿using HoverLabels.Framework;
using StardewValley.Buildings;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AchtuurCore.Extensions;
using Microsoft.Xna.Framework.Graphics;
using AchtuurCore.Utility;
using AchtuurCore.Framework;
using StardewValley.Objects;
using HoverLabels.Labels.Objects;
using SObject = StardewValley.Object;
using AchtuurCore.Framework.Borders;

namespace HoverLabels.Labels.Buildings;
internal class JunimoHutLabel : BuildingLabel
{
    JunimoHut hoverHut;
    public JunimoHutLabel(int? priority = null) : base(priority)
    {
    }

    protected override void ResetBorders()
    {
        base.ResetBorders();
        hoverHut = null;
    }
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        // Assume that greenhouse can only be on farm and outdoors
        if (!Game1.currentLocation.IsFarm || !Game1.currentLocation.IsOutdoors)
            return false;

        IEnumerable<JunimoHut> farmHuts = Game1.getFarm().buildings.Where(b => b is JunimoHut).Cast<JunimoHut>();
        return farmHuts.Any(hut => hut.GetRect().Contains(cursorTile));
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        this.hoverHut = this.hoverBuilding as JunimoHut;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();
        Chest hutInventory = hoverHut.GetOutputChest();
        IEnumerable<Item> items = hutInventory.Items.Where(item => item.ParentSheetIndex != SObject.prismaticShardIndex);

        // If difference in length, prismatic shard was filtered out
        if (hutInventory.Items.Count != items.Count())
        {
            ResetBorders();
            AddBorder(new TitleLabel("Junimo Hut (Prismatic)"));
        }
            

        IEnumerable<Item> inventoryContents = ChestLabel.ListInventoryContents(items, ModEntry.IsShowDetailButtonPressed());
        AddBorder(new GridLabel(inventoryContents));

        Border control_border = new Border();
        string showAllMsg = ChestLabel.GetShowAllMessage(hutInventory.Items);
        if (showAllMsg is not null)
            control_border.AddLabel(showAllMsg);

        if (!ModEntry.IsAlternativeSortButtonPressed() && inventoryContents.Count() > 1)
            control_border.AddLabel(I18n.LabelChestAltsort(ModEntry.GetAlternativeSortButtonName()));

        control_border.AddLabel(I18n.LabelShowrange(ModEntry.GetShowDetailButtonName()));
    }

    public override void DrawOnOverlay(SpriteBatch spriteBatch)
    {
        if (!ModEntry.IsShowDetailButtonPressed())
            return;

        IEnumerable<Vector2> hutRange = GetHutRangeRect(hoverHut).GetTiles();
        Overlay.DrawTiles(spriteBatch, hutRange, tileTexture: Overlay.GreenTilePlacementTexture);

    }

    private static Rectangle GetHutRangeRect(JunimoHut hut)
    {
        return new Rectangle(hut.tileX.Value - 7, hut.tileY.Value - 7, 17, 17);
    }
}
