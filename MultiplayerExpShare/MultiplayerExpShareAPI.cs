using AchtuurCore.Framework;
using Microsoft.Xna.Framework;

namespace MultiplayerExpShare;

internal class MultiplayerExpShareAPI : IMultiplayerExpShareAPI
{
    public void AddSkillTrailParticle(string skill_id, Color color)
    {
        ModEntry.ShareTrailParticles.Add(
            skill_id,
            new TrailParticle(
                Vector2.Zero,
                Vector2.Zero,
                ModEntry.Instance.ParticleTrailLength,
                color,
                ModEntry.Instance.ParticleSize
            )
        );
    }
}
