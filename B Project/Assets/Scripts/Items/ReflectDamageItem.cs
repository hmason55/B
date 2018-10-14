using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectDamageItem : BaseItem
{
    public int ReflectFix;
    public float ReflectPercent;


    public override void Activate()
    {
        GlobalsManager.Instance.SubscribeDamageReductionAdditive(ReflectDamage);

        Icon = Resources.Load<Sprite>("Sprites/Icons/burn");
    }

    public override void Deactivate()
    {
        GlobalsManager.Instance.UnsubscribeDamageReductionAdditive(ReflectDamage);
    }

    public override string GetDescription()
    {
        string msg = "Reflect ";
        if (ReflectFix > 0)
            msg += ReflectFix.ToString();
        if (ReflectPercent > 0)
            msg += (ReflectPercent * 100).ToString() + "%";
        msg += " damage after receiving a hit";
        return msg;
    }

    void ReflectDamage(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender)
    {
        if (defender.GetDeckClass() != DeckType && DeckType != Card.DeckClass.Neutral)
            return;
        int reflect = ReflectFix + (int)(ReflectPercent * damage.Value);
        if (reflect > 0)
            attacker.DealDamage(reflect, defender);
    }

}
