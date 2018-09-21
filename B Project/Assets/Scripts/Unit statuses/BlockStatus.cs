
using System;
using UnityEngine;

public class BlockStatus : BaseStatus
{
	public BlockStatus(int strength, int duration, BaseUnit owner, BaseUnit target) :base( strength,  duration,  owner,  target)
    {
        Icon = Resources.Load<Sprite>("Sprites/Icons/shieldbreak");

        owner.SpawnBattleText("+" + strength.ToString() + " Block");

        // Update threat
        if (owner.IsPlayer())
        {
            PartyManager.Instance.ChangeThreat(owner, strength * 0.005f);
        }
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
        Strength += newStatus.Strength;
        Duration = newStatus.Duration;
    }

    public override string GetDescription()
    {
        string msg = "Next turn blocks " + Strength + " damage";
        return msg;
    }

    public override void EndTurnExecute()
    {
    }

    public override void DestroyStatusExecute()
    {
    }

    public override void ExecuteLinkEffect()
    {
    }
}
