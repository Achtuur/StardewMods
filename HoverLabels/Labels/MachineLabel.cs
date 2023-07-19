using AchtuurCore.Extensions;
using HoverLabels.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;
using StardewValley.Objects;

namespace HoverLabels.Labels;
internal class MachineLabel : ObjectLabel
{
    public MachineLabel(int? priority = null) : base(priority)
    {
    }

    /// <summary>
    /// Returns whether a new label should be created based on <paramref name="cursorTile"/>
    /// </summary>
    /// <param name="cursorTile"></param>
    /// <returns></returns>
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = ObjectLabel.GetCursorObject(cursorTile);

        return sobj is not null && 
            (sobj.MinutesUntilReady > 0 || sobj.readyForHarvest.Value);
    }

    /// <inheritdoc/>
    public override void GenerateLabel()
    {
        base.GenerateLabel();

        SObject processingItem = hoverObject.heldObject.Value;

        if (processingItem is null)
            return;

        if (hoverObject.readyForHarvest.Value)
        {
            //display either "yx" (where y is number of items > 2) or ""
            string amt = processingItem.Stack > 1 ? $"{processingItem.Stack}x " : string.Empty;
            if (processingItem.Stack == 1)
                Description.Add(I18n.LabelMachineSingleItemReady(processingItem.DisplayName));
            else
                Description.Add(I18n.LabelMachineMultipleItemsReady(processingItem.DisplayName, processingItem.Stack));

            string quality_string = GetQualityString(processingItem.Quality);
            Description.Add(I18n.LabelMachineQuality(quality_string));
        }
        else
        {
            string duration = GetDurationUntilReadyString();
            Description.Add(I18n.LabelMachineCrafting(processingItem.DisplayName));
            Description.Add(I18n.LabelMachineReadyIn(duration));
        }
    }

    private string GetQualityString(int quality)
    {
        switch (quality)
        {
            case 0: return I18n.NormalQuality();
            case 1: return I18n.SilverQuality();
            case 2: return I18n.GoldQuality();
            case 4: return I18n.IridiumQuality();
            default: return I18n.NormalQuality();
        }
    }

    private string GetDurationUntilReadyString()
    {
        if (hoverObject is Cask cask)
        {
            return GetCaskDurationString(cask);
        }
        return GetTimeString(hoverObject.MinutesUntilReady);
    }

    private string GetCaskDurationString(Cask cask)
    {
        // get days needed to reach next quality
        int nextQuality = cask.GetNextQuality(cask.heldObject.Value.Quality);
        float daysToMature = cask.daysToMature.Value - cask.GetDaysForQuality(nextQuality);
        float aging_rate = cask.agingRate.Value;
        int days = (int)Math.Ceiling(daysToMature / aging_rate);

        string readyDate = ModEntry.GetDateAfterDays(days);
        return $"{days}d ({readyDate})";
    }

    private string GetTimeString(int minutes)
    {
        //days in stardew are 1600 minutes and not 1440 minutes. Hours from 2am to 6am are 100 minutes long for whatever reason.
        int days = minutes >= 1600 ? minutes / 1600 : 0;
        minutes %= 1600;
        int hours = minutes >= 60 ? minutes / 60 : 0;
        minutes %= 60;

        string time = "";

        if (days > 0)
        {
            time += $"{days}d" + (minutes > 0 || hours > 0 ? " " : "");
        }

        if (hours > 0)
        {
            time += $"{hours}h" + (minutes > 0 ? " " : "");
        }

        if (minutes > 0 || days == 0 && hours == 0)
        {
            time += $"{minutes}m";
        }
        return time;
    }

}
