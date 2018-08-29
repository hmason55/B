
using System;
using UnityEngine;

public class PoisonStatus : BaseStatus
{
    public PoisonStatus(int strength, int duration, BaseUnit owner, BaseUnit target) :base( strength,  duration,  owner,  target)
    {
        Icon = Resources.Load<Sprite>("Sprites/Icons/poison");
    }

    public override void EndStatusExecute()
    {
       
    }

    public override string GetDescription()
    {
        string msg = "At the start of next turn receive " + Strength + " poison damage";
        return msg;
    }

    public override void StartTurnExecute()
    {
        if (Duration<1)
        {
            // The status was expired
            UnityEngine.Debug.Log("Status " + this + " expired but stil active");
            return;
        }
        //apply damage
        Target.DealDamage(Strength);
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
