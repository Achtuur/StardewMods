using AchtuurCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle.StartBehaviour;
public class RandomStartBehaviour : ParticleStartBehaviour
{
    public RandomStartBehaviour()
    {
    }

    public override void Start()
    {
        base.Start();
        m_Kinematics.Velocity = RandomUtil.GetUnitVector() * Kinematics.c_MaxVelocity;
    }
}
