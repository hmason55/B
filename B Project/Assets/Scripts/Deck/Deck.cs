using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Deck : MonoBehaviour {

	static string cardDataPath = "Prefabs/Cards/Data/";
	List<CardData> deck;
	[SerializeField] TextAsset deckFile;

	public List<CardData> DeckList {
		get{return deck;}
	}

	void Start() {
		if(deckFile) {
			LoadCardsFromFile();
		} else {
			LoadAllCards();
		}
	}

	public void LoadAllCards() {
		Clear();

		UnityEngine.Object[] textAssets = Resources.LoadAll(cardDataPath, typeof(TextAsset));

		foreach(TextAsset textAsset in textAssets) {
			CardData cardData = JsonUtility.FromJson<CardData>(textAsset.text);
			if(!cardData.OmitFromDeck) {
				deck.Add(cardData);
			}
		}

		Debug.Log(deck.Count);
	}

	public void LoadCardsByClass(Card.DeckClass deckClass) {
		
	}

	public void LoadCardsFromFile() {
		Clear();

		if(deckFile == null) {
			return;
		}

		// Remove newline characters
		string formattedText = deckFile.text.Replace(System.Environment.NewLine, String.Empty);
		string[] cardNames = formattedText.Split(',');

		foreach(string cardName in cardNames) {
			TextAsset textAsset = Resources.Load<TextAsset>(cardDataPath + cardName);
			if(textAsset != null) {
				CardData cardData = JsonUtility.FromJson<CardData>(textAsset.text);
				if(!cardData.OmitFromDeck) {
					deck.Add(cardData);
				}
			}
		}
	}

	public void Clear() {
		deck = new List<CardData>();
	}

	public CardData GetRandomCard() {
		return deck[UnityEngine.Random.Range(0, deck.Count)];
	}

	public CardData GetRandomCard(Card.DeckClass deckClass) {
		
		if(UnityEngine.Random.value < 0.15f) {
			foreach(CardData data in deck) {
				if(data.Title == "Block") {
					return data;
				}
			}
		}

		CardData cardData;
		int attempts = 0;
		do {
			cardData = deck[UnityEngine.Random.Range(0, deck.Count)];
		} while(cardData.DeckType != deckClass && attempts < 500);

		return cardData;
	}
}
