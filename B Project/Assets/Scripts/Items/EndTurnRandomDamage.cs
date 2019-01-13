using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnRandomDamage : BaseItem
{
    public int HitsNumber;
    public int Damage;
    public bool Player;

    public override void Activate()
    {
        GlobalsManager.Instance.SubscribeEndTurn(EndTurn);

        Icon = Resources.Load<Sprite>("Sprites/Icons/burn");
    }

    public override void Deactivate()
    {
        GlobalsManager.Instance.UnsubscribeEndTurn(EndTurn);
    }

    public override string GetDescription()
    {
        string msg = "At the end of ";
        if (Player)
            msg += "player ";
        else
            msg += "enemy ";
        msg += "turn deals " + Damage + " damage to " + HitsNumber + " random enemies";
        return msg;
    }

    void EndTurn(bool player)
    {
        if (player==Player)
        {
            for (int i = 0; i < HitsNumber; i++)
            {
                // Populate possible targets
                List<BaseUnit> units = new List<BaseUnit>();
                if (player)
                {
                    foreach (BaseUnit unit in AIManager.Instance.GetEnemies())
                    {
                        if (unit.GetActualHP() > 0)
                            units.Add(unit);
                    }                    
                }

                // Pick random target and deal damage
                BaseUnit target = units[UnityEngine.Random.Range(0, units.Count)];
                target.DealDamage(Damage, null);
            }
        }
    }
}
