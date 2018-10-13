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

public delegate void AddictiveDamageModifier(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender);
public delegate void MultiplicativeDamageModifier(DamageEventArgs damage, BaseUnit attacker, BaseUnit defender);
public delegate void CardPlayed();

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

    private event AddictiveDamageModifier _addictiveDamageEvent;
    private event MultiplicativeDamageModifier _multiplicativeDamageEvent;
    private event CardPlayed _cardPlayedEvent;

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

        // Activate all items
        foreach (BaseItem item in _activeItems)
        {
            item.Activate();
        }

        // Create UI
        _itemDisplayUI = Instantiate(ItemUIPrefab).GetComponent<ItemDisplayUI>();
        _itemDisplayUI.UpdateItems(_activeItems);

    }


    public void SubscribeAddictiveDamage(AddictiveDamageModifier newDelegate)
    {
        _addictiveDamageEvent += newDelegate;
    }

    public void UnsubscribeAddictiveDamage(AddictiveDamageModifier newDelegate)
    {
        _addictiveDamageEvent -= newDelegate;
    }

    public void SubscribeMultiplicativeDamage(MultiplicativeDamageModifier newDelegate)
    {
        _multiplicativeDamageEvent += newDelegate;
    }

    public void UnsubscribeMultiplicativeDamage(MultiplicativeDamageModifier newDelegate)
    {
        _multiplicativeDamageEvent -= newDelegate;
    }

    public void SubscribeCardPlayed(CardPlayed newDelegate)
    {
        _cardPlayedEvent += newDelegate;
    }

    public void UnsubscribeCardPlayed(CardPlayed newDelegate)
    {
        _cardPlayedEvent -= newDelegate;
    }

    public int ApplyDamageModifiers(int damage,BaseUnit attacker, BaseUnit defender)
    {
        DamageEventArgs damageArg = new DamageEventArgs();
        damageArg.Value = damage;
        damageArg.Bonus = 0;
        if (_multiplicativeDamageEvent!=null)
            _multiplicativeDamageEvent(damageArg, attacker, defender);
        if (_addictiveDamageEvent!=null)
            _addictiveDamageEvent(damageArg, attacker, defender);
        Debug.Log("starting damage " + damage+"   bonus "+damageArg.Bonus);
        return damageArg.Value+Mathf.RoundToInt( damageArg.Bonus);
    }

    public int ApplyBlockModifiers(int block, BaseUnit attacker, BaseUnit defender)
    {

        return block;
    }

    public void ApplyCardPlayed(bool player)
    {
        if (player && _cardPlayedEvent!=null)
            _cardPlayedEvent();
    }

    public void ApplyEndTurn(bool player)
    {

    }
    

}
