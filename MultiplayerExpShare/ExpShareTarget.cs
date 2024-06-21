using MultiplayerExpShare.Patches;
using StardewValley;
using StardewValley.Menus;
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

    public static bool FarmerHasMastery(Farmer f)
    {
        return f.Level >= 25 && MasteryTrackerMenu.getCurrentMasteryLevel() < 5; // this is used in base game code?
    }

    public bool HasMastery()
    {
        return FarmerHasMastery(m_Farmer);
    }

    public int GetSkillLevel(string skill_name)
    {
        int vanilla_id = AchtuurCore.Utility.Skills.GetSkillIdFromName(skill_name);
        if (vanilla_id != -1)
            return m_Farmer.GetSkillLevel(vanilla_id);
        return ModEntry.Instance.SpaceCoreAPI.GetLevelForCustomSkill(m_Farmer, skill_name);
    }

    public bool CanReceiveExp(string skill_name)
    {
        switch (m_Type)
        {
            case SharerType.Actor:
                return true;
            case SharerType.Receiver:
                return GetSkillLevel(skill_name) < ModEntry.Instance.SkillMaxLevels.Value[skill_name] || HasMastery();
            default:
                return false;
        }
    }

    public bool CanReceiveExpFrom(string skill_name, ExpShareTarget sharer)
    {
        // can only get exp from actors
        if (sharer.m_Type != SharerType.Actor || sharer.MultiplayerID == this.MultiplayerID)
            return false;

        // if you have mastery, and other doesn't you dont get exp from them
        if (HasMastery() && !sharer.HasMastery())
            return false;

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
