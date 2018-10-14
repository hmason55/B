using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePerCardPlayed : BaseItem
{
    public int CardNumber;
    public int BonusResource;

    private int _counter;

    public override void Activate()
    {
        GlobalsManager.Instance.SubscribeCardPlayed(CardPlayed);

        Icon = Resources.Load<Sprite>("Sprites/Icons/burn");
    }

    public override void Deactivate()
    {
        GlobalsManager.Instance.UnsubscribeCardPlayed(CardPlayed);
    }

    public override string GetDescription()
    {
        string msg = "For every "+CardNumber+ " played by "+DeckType.ToString()+" add "+BonusResource+" resource";
        return msg;
    }

    void CardPlayed(CardData cardData)
    {
        if (cardData.DeckType != DeckType && DeckType!= Card.DeckClass.Neutral)
            return;
        if (++_counter>= CardNumber)
        {
            _counter = 0;
            ResourceManager.Instance.SpendResources(-BonusResource, null);
        }
    }
}
