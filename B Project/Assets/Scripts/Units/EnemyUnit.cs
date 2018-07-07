using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : BaseUnit
{
    // Enemy deck
    public TextAsset Deck;

    private Deck _deck;

    // The card the enemy will play on its turn
    private Card _nextCard;

    // Enemy next card UI
    //private TextMesh _nextCardUI;

    protected override void Awake()
    {
        base.Awake();
        _player = false;

        /*
        GameObject UI = new GameObject("Next Card UI");
        _nextCardUI = UI.AddComponent<TextMesh>();
        UI.transform.SetParent(transform);
        */

        // Loads cards to the deck
        _deck = new Deck(Deck);
    }


    public void DealAnotherCard()
    {
        if (_deck.DeckList.Count==0)
        {
            // No card to play
            Debug.Log("Mo card to play on:  " + name);            
        }

        // Load a random card from the deck  
        GameObject card = new GameObject();
        _nextCard = card.AddComponent<Card>();
        _nextCard.cardData = _deck.DrawRandomCard(_deck.DeckList, false);
        _nextCard.cardData.Owner = this;
        _nextCard.LoadCardData();

        // Update next card UI
        UpdateNextCardUI();
    }

    void UpdateNextCardUI()
    {
        /*
        _nextCardUI.transform.position = transform.position + Vector3.up * 3f;
        _nextCardUI.text = _nextCard.title;
        _nextCardUI.fontSize = 24;
        _nextCardUI.characterSize = 0.1f;
        _nextCardUI.anchor = TextAnchor.MiddleCenter;
        _nextCardUI.color = Color.red;
        */
        _characterUI.SetNextCard(_nextCard);
    }

    public Card GetNextCard()
    {
        return _nextCard;
    }
}
