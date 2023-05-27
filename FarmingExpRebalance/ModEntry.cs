using AchtuurCore.Events;
using StardewModdingAPI;
using StardewValley;
using System;

namespace FarmingExpRebalance
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance;
        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;

            AchtuurCore.Events.EventPublisher.onFinishedWateringSoil += onFinishedWateringSoil;
        }

        private void onFinishedWateringSoil(object sender, WateringFinishedArgs e)
        {

            if (e.farmer == Game1.player)
            {
                Game1.player.gainExperience(0, 1);
            }
            //e.farmer.add
        }
    }
}
