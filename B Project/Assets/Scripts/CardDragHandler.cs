using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	[SerializeField] Card card;
	[SerializeField] Image outline;
	[SerializeField] Transform hand;
	LineRenderer targetLine;
	Vector3 mouseOffset;

	public void OnBeginDrag(PointerEventData eventData) {
		mouseOffset = transform.position - Input.mousePosition;
		Debug.Log("Drag " + gameObject.name);

		if(hand == null) {return;}
		foreach(Transform cardTransform in hand) {
			cardTransform.GetComponent<Card>().DisableRaycast();
		}

		hand.GetComponent<Hand>().draggedCard = card;
	}

	public void OnDrag(PointerEventData eventData) {
		Vector3 mousePosition = Input.mousePosition;
		if(card.requireTarget) {

			if(Input.mousePosition.y > hand.GetComponent<Hand>().verticalThreshold) {
				transform.position = new Vector3(Screen.width/2f, 210f, 0f);
				EnableLineTarget();

			} else {
				transform.position = mousePosition + mouseOffset;
				DisableLineTarget();
			}
		} else {
			transform.position = mousePosition + mouseOffset;
			DisableLineTarget();
		}

		GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
		transform.SetAsLastSibling();
		GetComponent<CardMouseOverHandler>().ShowOutline();
	}

	public void OnEndDrag(PointerEventData eventData) {
		Debug.Log("Drop " + gameObject.name);
		if(hand == null) {return;}
		card.SnapToHand();
		DisableLineTarget();

		foreach(Transform cardTransform in hand) {
			Card card = cardTransform.GetComponent<Card>();
			card.ResumeRaycastState();
		}

		GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		GetComponent<CardMouseOverHandler>().HideOutline();
		transform.SetSiblingIndex(card.zIndex);

		hand.GetComponent<Hand>().draggedCard = null;

        //TEMP TEST to apply card to units, remove all this ugly code asap!
        Battleground bg = FindObjectOfType<Battleground>();
        foreach (var unit in bg.GetUnitsSelected())
        {
            Debug.Log("target " + unit.UnitName);
            unit.DealDamage(10);
        } 
        // END TEST


	}

	void Awake() {
		hand = GameObject.FindGameObjectWithTag("Hand").transform;
		GameObject lineGO = GameObject.FindGameObjectWithTag("TargetLine");
		if(lineGO != null) {
			targetLine = lineGO.GetComponent<LineRenderer>();
		}
	}

	void EnableLineTarget() {
		if(targetLine != null) {
			targetLine.enabled = true;
			Vector3 cardWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z));
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			cardWorldPosition.z = 0;
			mouseWorldPosition.z = 0;
			Vector3 unitVector = mouseWorldPosition-cardWorldPosition;

			targetLine.SetPosition(0, new Vector3(cardWorldPosition.x, cardWorldPosition.y, -1f));
			targetLine.SetPosition(1, new Vector3(cardWorldPosition.x, cardWorldPosition.y, -1f) + unitVector*0.795f);
			targetLine.SetPosition(2, new Vector3(cardWorldPosition.x, cardWorldPosition.y, -1f) + unitVector*0.80f);
			targetLine.SetPosition(3, new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, -1f));
		}
	}

	void DisableLineTarget() {
		if(targetLine != null) {
			targetLine.SetPosition(0, Vector3.zero);
			targetLine.SetPosition(1, Vector3.zero);
			targetLine.SetPosition(2, Vector3.zero);
			targetLine.SetPosition(3, Vector3.zero);
		}
	}
}
