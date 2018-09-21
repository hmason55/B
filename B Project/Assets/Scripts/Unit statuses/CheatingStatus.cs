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
        string msg = "Gain " + Strength + " resource in the next turn for each attack made";        
        return msg;
    }

    public override void StartTurnExecute()
    {
        Duration--;
    }

    public override void Update(BaseStatus newStatus)
    {
        // Update duration and strength
        Duration = newStatus.Duration;
        Strength += newStatus.Strength;
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