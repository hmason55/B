using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunStatus : BaseStatus
{
    public StunStatus( int duration, BaseUnit owner, BaseUnit target) :base( 0,  duration,  owner,  target)
    {
        Icon = Resources.Load<Sprite>("Sprites/Icons/midthreat");
    }

    public override void EndStatusExecute()
    {
        // reduce duration
        Duration--;
    }

    public override string GetDescription()
    {
        string msg = "Unit is stunned for " + Duration + " turn";
        if (Duration > 1)
            msg += "s";
        return msg;
    }

    public override void StartTurnExecute()
    {
       
    }

    public override void Update(BaseStatus newStatus)
    {
        // Update duration
        Duration = newStatus.Duration;
    }
}
