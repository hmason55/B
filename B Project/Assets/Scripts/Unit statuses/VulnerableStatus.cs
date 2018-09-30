using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VulnerableStatus : BaseStatus
{

    public VulnerableStatus(float ratio,int duration, BaseUnit owner, BaseUnit target) :base(ratio,duration,owner,target)
    {
        Icon = Resources.Load<Sprite>("Sprites/Icons/lowthreat");

    }

    public override string GetDescription()
    {
        string msg = "Increase damage received by" + (Multiplier*100).ToString("0")+"%";
        return msg;
    }

    public override void StartTurnExecute()
    {
        Duration--;
    }

    public override void Update(BaseStatus newStatus)
    {
        Duration = newStatus.Duration;
        Multiplier = Mathf.Max(newStatus.Multiplier, Multiplier);
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