using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

	// UI
	[SerializeField] float cardWidth = 250f;
	[SerializeField] float cardHeight = 350f;
	[SerializeField] float spacing = 24f;

	// Hand Data
	[SerializeField] Deck deck;
	[SerializeField] int drawCount = 7;
	[SerializeField] int maxCards = 12;
	List<CardData> handList;
	float drawDelay = 0.15f;

	Coroutine deal;

	public List<CardData> HandList {
		get{return handList;}
	}

	public void Deal() {
		if(deal == null) {
			deal = StartCoroutine(IEDeal());
		}
	}

	public IEnumerator IEDeal() {
		Clear();

		if(deck.ReferenceDeck.Count < drawCount) {
			for(int i = 0; i < deck.ReferenceDeck.Count; i++) {
				Draw(deck.DrawRandomCard(handList));
				yield return new WaitForSeconds(drawDelay);
			}
		} else {
			for(int i = 0; i < drawCount; i++) {
				Draw(deck.DrawRandomCard(handList));
				yield return new WaitForSeconds(drawDelay);
			}
		}

		deal = null;
	}

	public void Draw(CardData cardData) {
		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Cards/CardDesigner")) as GameObject;
		Card card = obj.GetComponent<Card>();
		card.cardData = cardData;
		card.LoadCardData();
		handList.Add(card.cardData);
		obj.name = card.title;
		obj.transform.SetParent(transform);
		obj.transform.SetAsLastSibling();
		card.zIndex = obj.transform.GetSiblingIndex();
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
		Resize();
	}

	public void Clear() {
		handList = new List<CardData>();
		int childCount = transform.childCount;
		for(int i = childCount-1; i >= 0; i--) {
			GameObject flagged = transform.GetChild(i).gameObject;
			flagged.transform.SetParent(null);
			Destroy(flagged);
		}
	}

	public void Resize() {
		for(int i = 0; i < transform.childCount; i ++) {
			int handSize = transform.childCount;
			Transform t = transform.GetChild(i);
			if(t != null) {
				float handWidth = (handSize-1) * (spacing + cardWidth);
				float xPosition = i * (spacing + cardWidth);
				RectTransform rt = t.GetComponent<RectTransform>();
				Vector2 handPosition = new Vector2(xPosition - handWidth/2, 0f);
				t.GetComponent<RectTransform>().anchoredPosition = handPosition;
				t.GetComponent<Card>().HandPosition = handPosition;
			}
		}
	}
}
