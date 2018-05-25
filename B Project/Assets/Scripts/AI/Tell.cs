using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tell : MonoBehaviour {

	[SerializeField] bool hidden = false;
	[SerializeField] Card.DeckClass deckType = Card.DeckClass.Neutral;
	[SerializeField] Deck deck;
	GameObject tellObject;

	void Start() {
		deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
	}

	public void ToggleTell() {
		if(hidden) {
			ShowTell();
		} else {
			HideTell();
		}
		hidden = !hidden;
	}

	void ShowTell() {
		CardData cardData = deck.GetRandomCard(deckType);
		if(Random.value > 0.80f) {
			cardData = deck.GetRandomCard(Card.DeckClass.Neutral);
		}

		tellObject = GameObject.Instantiate(Resources.Load("Prefabs/Cards/CardDesigner")) as GameObject;
		Card card = tellObject.GetComponent<Card>();
		card.cardData = cardData;
		card.LoadCardData();
		tellObject.name = card.title;
		tellObject.transform.SetParent(transform.parent);
		tellObject.transform.SetAsLastSibling();
		tellObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		tellObject.transform.localScale = new Vector3(1f, 1f, 1f);

		GetComponentInChildren<Text>().text = "X";
	}

	void HideTell() {
		Destroy(tellObject);
		GetComponentInChildren<Text>().text = "?";
	}
}
