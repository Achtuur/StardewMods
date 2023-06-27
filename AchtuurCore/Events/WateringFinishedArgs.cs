using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace AchtuurCore.Events;

public class WateringFinishedArgs : EventArgs
{
    public Farmer farmer;
    public HoeDirt target;
    public WateringFinishedArgs(Farmer _farmer, HoeDirt _target)
    {
        this.farmer = _farmer;
        this.target = _target;
    }
}
