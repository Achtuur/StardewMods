using HarmonyLib;
using StardewValley;
using System;
using System.Linq;

namespace MultiplayerExpShare.Patches;


public class GainExperiencePatch : BaseExpPatcher
{
    public override void Patch(Harmony harmony)
    {

        base.Patch(harmony);

        harmony.Patch(
            original: this.GetOriginalMethod<Farmer>(nameof(Farmer.gainExperience)),
            prefix: this.GetHarmonyMethod(nameof(Prefix_GainExperience))
        );

    }

    /// <summary>
    /// Calls <see cref="Farmer.gainExperience(int, int)"/> and sets <see cref="isProcessingSharedExp"/>. Use this method instead of <see cref="Farmer.gainExperience(int, int)"/> when working with shared exp
    /// </summary>
    /// <param name="farmer"></param>
    /// <param name="which"></param>
    /// <param name="howMuch"></param>
    /// <param name="isSharedExp"></param>
    public static void InvokeGainExperience(Farmer farmer, ExpGainData exp_data)
    {
        isProcessingSharedExp = true;
        int skill = AchtuurCore.Utility.Skills.GetSkillIdFromName(exp_data.skill_id);
        if (skill < 0 || skill > 5)
            return;
        farmer.gainExperience(skill, exp_data.amount);
    }

    private static void Prefix_GainExperience(int which, ref int howMuch, Farmer __instance)
    {
        if (!CanExpBeShared())
            return;

        string skill_name = AchtuurCore.Utility.Skills.GetSkillNameFromId(which);
        howMuch = BaseExpPatcher.ShareExpWithOnlinePlayers(skill_name, howMuch);
    }
}
