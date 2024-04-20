﻿using AchtuurCore.Framework.Borders;
using FishCatalogue.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace FishCatalogue.Drawing;
internal class FishHUD
{
    static Color UndiscoveredColor = new Color(0, 0, 0, 50);
    static Color UncaughtColor = new Color(72, 72, 72, 150);

    BorderDrawer borderDrawer;
    public FishHUD()
    {
        borderDrawer = new();
    }

    public void Draw(SpriteBatch sb, Vector2 position, float alpha)
    {
        borderDrawer.Reset();
        borderDrawer.AddBorder(CreateBorders());
        borderDrawer.Draw(sb, position);
    }

    private IEnumerable<Border> CreateBorders()
    {
        yield return CreateAvailableFishBorder();
        yield return CreateUnavailableFishBorder();
    }

    private Border CreateAvailableFishBorder()
    {
        IEnumerable<ItemLabel> itemLabels = FishCatalogue
            .GetCurrentlyAvailableFish()
            .Select(fish => GenerateItemLabel(fish));

        GridLabel grid_label = new(itemLabels);
        grid_label.SetNumberOfColumns(ModEntry.Instance.Config.HUD_Columns);
        return new Border(grid_label);
    }

    private Border CreateUnavailableFishBorder()
    {
        IEnumerable<IEnumerable<Label>> itemLabels = FishCatalogue
            .GetCurrentLocationFish()
            .Except(FishCatalogue.GetCurrentlyAvailableFish())
            .Where(fish => fish.CanBeCaughtThisSeason())
            .Select(fish => fish.GenerateUnfulfilledConditionLabel());

        // Make sure all labels have the same number of columns
        // To align the grid
        int max_columns = itemLabels.Max(label => label.Count());
        itemLabels = itemLabels.Select(label => label
        .Concat(Enumerable.Repeat(new EmptyLabel(), max_columns - label.Count())));

        GridLabel grid_label = new(itemLabels.SelectMany(l => l));
        grid_label.SetNumberOfColumns(max_columns);
        return new Border(grid_label);
    }

    private ItemLabel GenerateItemLabel(FishData fish)
    {
        ItemLabel itemLabel = new ItemLabel(fish.FishItem);

        // fish caught -> dont do anything special
        if (fish.IsCaughtByPlayer()) 
            return itemLabel;

        if (ModEntry.Instance.Config.HideUncaughtFish)
        {
            itemLabel.SetText("???");
            itemLabel.SetColor(UndiscoveredColor);
        } 
        else
        {
            itemLabel.SetColor(UncaughtColor);
        }
        if (!ModEntry.Instance.Config.ShowFishNames)
            itemLabel.HideDescription();

        return itemLabel;
    }
}