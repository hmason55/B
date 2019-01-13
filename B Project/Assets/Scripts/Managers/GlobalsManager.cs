using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Container for damage calculation to pass around modifiers
public class DamageEventArgs : EventArgs
{
    public int Value { get; set; }
    public float Bonus { get; set; }
}

public delegate void DamageBonusAdditiveModifier(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender);
public delegate void DamageBonusMultiplicativeModifier(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender);
public delegate void CardPlayed(CardData cardData);
public delegate void DamageReductionAdditiveModifier(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender);
public delegate void DamageReductionMultiplicativeModifier(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender);
public delegate void EndTurn(bool player);

public class GlobalsManager : Singleton<GlobalsManager>
{
    // Handler of all effects globally available like items and global cards
    public GameObject ItemUIPrefab;
    public GameObject[] ItemPrefabs;
    // TEMP number of items to start with
    public int StartingItems;

    // Items in use
    private List<BaseItem> _activeItems;
    // UI script
    private ItemDisplayUI _itemDisplayUI;

    private event DamageBonusAdditiveModifier _addictiveDamageEvent;
    private event DamageBonusMultiplicativeModifier _multiplicativeDamageEvent;
    private event CardPlayed _cardPlayedEvent;
    private event DamageReductionAdditiveModifier _subtractiveDamageRedEvent;
    private event DamageReductionMultiplicativeModifier _multiplicativeDamageRedEvent;
    private event EndTurn _endTurnEvent;

    void Start()
    {
       // TEMP
        // Pick random items
        _activeItems = new List<BaseItem>();
        for (int i = 0; i < StartingItems; i++)
        {
            GameObject obj = Instantiate(ItemPrefabs[UnityEngine.Random.Range(0, ItemPrefabs.Length)]);
            BaseItem item = obj.GetComponent<BaseItem>();
            _activeItems.Add(item);
            Debug.Log("adding item: " + item.ItemName);


        }

        _itemDisplayUI = Instantiate(ItemUIPrefab).GetComponent<ItemDisplayUI>();


    }

    public void SetupItems()
    {
        // Activate all items
        foreach (BaseItem item in _activeItems)
        {
            item.Activate();
        }

        // Create UI
        //_itemDisplayUI = Instantiate(ItemUIPrefab).GetComponent<ItemDisplayUI>();
        _itemDisplayUI.UpdateItems(_activeItems);
    }

    #region Subscribe/unsubscribe events

    public void SubscribeAddictiveDamage(DamageBonusAdditiveModifier del)
    {
        _addictiveDamageEvent += del;
    }

    public void UnsubscribeAddictiveDamage(DamageBonusAdditiveModifier del)
    {
        _addictiveDamageEvent -= del;
    }

    public void SubscribeMultiplicativeDamage(DamageBonusMultiplicativeModifier del)
    {
        _multiplicativeDamageEvent += del;
    }

    public void UnsubscribeMultiplicativeDamage(DamageBonusMultiplicativeModifier del)
    {
        _multiplicativeDamageEvent -= del;
    }

    public void SubscribeCardPlayed(CardPlayed del)
    {
        _cardPlayedEvent += del;
    }

    public void UnsubscribeCardPlayed(CardPlayed del)
    {
        _cardPlayedEvent -= del;
    }

    public void SubscribeDamageReductionAdditive(DamageReductionAdditiveModifier del)
    {
        _subtractiveDamageRedEvent += del;
    }

    public void UnsubscribeDamageReductionAdditive(DamageReductionAdditiveModifier del)
    {
        _subtractiveDamageRedEvent -= del;
    }

    public void SubscribeDamageReductionMultiplicative(DamageReductionMultiplicativeModifier del)
    {
        _multiplicativeDamageRedEvent += del;
    }

    public void UnsubscribeDamageReductionMultiplicative(DamageReductionMultiplicativeModifier del)
    {
        _multiplicativeDamageRedEvent -= del;
    }

    public void SubscribeEndTurn(EndTurn del)
    {
        _endTurnEvent += del;
    }

    public void UnsubscribeEndTurn(EndTurn del)
    {
        _endTurnEvent -= del;
    }

    #endregion

    public int ApplyDamageModifiers(int damage,BaseUnit attacker, BaseUnit defender)
    {
        DamageEventArgs damageArg = new DamageEventArgs();
        damageArg.Value = damage;
        damageArg.Bonus = 0;
        

        // Apply bonus for attacker damage
        if (!defender.IsPlayer())
        {
            if (_multiplicativeDamageEvent != null)
                _multiplicativeDamageEvent(damageArg, attacker, defender);
            if (_addictiveDamageEvent != null)
                _addictiveDamageEvent(damageArg, attacker, defender);
        }
        else
        {
            // Apply damage reduction for defender
            if (_multiplicativeDamageRedEvent != null)
                _multiplicativeDamageRedEvent(damageArg, attacker, defender);
            if (_subtractiveDamageRedEvent != null)
                _subtractiveDamageRedEvent(damageArg, attacker, defender);
        }

        Debug.Log("starting damage " + damage+"   bonus "+damageArg.Bonus);
        return damageArg.Value+Mathf.RoundToInt( damageArg.Bonus);
    }

    public int ApplyBlockModifiers(int block, BaseUnit attacker, BaseUnit defender)
    {

        return block;
    }

    public void ApplyCardPlayed(bool player,CardData cardData)
    {
        if (player && _cardPlayedEvent!=null)
            _cardPlayedEvent(cardData);
    }

    public void ApplyEndTurn(bool player)
    {
        if (_endTurnEvent!=null)
            _endTurnEvent(player);
    }
    

}
