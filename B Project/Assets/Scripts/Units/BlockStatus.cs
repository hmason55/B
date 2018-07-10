﻿
using System;

public class BlockStatus : BaseStatus
{
	public BlockStatus(int strength, int duration, BaseUnit owner, BaseUnit target) :base( strength,  duration,  owner,  target)
    {
        
    }

    public override void EndStatusExecute()
    {
       
    }

    public override void StartTurnExecute()
    {
        if (Duration<1)
        {
            // The status was expired
            UnityEngine.Debug.Log("Status " + this + " expired but stil active");
            return;
        }

        // reduce duration
        Duration--;
        // Reduce strength and expire if 0
        if (--Strength < 1)
            Duration = 0;
    }

    public override void Update(BaseStatus newStatus)
    {
        // Sum duration and strength
        Strength += newStatus.Strength;
        Duration += newStatus.Strength;
    }
}
