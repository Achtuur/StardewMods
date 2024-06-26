﻿using AchtuurCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle.StartBehaviour;
public class EdgeOfMapStartBehaviour : ParticleStartBehaviour
{
    public EdgeOfMapStartBehaviour()
    {
    }

    public override void Start()
    {
        base.Start();
        Ellipse screen = Tiles.GetVisibleAreaEllipse(expand: 3);
        // Start at random edge of screen
        //m_Kinematics.InitialPosition = RandomUtil.GetPointOnEllipse(screen);
        //m_Kinematics.Position = m_Kinematics.InitialPosition;

        float angle = RandomUtil.GetFloat(-0.1, 0.1) - (float)Math.PI / 2f;
        m_Kinematics.InitialPosition = screen.PointAt(angle);
        m_Kinematics.Position = m_Kinematics.InitialPosition;

    }
}
