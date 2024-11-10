using MultiplayerExpShare.Patches;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExpShare;
internal class ExpShare
{
    ExpShareTarget m_Actor;
    List<ExpShareTarget> m_Targets = new List<ExpShareTarget>();
    List<ExpShareTarget> m_ValidTargets = new List<ExpShareTarget>();

    string m_SkillId;
    int m_Amount;

    int m_ActorExp;
    int m_SharedExp;

    public ExpShare(Farmer source, string skill_id, int amount)
    {
        this.m_Actor = new ExpShareTarget(source, SharerType.Actor);
        this.m_SkillId = skill_id;
        this.m_Amount = amount;
    }

    public static ExpShare FromOnlinePlayers(string skill_id, int amount)
    {
        ExpShare expShare = new ExpShare(Game1.player, skill_id, amount);
        expShare.SetTargets(Game1.getOnlineFarmers());
        return expShare;
    }

    public static void ReceiveExp(ExpGainData data)
    {
        if (data.actor_multiplayerid == Game1.player.UniqueMultiplayerID)
            return;

        Farmer nearby_actor = ModEntry.GetFarmerFromMultiplayerID(data.actor_multiplayerid);
        Farmer receiver = ModEntry.GetFarmerFromMultiplayerID(data.nearby_farmer_id);
        ModEntry.SpawnParticles(nearby_actor, receiver, data.skill_id, data.amount);

        AchtuurCore.Logger.DebugLog(
            ModEntry.Instance.Monitor, 
            $"{receiver.Name} received {data.amount} exp in {data.skill_id} from ({nearby_actor.Name})!"
        );


        if (receiver.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
            return;


        if (ModEntry.IsVanillaSkill(data.skill_id))
            GainExperiencePatch.InvokeGainExperience(Game1.player, data);
        else
            SpaceCoreExperiencePatch.InvokeGainExperience(Game1.player, data);
    }

    public void SetTargets(IEnumerable<Farmer> farmers)
    {
        this.m_Targets = farmers
            .Where(f => ModEntry.FarmerIsNearby(f))
            .Select(f => new ExpShareTarget(f, SharerType.Receiver))
            .Where(f => f.MultiplayerID != m_Actor.MultiplayerID)
            .ToList();
        this.m_ValidTargets = m_Targets
            .Where(t => t.CanReceiveExpFrom(m_SkillId, m_Actor))
            .ToList();

        int actor_exp = (int)Math.Ceiling(m_Amount * m_Actor.GetExpPercentage(m_SkillId));
        int shared_exp = m_ValidTargets.Sum(t => t.GetExpAmount(m_SkillId, m_Amount));
        int rounding_loss = m_Amount - (actor_exp + shared_exp);
        m_ActorExp = actor_exp + rounding_loss;
        m_SharedExp = shared_exp;
    }

    /// <summary>
    /// Returns true if sharing exp will actually happen.
    /// 
    /// Returns false if there are no targets, or if GetActorExp() == amount
    /// </summary>
    /// <returns></returns>
    public bool WillShareExp()
    {
        return ModEntry.IsSharingForSkillEnabled(m_SkillId)
            && GetActorExp() != m_Amount
            && m_ValidTargets.Count > 0;
    }

    public int GetTotalTargetExp()
    {
        return m_SharedExp;
    }

    public int GetPerTargetExp()
    {
         return GetTotalTargetExp() / m_ValidTargets.Count;
    }

    public int GetActorExp()
    {
        return m_ActorExp;
    }

    public void ShareExp()
    {
        if (!WillShareExp())
            return;

        int actor_exp = GetActorExp();
        int shared_exp = GetTotalTargetExp();
        AchtuurCore.Logger.DebugLog(
            ModEntry.Instance.Monitor,
            $"({Game1.player.Name}) is sharing exp with {m_ValidTargets.Count} farmer(s): {m_Amount} -> {actor_exp} / {shared_exp}"
        );

        foreach (ExpShareTarget target in m_ValidTargets)
        {
            int shared_amount = ShareExpWithTarget(target);
            ModEntry.SpawnParticles(m_Actor.Farmer, target.Farmer, m_SkillId, shared_amount);
        }
    }

    private int ShareExpWithTarget(ExpShareTarget target)
    {
        int amount = target.GetExpAmount(m_SkillId, m_Amount);
        ExpGainData expdata = new ExpGainData(m_Actor.MultiplayerID, target.MultiplayerID, m_SkillId, amount);
        target.ShareExpWith(expdata);
        return amount;
    }
}

public struct ExpGainData
{
    public long actor_multiplayerid;
    public long nearby_farmer_id;
    public int amount;
    public string skill_id;

    public ExpGainData(long actor_multiplayerid, long nearby_farmer_id, string skill_id, int amount)
    {
        this.actor_multiplayerid = actor_multiplayerid;
        this.nearby_farmer_id = nearby_farmer_id;
        this.skill_id = skill_id;
        this.amount = amount;
    }
}