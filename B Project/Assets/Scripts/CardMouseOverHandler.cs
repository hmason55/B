using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField] Card card;
	[SerializeField] Image outline;

	public void OnPointerEnter(PointerEventData eventData) {
		// set scale
		Vector2 currentPosition = GetComponent<RectTransform>().anchoredPosition;
		GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPosition.x, 70f);
		GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
		transform.SetAsLastSibling();
	
		if(outline == null) {return;}

		ShowOutline();
	}

	public void OnPointerExit(PointerEventData eventData) {
		// set scale
		Vector2 currentPosition = GetComponent<RectTransform>().anchoredPosition;
		GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPosition.x, 0f);
		GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		transform.SetSiblingIndex(card.zIndex);

		if(outline == null) {return;}

		HideOutline();
	}

	public void ShowOutline() {
		outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 0.85f);
	}

	public void HideOutline() {
		outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 0f);
	}
}
