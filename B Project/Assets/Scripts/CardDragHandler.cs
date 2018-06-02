using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	[SerializeField] Card card;
	[SerializeField] Transform hand;
	Vector3 mouseOffset;

	public void OnBeginDrag(PointerEventData eventData) {
		mouseOffset = transform.position - Input.mousePosition;
		Debug.Log("Drag " + gameObject.name);

		if(hand == null) {return;}
		foreach(Transform cardTransform in hand) {
			if(cardTransform != transform) {
				cardTransform.GetComponent<Card>().DisableRaycast();
			}
		}
	}

	public void OnDrag(PointerEventData eventData) {
		transform.position = Input.mousePosition + mouseOffset;
	}

	public void OnEndDrag(PointerEventData eventData) {
		Debug.Log("Drop " + gameObject.name);

		card.SnapToHand();

		foreach(Transform cardTransform in hand) {
			Card card = cardTransform.GetComponent<Card>();
			card.ResumeRaycastState();
		}
	}

	void Awake() {
		hand = GameObject.FindGameObjectWithTag("Hand").transform;
	}
}
