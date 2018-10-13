using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusDamageItem : BaseItem
{
    public int DamageAdd;
    public float DamageMult;


    public override void Activate()
    {
        GlobalsManager.Instance.SubscribeAddictiveDamage(AddictiveDamage);
        GlobalsManager.Instance.SubscribeMultiplicativeDamage(MultiplicativeDamage);

        Icon = Resources.Load<Sprite>("Sprites/Icons/burn");


    }

    public override void Deactivate()
    {
        GlobalsManager.Instance.UnsubscribeAddictiveDamage(AddictiveDamage);
        GlobalsManager.Instance.UnsubscribeMultiplicativeDamage(MultiplicativeDamage);
    }

    public override string GetDescription()
    {
        string msg = "Increase each damage dealt by";
        if (DamageAdd > 0)
            msg +=" "+ DamageAdd.ToString();
        if (DamageMult > 0)
            msg += " " + (DamageMult*100).ToString()+"%";
        return msg;
    }

    void AddictiveDamage(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender)
    {
        damage.Bonus += DamageAdd;
    }

    void MultiplicativeDamage(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender)
    {
        damage.Bonus += DamageMult * damage.Value;
    }
}
