using AchtuurCore.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;

namespace MultiplayerExpShare.Patches;

public abstract class BaseExpPatcher : GenericPatcher
{
    /// <summary>
    /// Whether the current (patched) method is processing shared exp. If it is not, then exp should be shared
    /// </summary>
    protected static bool isProcessingSharedExp;

    public override void Patch(Harmony harmony)
    {
        isProcessingSharedExp = false;

        harmony.Patch(
            original: this.GetOriginalMethod<Farmer>(nameof(Farmer.gainExperience)),
            postfix: this.GetHarmonyMethod(nameof(Postfix_GainExperience))
        );
    }

    [HarmonyPriority(Priority.Last)]
    private static void Postfix_GainExperience()
    {
        if (isProcessingSharedExp)
        {
            isProcessingSharedExp = false;
        }
    }

    protected static int ShareExpWithOnlinePlayers(string skill_name, int amount)
    {
        if (!CanExpBeShared())
            return 0;

        ExpShare exp_share = ExpShare.FromOnlinePlayers(skill_name, amount);
        int actor_exp = exp_share.GetActorExp();
        exp_share.ShareExp();
        return actor_exp;
    }


    /// <summary>
    /// Returns true if exp can be shared based on <see cref="isProcessingSharedExp"/>
    /// </summary>
    /// <returns></returns>
    protected static bool CanExpBeShared()
    {
        // Skip execution if world isnt loaded
        if (!Context.IsWorldReady)
            return false;

        // If processing shared exp, then 'howMuch' already contains correct exp to add and no message should be sent
        if (isProcessingSharedExp)
            return false;

        return true;
    }
}
