using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

	public bool mouseOver;
	public Card draggedCard;
	public float verticalThreshold;

	// UI
	[SerializeField] float cardWidth = 250f;
	[SerializeField] float cardHeight = 350f;
	[SerializeField] float maxSpacing = 24f;
	[SerializeField] float horizontalPadding = 75f;

	// Hand Data
	Deck deck;
	[SerializeField] int drawCount = 7;
	[SerializeField] int maxCards = 12;
	List<CardData> handList;
	float drawDelay = 0.15f;

	Coroutine deal;

	public List<CardData> HandList {
		get{return handList;}
	}

	public Deck DeckList {
		get{return deck;}
		set{deck = value;}
	}

	public void Deal() {
		if(deal == null) {
			deal = StartCoroutine(IEDeal());
		}
	}

	public IEnumerator IEDeal() {
		Clear();

		foreach(Transform t in transform) {
			t.GetComponent<Card>().DisableRaycast();
		}

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

		foreach(Transform t in transform) {
			t.GetComponent<Card>().ResumeRaycastState();
		}

		deal = null;
	}

	public void Draw(CardData cardData) {
		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Cards/CardDesigner")) as GameObject;
		Card card = obj.GetComponent<Card>();
		card.DisableRaycast();
		card.cardData = cardData;
		card.LoadCardData();
        Debug.Log(card.title+"card has " + card.effects.Count + " effects");
		handList.Add(card.cardData);
		obj.name = card.title;
		obj.transform.SetParent(transform);
		obj.transform.SetAsLastSibling();
		card.zIndex = obj.transform.GetSiblingIndex();
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
		Resize();
	}

	public void Remove(Card card) {
		int childCount = transform.childCount;
		for(int i = childCount-1; i >= 0; i--) {
			if(GameObject.ReferenceEquals(card.gameObject, transform.GetChild(i).gameObject)) {
				
				GameObject flagged = transform.GetChild(i).gameObject;

				// Spawn Particles
				GameObject particles = Instantiate(Resources.Load("Prefabs/Particles/Cards/Warrior Card Dissolve"), flagged.transform.position, Quaternion.identity) as GameObject;
				particles.transform.SetParent(transform.parent);


				flagged.transform.SetParent(null);
				Destroy(flagged);
			}
		}
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
		Canvas canvas = GameObject.FindObjectOfType<Canvas>();
		float spacingResize = maxSpacing;
		float difference = canvas.GetComponent<RectTransform>().sizeDelta.x - (2*horizontalPadding) - (cardWidth*transform.childCount) - (maxSpacing*(transform.childCount-1));
		if(difference < 0) {
			spacingResize = difference / (transform.childCount-1);
		}

		for(int i = 0; i < transform.childCount; i ++) {
			int handSize = transform.childCount;
			Transform t = transform.GetChild(i);
			if(t != null) {
				float handWidth = (handSize-1) * (spacingResize + cardWidth);
				float xPosition = i * (spacingResize + cardWidth);
				RectTransform rt = t.GetComponent<RectTransform>();
				Vector2 handPosition = new Vector2(xPosition - handWidth/2, 0f);
				t.GetComponent<RectTransform>().anchoredPosition = handPosition;
				t.GetComponent<Card>().HandPosition = handPosition;
			}
		}
	}

	public void UpdateResourceAvailability() {
		for(int i = 0; i < transform.childCount; i ++) {
			Transform t = transform.GetChild(i);
			t.GetComponent<Card>().UpdateResourceCost();
		}
	}

	void Start() {
		RectTransform rt = GetComponent<RectTransform>();
		verticalThreshold = rt.anchoredPosition.y + rt.sizeDelta.y;
	}
}
