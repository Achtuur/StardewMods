using AchtuurCore.Patches;
using HarmonyLib;
using System;

namespace BetterRods.Patches;

internal class TimeUntilFishingBitePatch : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: this.GetOriginalMethod<StardewValley.Tools.FishingRod>("calculateTimeUntilFishingBite"),
            postfix: this.GetHarmonyMethod(nameof(Postfix_calculateTimeUntilFishingBite))
        );
    }


    private static void Postfix_calculateTimeUntilFishingBite(ref float __result)
    {
        try
        {
            __result *= ModEntry.GetNibbleTimeMultiplier();
        }
        catch (Exception e)
        {
            AchtuurCore.Logger.ErrorLog(ModEntry.Instance.Monitor, $"Something went wrong in postfix patch of calculateTimeUntilFishingBite:\n{e}");
        }
    }
}
