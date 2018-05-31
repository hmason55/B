using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMouseExitHandler : MonoBehaviour, IPointerExitHandler {

	[SerializeField] Card card;
	[SerializeField] Image outline;

	public void OnPointerExit(PointerEventData eventData) {
		// set scale
		GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		transform.SetSiblingIndex(card.zIndex);

		if(outline == null) {return;}

		outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 0f);
	}
}
