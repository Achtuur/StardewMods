using MultiplayerExpShare.Patches;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace MultiplayerExpShare;

internal enum SharerType
{
    Actor,
    Receiver,
}
internal class ExpShareTarget
{
    Farmer m_Farmer;
    SharerType m_Type;

    public Farmer Farmer => m_Farmer;
    public long MultiplayerID => m_Farmer.UniqueMultiplayerID;
    public ExpShareTarget(Farmer farmer, SharerType sharer_type)
    {
        m_Farmer = farmer;
        m_Type = sharer_type;
    }
    public static bool PlayerHasMastery()
    {
        return new ExpShareTarget(Game1.player, SharerType.Actor).HasMastery();
    }

    public static int GetFarmerMasteryLevel(Farmer f)
    {
        uint mastery_exp = f.stats.Get("MasteryExp");
        int level = 0;
        for(int i = 1; i <= 5; i++)
        {
            if (mastery_exp >= MasteryTrackerMenu.getMasteryExpNeededForLevel(i))
                level++;
        }
        return level;

    }

    public static bool FarmerHasMastery(Farmer f)
    {
        return f.Level >= 25 && GetFarmerMasteryLevel(f) < 5; // this is used in base game code?
    }

    public bool HasMastery()
    {
        return FarmerHasMastery(m_Farmer);
    }

    public int GetSkillLevel(string skill_id)
    {
        if (!ModEntry.IsVanillaSkill(skill_id))
            return ModEntry.Instance.SpaceCoreAPI.GetLevelForCustomSkill(m_Farmer, skill_id);

        int vanilla_id = AchtuurCore.Utility.Skills.GetSkillIdFromName(skill_id);
        return m_Farmer.GetSkillLevel(vanilla_id);
    }

    public bool CanReceiveExp(string skill_id)
    {
        switch (m_Type)
        {
            case SharerType.Actor:
            case SharerType.Receiver:
                return GetSkillLevel(skill_id) < ModEntry.Instance.SkillMaxLevels.Value[skill_id] || HasMastery();
            default:
                return false;
        }
    }

    public bool IsMaxLevel(string skill_id)
    {
        return GetSkillLevel(skill_id) >= ModEntry.Instance.SkillMaxLevels.Value[skill_id] && MasteryTrackerMenu.getCurrentMasteryLevel() == 5;
    }

    public bool CanReceiveExpFrom(string skill_name, ExpShareTarget actor)
    {
        // can only get exp from actors
        if (actor.m_Type != SharerType.Actor || actor.MultiplayerID == this.MultiplayerID)
            return false;

        // if actor is max level, always share if receiver can take it
        if (actor.IsMaxLevel(skill_name))
            return CanReceiveExp(skill_name);

        // if actor has no mastery and receiver does, only share if actor cant get exp
        if (HasMastery() && !actor.HasMastery())
            return !actor.CanReceiveExp(skill_name) && CanReceiveExp(skill_name);


        return CanReceiveExp(skill_name);
    }

    public float GetExpPercentage(string skill_name)
    {
        if (!CanReceiveExp(skill_name))
            return 0f;

        switch (m_Type)
        {
            case SharerType.Actor:
                return ModEntry.Instance.Config.ExpPercentageToActor;
            case SharerType.Receiver:
                return 1f - ModEntry.Instance.Config.ExpPercentageToActor;
            default:
                return 0f;
        }
    }

    public int GetExpAmount(string skill_name, int amount)
    {
        return (int)Math.Floor(GetExpPercentage(skill_name) * amount);
    }

    public void ShareExpWith(ExpGainData expdata)
    {
        ModEntry.Instance.Helper.Multiplayer.SendMessage<ExpGainData>(expdata, ModEntry.ExpShareMessage, modIDs: new[] { ModEntry.Instance.ModManifest.UniqueID });

    }
}
