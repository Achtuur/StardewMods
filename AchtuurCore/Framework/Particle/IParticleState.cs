﻿using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle;
public abstract class IParticleState
{

    public abstract void Start();
    public abstract void Update();
    public abstract void Reset();
    public abstract bool IsFinished();

    public abstract Kinematics GetKinematics();

    public abstract void SetKinematics(Kinematics kinematics);

    public virtual void SetInitialPosition(Vector2 position)
    {
        Kinematics kin = GetKinematics();
        kin.InitialPosition = position;
        kin.Position = position;
        SetKinematics(kin);
    }
    public virtual void SetTargetPosition(Vector2 position)
    {
        Kinematics kin = GetKinematics();
        kin.TargetPosition = position;
        SetKinematics(kin);
    }
    public virtual void SetTargetFarmer(Farmer farmer)
    {
        Kinematics kin = GetKinematics();
        kin.TargetFarmer = farmer;
        SetKinematics(kin);
    }

    public virtual Farmer GetTargetFarmer()
    {
        return GetKinematics().TargetFarmer;
    }

    public virtual Vector2 GetInitialPosition()
    {
        return GetKinematics().InitialPosition;
    }

    public virtual Vector2 GetTargetPosition()
    {
        if (GetTargetFarmer() != null)
        {
            return GetTargetFarmer().Position + Kinematics.c_FarmerPositionCorrection;
        }
        return GetKinematics().TargetPosition;
    }

    public Vector2 Position => GetKinematics().Position;
    public Vector2 Velocity => GetKinematics().Velocity;
    public Vector2 Acceleration => GetKinematics().Acceleration;

}
