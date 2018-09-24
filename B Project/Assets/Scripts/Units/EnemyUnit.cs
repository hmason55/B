using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : BaseUnit
{
    // Enemy deck
    public TextAsset Deck;

    private List<Deck> _deck;

    // The card the enemy will play on its turn
    private Card _nextCard;
    private int _deckIndex;
    private int _cardIndex;

    string cardDataPath = "Prefabs/Cards/Data/";

    protected override void Awake()
    {
        base.Awake();
        _player = false;
        _deck = new List<Deck>();


        // Loads cards to the deck parsing multiple subdecks 
        string formattedText = Deck.text.Replace(System.Environment.NewLine, String.Empty);
        string[] cardNames = formattedText.Split(',');
        
        List<string> cards = new List<string>();
        foreach (string cardName in cardNames)
        {
            if (cardName == "")
            {
                if (cards.Count>0)
                {
                    // Check to avoid adding empty decks

                    // Create new deck from split textAsset
                    TextAsset textAsset = new TextAsset(string.Join(",", cards.ToArray()));
                    Deck d = new Deck(textAsset);
                    _deck.Add(d);

                    // Reset cards
                    cards = new List<string>();
                }
            }
            else
            {
                cards.Add(cardName + ",");
            }
        }
        
        /*
        int i = 0;        
        foreach (Deck d in _deck)
        {
            Debug.Log("deck: " + i++);
            int j = 0;
            foreach (CardData card in d.DeckList)
            {
                Debug.Log("card: " + j + "   " + card.Title);
                j++;
            }
        }
        */
        ShuffleDeck();
    }

    void ShuffleDeck()
    {
        Deck temp;

    }


    public void DealAnotherCard()
    {        
        // Create next card          
        if (_cardIndex>=_deck[_deckIndex].DeckList.Count)
        {
            _deckIndex++;
            _cardIndex = 0;
        }
        if (_deckIndex >= _deck.Count)
        {
            // Surpassed subdeck length
            ShuffleDeck();
            _deckIndex = 0;
        }

        GameObject card = new GameObject();
        _nextCard = card.AddComponent<Card>();
        _nextCard.cardData = _deck[_deckIndex].DeckList[_cardIndex];
        _nextCard.cardData.Owner = this;
        _nextCard.LoadCardData();

        _cardIndex++;
        // Update next card UI
        UpdateNextCardUI();
    }

    void UpdateNextCardUI()
    {
        _characterUI.SetNextCard(_nextCard);
    }

    public Card GetNextCard()
    {
        return _nextCard;
    }
}
