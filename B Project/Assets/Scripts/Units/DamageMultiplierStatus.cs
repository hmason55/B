
using System;

public class DamageMultiplierStatus : BaseStatus
{
	public DamageMultiplierStatus(float multiplier, int duration, BaseUnit owner, BaseUnit target) :base( multiplier,  duration,  owner,  target)
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
    }

    public override void Update(BaseStatus newStatus)
    {
        // Sum duration and strength
        Multiplier += newStatus.Multiplier;
    }
}
