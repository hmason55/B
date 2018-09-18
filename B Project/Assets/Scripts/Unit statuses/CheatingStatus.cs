using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatingStatus : BaseStatus
{

    public CheatingStatus(int strength, int duration, BaseUnit owner) :base( strength,  duration,  owner,  null)
    {
        Icon = Resources.Load<Sprite>("Sprites/Icons/lowthreat");

        owner.AddLinkEvent(ExecuteLinkEffect);
    }
    
    public override string GetDescription()
    {
        string msg = "Unit is taunted by " + Owner.UnitName + " for " + Duration + " turn";
        if (Duration > 1)
            msg += "s";
        return msg;
    }

    public override void StartTurnExecute()
    {
        Duration--;
    }

    public override void Update(BaseStatus newStatus)
    {
        // Update duration
        Duration = newStatus.Duration;
    }

    public BaseUnit GetTauntTarget()
    {
        return Owner;
    }

    public override void EndTurnExecute()
    {
    }

    public override void DestroyStatusExecute()
    {
        Owner.RemoveLinkEvent(ExecuteLinkEffect);
    }

    public override void ExecuteLinkEffect()
    {
        ResourceManager.Instance.AddNextTurnResource(Strength, Owner);
    }
}