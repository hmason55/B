﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Deck : MonoBehaviour {

	static string cardDataPath = "Prefabs/Cards/Data/";
	List<CardData> deck;

	void Start() {
		LoadAllCards();
	}

	public void LoadAllCards() {
		Clear();

		DirectoryInfo info = new DirectoryInfo(cardDataPath);
		Object[] textAssets = Resources.LoadAll(cardDataPath, typeof(TextAsset));

		foreach(TextAsset textAsset in textAssets) {
			CardData cardData = JsonUtility.FromJson<CardData>(textAsset.text);
			deck.Add(cardData);
		}

		Debug.Log(deck.Count);

	}

	public void LoadCardsByClass(Card.DeckClass deckClass) {
		
	}

	public void Clear() {
		deck = new List<CardData>();
	}
}
