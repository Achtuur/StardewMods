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

namespace HoverLabels;
internal class ObjectLabel : BaseLabel
{
    private SObject hoverObject;
    public ObjectLabel(Vector2 cursorTile) : base(cursorTile)
    {
    }

    /// <inheritdoc/>
    protected override void GenerateLabel()
    {
        if (!Game1.currentLocation.isObjectAtTile(CursorTile))
            return;

        this.hoverObject = Game1.currentLocation.getObjectAtTile(CursorTile);

        GenerateObjectLabel();

        // is machine -> generate machine label
        if (hoverObject.MinutesUntilReady > 0 || hoverObject.readyForHarvest.Value)
            GenerateMachineLabel();


    }
    /// <summary>
    /// Generate label for generic object, 
    /// </summary>
    private void GenerateObjectLabel()
    {
        this.Name = hoverObject.DisplayName;
    }

    private void GenerateMachineLabel()
    {
        SObject processingItem = hoverObject.heldObject.Value;
        if (hoverObject.readyForHarvest.Value)
        {
            //display either "yx" (where y is number of items > 2) or ""
            string amt = (processingItem.Stack > 1) ? $"{processingItem.Stack}x " : String.Empty;
            this.Description.Add($"{amt}{processingItem.DisplayName} ready!");
        }
        else
        {
            this.Description.Add($"Crafting {processingItem.DisplayName}");
            this.Description.Add($"Ready in {GetDurationUntilReadyString(hoverObject.MinutesUntilReady)}");
        }
    }

    private string GetDurationUntilReadyString(int minutes)
    {
        //days in stardew are 1600 minutes and not 1440 minutes. Hours from 2am to 6am are 100 minutes long for whatever reason.
        int days = (minutes >= 1600) ? minutes / 1600 : 0;
        minutes %= 1600;
        int hours = (minutes >= 60) ? minutes / 60 : 0;
        minutes %= 60;

        string time = "";

        if (days > 0)
        {
            time += $"{days}d" + ((minutes > 0 || hours > 0) ? " " : "");
        }

        if (hours > 0)
        {
            time += $"{hours}h" + ((minutes > 0) ? " " : "");
        }

        if (minutes > 0 || (days == 0 && hours == 0))
        {
            time += $"{minutes}m";
        }
        return time;
    }

}
