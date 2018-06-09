using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandMouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public void OnPointerEnter(PointerEventData eventData) {
		GetComponent<Hand>().mouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		GetComponent<Hand>().mouseOver = false;
	}
}
