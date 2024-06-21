using HarmonyLib;
using StardewValley;
using System;
using System.Linq;
using System.Reflection;

namespace MultiplayerExpShare.Patches;

/// <summary>
/// Patch for <see cref="SpaceCore.Skills.AddExperience(Farmer, string, int)"/> to support exp sharing for SpaceCore based skills
/// </summary>
public class SpaceCoreExperiencePatch : BaseExpPatcher
{

    public override void Patch(Harmony harmony)
    {
        if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
            return;

        base.Patch(harmony);

        Type spacecore_skills = AccessTools.TypeByName("SpaceCore.Skills");
        MethodInfo addExp_method = AccessTools.Method(spacecore_skills, "AddExperience");

        harmony.Patch(
            original: this.GetOriginalMethod(spacecore_skills, addExp_method.Name),
            prefix: this.GetHarmonyMethod(nameof(Prefix_AddExperienceSpaceCore))
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
        ModEntry.Instance.SpaceCoreAPI.AddExperienceForCustomSkill(farmer, exp_data.skill_id, exp_data.amount);
    }

    [HarmonyPriority(Priority.Last)]
    private static void Postfix_GainExperience()
    {
        if (isProcessingSharedExp)
        {
            isProcessingSharedExp = false;
        }
    }

    private static void Prefix_AddExperienceSpaceCore(Farmer farmer, string skillName, ref int amt)
    {
        if (!CanExpBeShared())
            return;

        amt = BaseExpPatcher.ShareExpWithOnlinePlayers(skillName, amt);
    }
}
