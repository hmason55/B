using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Deck : MonoBehaviour {

	static string cardDataPath = "Prefabs/Cards/Data/";
	List<CardData> referenceDeck;
	List<CardData> deck;
	[SerializeField] TextAsset deckFile;

	public List<CardData> DeckList {
		get{return deck;}
	}

	public List<CardData> ReferenceDeck {
		get{return referenceDeck;}
	}

	void Start() {
		if(deckFile) {
			LoadCardsFromFile();
		} else {
			LoadAllCards();
		}
	}

	public void LoadAllCards() {
		ClearReferenceDeck();
		ClearDeck();

		UnityEngine.Object[] textAssets = Resources.LoadAll(cardDataPath, typeof(TextAsset));

		foreach(TextAsset textAsset in textAssets) {
			CardData cardData = JsonUtility.FromJson<CardData>(textAsset.text);
			if(!cardData.OmitFromDeck) {
				referenceDeck.Add(cardData);
			}
		}

		Debug.Log(deck.Count);
	}

	public void LoadCardsByClass(Card.DeckClass deckClass) {
		
	}

	public void LoadCardsFromFile() {
		ClearReferenceDeck();
		ClearDeck();

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
					referenceDeck.Add(cardData);
				}
			}
		}
	}

	public void LoadCardsFromFile(int saveSlot, Character.ID characterID) {
		ClearReferenceDeck();
		ClearDeck();

		deckFile = Resources.Load<TextAsset>("Prefabs/Cards/Decks/Slot_" + saveSlot + "/" + characterID.ToString());

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
					referenceDeck.Add(cardData);
				}
			}
		}
	}

	public void ClearDeck() {
		deck = new List<CardData>();
	}

	public void ClearReferenceDeck() {
		referenceDeck = new List<CardData>();
	}

	public CardData DrawRandomCard(List<CardData> handList, bool removeCard = true) {

		if(deck.Count == 0) {
			PopulateDeckFromReference(handList);
		}

		int ndx = UnityEngine.Random.Range(0, deck.Count);
		CardData cardData = deck[ndx];
		if(removeCard) {
			deck.RemoveAt(ndx);
		}
		return cardData;
	}

	public CardData DrawRandomCard(Card.DeckClass deckClass, List<CardData> handList, bool removeCard = true) {

		if(deck.Count == 0) {
			PopulateDeckFromReference(handList);
		}

		// For ai control
		if(UnityEngine.Random.value < 0.15f) {
			foreach(CardData data in deck) {
				if(data.Title == "Block") {
					return data;
				}
			}
		}

		CardData cardData;
		int ndx = -1;
		int attempts = 0;
		do {
			ndx = UnityEngine.Random.Range(0, deck.Count);
			cardData = deck[ndx];
		} while(cardData.DeckType != deckClass && attempts < 500);

		if(ndx > -1 && removeCard) {
			deck.RemoveAt(ndx);
		}

		return cardData;
	}

	void PopulateDeckFromReference(List<CardData> handList) {
		if(referenceDeck == null) {
			return;
		}

		ClearDeck();

		foreach(CardData rCardData in referenceDeck) {
			int ndx = -1;
			if(handList != null) {
				for(int i = 0; i < handList.Count; i++) {
					if(handList[i].Title == rCardData.Title) {
						ndx = i;
					}
				}
			}

			if(ndx == -1) {
				deck.Add(rCardData);
			} else {
				handList.RemoveAt(ndx);
			}
		}
	}

}
