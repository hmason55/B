
using System;
using UnityEngine;

public class DamageMultiplierStatus : BaseStatus
{
	public DamageMultiplierStatus(float multiplier, int duration, BaseUnit owner, BaseUnit target) :base( multiplier,  duration,  owner,  target)
    {
        //Icon=  UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        Icon = Resources.Load<Sprite>("Sprites/Icons/burn");
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
        string msg = "Next attack deals " + (Multiplier*100).ToString("#") + "% bonus damage";
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

        // reduce duration
        Duration--;
    }

    public override void Update(BaseStatus newStatus)
    {
        // Sum duration and strength
        Multiplier += newStatus.Multiplier;
    }
}
