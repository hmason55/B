using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthLinkStatus : BaseStatus
{

    public HealthLinkStatus(float ratio,BaseUnit owner, BaseUnit target) :base(ratio,999,owner,target)
    {
        Icon = Resources.Load<Sprite>("Sprites/Icons/lowthreat");
        
    }

    public override string GetDescription()
    {
        string msg = "Damage receiver is transfered to "+Target;
        return msg;
    }

    public override void StartTurnExecute()
    {
        Duration--;
    }

    public override void Update(BaseStatus newStatus)
    {
        
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