using AchtuurCore.Framework;
using AchtuurCore.Framework.Particle;
using Microsoft.Xna.Framework;

namespace MultiplayerExpShare;

internal class MultiplayerExpShareAPI : IMultiplayerExpShareAPI
{
    public void AddSkillTrailParticle(string skill_id, Color color)
    {
        ModEntry.ShareTrailParticles.Add(
            skill_id,
            new TrailParticle(
                ModEntry.Instance.ParticleTrailLength,
                color,
                ModEntry.Instance.ParticleSize
            )
        );
    }
}
