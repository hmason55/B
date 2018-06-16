using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : BaseUnit
{
    // TEMP
    // The cards the enemy can play
    public Card[] PlayableCards;

    // The card the enemy will play on its turn
    private Card _nextCard;
    // Enemy next card UI
    private TextMesh _nextCardUI;

    protected override void Awake()
    {
        base.Awake();
        
        GameObject UI = new GameObject("Next Card UI");
        _nextCardUI = UI.AddComponent<TextMesh>();
        UI.transform.SetParent(transform);
        
    }


    public void DealAnotherCard()
    {
        if (PlayableCards.Length==0)
        {
            // No card to play
            Debug.Log("Mo card to play on:  " + name);            
        }

        // TEMP pick a card randomly for now
        _nextCard = PlayableCards[Random.Range(0, PlayableCards.Length)];

        // Update next card UI
        UpdateNextCardUI();
    }

    void UpdateNextCardUI()
    {
        _nextCardUI.transform.position = transform.position + Vector3.up * 3f;
        _nextCardUI.text = _nextCard.name;
        _nextCardUI.fontSize = 24;
        _nextCardUI.characterSize = 0.1f;
        _nextCardUI.anchor = TextAnchor.MiddleCenter;
        _nextCardUI.color = Color.red;
    }

    public Card GetNextCard()
    {
        return _nextCard;
    }
}
