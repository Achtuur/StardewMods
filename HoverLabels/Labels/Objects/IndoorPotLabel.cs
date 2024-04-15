using AchtuurCore.Extensions;
using HarmonyLib;
using HoverLabels.Drawing;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Objects;
internal class IndoorPotLabel : ObjectLabel
{
    IndoorPot hoverPot;
    HoeDirt hoverHoeDirt;
    Crop hoverPotCrop;

    public IndoorPotLabel(int? priority = null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        if (Game1.currentLocation.isObjectAtTile(cursorTile))
        {
            SObject sobj = Game1.currentLocation.getObjectAtTile(cursorTile);
            return sobj is not null && sobj is IndoorPot;
        }
        return false;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        hoverPot = hoverObject as IndoorPot;
        hoverHoeDirt = hoverPot.hoeDirt.Value;
        hoverPotCrop = hoverHoeDirt.crop;

        if (hoverPotCrop is null || hoverObject is null)
        {
            return;
        }

        ResetBorders();
        SObject harvestedItem = hoverPotCrop.programColored.Value ? new ColoredObject(hoverPotCrop.indexOfHarvest.Value, 1, hoverPotCrop.tintColor.Value) : new SObject(hoverPotCrop.indexOfHarvest.Value, 1, false, -1, 0);
        string title = $"{hoverObject.DisplayName} ({harvestedItem.DisplayName})";
        AddBorder(new TitleLabelText(title));

        NewBorder();
        GenerateCropLabel();
        GenerateFertilizerLabel();
        GenerateSoilStateLabel();
    }

    private void GenerateSoilStateLabel()
    {
        if (hoverPotCrop is null || CropLabel.IsCropFullyGrown(hoverPotCrop))
            return;

        Item watering_can = Game1.player.Items.Where(item => item is WateringCan).FirstOrDefault();
        if (watering_can is null)
            watering_can = ItemRegistry.Create("(T)WateringCan");
        if (this.hoverHoeDirt.state.Value == 0 && !this.hoverPotCrop.dead.Value)
            //AppendLabelToBorder(I18n.LabelCropsWaterNeeded());
            //AppendLabelToBorder(new ItemLabelText(watering_can, I18n.LabelCropsWaterNeeded()));
            AddBorder(new ItemLabelText(watering_can, I18n.LabelCropsWaterNeeded()));
    }

    private void GenerateFertilizerLabel()
    {
        string fertilizerQID = CropLabel.GetFertilizerQID(hoverHoeDirt.fertilizer.Value);
        if (fertilizerQID.Length > 0)
            //AppendLabelToBorder(new ItemLabelText(fertilizerQID));
            AddBorder(new ItemLabelText(fertilizerQID));
    }

    private void GenerateCropLabel()
    {
        //fully grown crop
        if (CropLabel.IsCropFullyGrown(hoverPotCrop))
        {
            AppendLabelToBorder(I18n.LabelCropsReadyHarvest());
        }
        // dead crop
        else if (hoverPotCrop.dead.Value)
        {
            AppendLabelToBorder(I18n.LabelCropsDead());
        }
        // Not fully grown yet
        else
        {
            int days = CropLabel.GetDaysUntilFullyGrown(hoverPotCrop);
            string readyDate = ModEntry.GetDateAfterDays(days);

            if (CropLabel.CropCanFullyGrowInTime(hoverPotCrop, hoverHoeDirt))
                AppendLabelToBorder(I18n.LabelCropsGrowTime(days, readyDate));
            else
                AppendLabelToBorder(I18n.LabelCropsInsufficientTime(days));
        }
    }
}
