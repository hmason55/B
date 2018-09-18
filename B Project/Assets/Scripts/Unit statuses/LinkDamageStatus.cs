using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkDamageStatus : BaseStatus
{
    public LinkDamageStatus(int strength, int duration, BaseUnit owner) :base( strength,  duration,  owner,  null)
    {
        Icon = Resources.Load<Sprite>("Sprites/Icons/lowthreat");
    }

    public override void DestroyStatusExecute()
    {
    }

    public override void EndTurnExecute()
    {
    }

    public override void ExecuteLinkEffect()
    {
    }

    public override string GetDescription()
    {
        string msg = "Increase any source of damage by " +Strength + " for " + Duration + " turn";
        if (Duration > 1)
            msg += "s";
        return msg;
    }

    public override void StartTurnExecute()
    {
        // reduce duration
        Duration--;
    }

    public override void Update(BaseStatus newStatus)
    {
        // Update duration
        Duration = Mathf.Max( newStatus.Duration,Duration);
        Strength += newStatus.Strength;
    }
    
}
