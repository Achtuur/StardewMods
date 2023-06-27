using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;

namespace AchtuurCore;

internal class ModEntry : Mod
{
    internal static ModEntry Instance;
    public override void Entry(IModHelper helper)
    {
        ModEntry.Instance = this;

        HarmonyPatcher.ApplyPatches(this,
            new WateringPatcher()
        );
        Events.EventPublisher.FinishedWateringSoil += this.OnWateredSoil;
    }

    private void OnWateredSoil(object sender, WateringFinishedArgs e)
    {
    }
}
