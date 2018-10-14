using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnRandomBlock : BaseItem
{
    public int HitsNumber;
    public int Block;
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
        msg += "turn adds " + Block + " block to " + HitsNumber + " random team members";
        return msg;
    }

    void EndTurn(bool player)
    {
        if (player == Player)
        {
            for (int i = 0; i < HitsNumber; i++)
            {
                // Populate possible targets
                List<BaseUnit> units = new List<BaseUnit>();
                if (player)
                {
                    foreach (BaseUnit unit in PartyManager.Instance.GetUnits())
                    {
                            units.Add(unit);
                    }
                }

                // Pick random target and adds block
                BaseUnit target = units[UnityEngine.Random.Range(0, units.Count)];
                BlockStatus block = new BlockStatus(Block, 1, target, null);
                target.AddStatus(block);
            }
        }
    }

}
