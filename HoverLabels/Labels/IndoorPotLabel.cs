using AchtuurCore.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels;
internal class IndoorPotLabel : ObjectLabel
{
    IndoorPot hoverPot;
    HoeDirt hoverHoeDirt;
    Crop hoverPotCrop;

    public IndoorPotLabel(int? priority=null) : base(priority)
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

        this.hoverPot = this.hoverObject as IndoorPot;
        this.hoverHoeDirt = this.hoverPot.hoeDirt.Value;
        this.hoverPotCrop = this.hoverHoeDirt.crop;

        this.GenerateCropLabel();
        this.GenerateFertilizerLabel();
        this.GenerateSoilStateLabel();
    }

    private void GenerateSoilStateLabel()
    {
        if (this.hoverPotCrop is null || CropLabel.IsCropFullyGrown(this.hoverPotCrop))
            return;

        if (this.hoverHoeDirt.state.Value == 0 && !this.hoverPotCrop.dead.Value)
            this.Description.Add(I18n.LabelCropsWaterNeeded());
    }

    private void GenerateFertilizerLabel()
    {
        string fertilizerName = CropLabel.GetFertilizerName(hoverHoeDirt.fertilizer.Value);
        if (fertilizerName.Length > 0)
            this.Description.Add(I18n.LabelCropsFertilizer(fertilizerName));
    }

    private void GenerateCropLabel()
    {
        if (hoverPotCrop is null)
        {
            return;
        }

        SObject harvestedItem = hoverPotCrop.programColored.Value ? new ColoredObject(hoverPotCrop.indexOfHarvest.Value, 1, hoverPotCrop.tintColor.Value) : new SObject(hoverPotCrop.indexOfHarvest.Value, 1, false, -1, 0);
        this.Name += $" ({harvestedItem.DisplayName})";

            if(CropLabel.IsCropFullyGrown(this.hoverPotCrop))
        {
            Description.Add(I18n.LabelCropsReadyHarvest());
            if (hoverPotCrop.minHarvest.Value == hoverPotCrop.maxHarvest.Value)
            {
                Description.Add(I18n.LabelCropsHarvestAmount(hoverPotCrop.minHarvest.Value));
            }
            else
            {
                Description.Add(I18n.LabelCropsHarvestRange(hoverPotCrop.minHarvest.Value, hoverPotCrop.maxHarvest.Value));
            }
        }
        else if (hoverPotCrop.dead.Value)
        {
            Description.Add(I18n.LabelCropsDead());
        }
        // Not fully grown yet
        else
        {
            int days = CropLabel.GetDaysUntilFullyGrown(hoverPotCrop);
            string readyDate = ModEntry.GetDateAfterDays(days);

            if (CropLabel.CropCanFullyGrowInTime(hoverPotCrop))
                Description.Add(I18n.LabelCropsGrowTime(days, readyDate));
            else
                Description.Add(I18n.LabelCropsInsufficientTime(days));
        }
    }
}
