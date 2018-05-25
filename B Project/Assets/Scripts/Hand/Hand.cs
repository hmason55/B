using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

	
	[SerializeField] Deck deck;
	[SerializeField] int drawCount = 7;

	public void Deal() {
		Clear();

		for(int i = 0; i < drawCount; i++) {
			Draw(deck.GetRandomCard());
		}
	}

	public void Draw(CardData cardData) {
		GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Cards/CardDesigner")) as GameObject;
		Card card = obj.GetComponent<Card>();
		card.cardData = cardData;
		card.LoadCardData();
		obj.name = card.title;
		obj.transform.SetParent(transform);
		obj.transform.SetAsLastSibling();
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void Clear() {
		int childCount = transform.childCount;
		for(int i = childCount-1; i >= 0; i--) {
			//GameObject child = transform.GetChild(i).gameObject;
			//child.transform.SetParent(null);
			Destroy(transform.GetChild(i).gameObject);
		}

	}
}
