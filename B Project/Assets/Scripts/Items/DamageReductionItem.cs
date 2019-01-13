using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReductionItem : BaseItem
{
    public int DamageRed;
    public float DamageRedPercent;


    public override void Activate()
    {
        GlobalsManager.Instance.SubscribeDamageReductionAdditive(SubtractiveDamage);
        GlobalsManager.Instance.SubscribeDamageReductionMultiplicative(PercentReductionDamage);

        Icon = Resources.Load<Sprite>("Sprites/Icons/burn");


    }

    public override void Deactivate()
    {
        GlobalsManager.Instance.UnsubscribeDamageReductionAdditive(SubtractiveDamage);
        GlobalsManager.Instance.UnsubscribeDamageReductionMultiplicative(PercentReductionDamage);
    }

    public override string GetDescription()
    {
        string msg = "Reduce damage received by";
        if (DamageRed > 0)
            msg += " " + DamageRed.ToString();
        if (DamageRedPercent > 0)
            msg += " " + (DamageRedPercent * 100).ToString() + "%";
        return msg;
    }

    void SubtractiveDamage(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender)
    {
        Debug.Log("adding "+DamageRed+" to bonus");
        damage.Bonus -= DamageRed;
    }

    void PercentReductionDamage(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender)
    {
        Debug.Log("adding " + DamageRedPercent * damage.Value + " to bonus");
        damage.Bonus -= DamageRedPercent * damage.Value;
    }

}
