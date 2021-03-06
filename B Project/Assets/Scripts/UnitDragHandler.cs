﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {


	Vector3 mouseOffset;

	public void OnBeginDrag(PointerEventData eventData) {
		mouseOffset = transform.position - Input.mousePosition;
	}

	public void OnDrag(PointerEventData eventData) {
		transform.position = Input.mousePosition + mouseOffset;
		//Debug.Log(transform.position + " -> " + Input.mousePosition);
	}

	public void OnEndDrag(PointerEventData eventData) {
		//transform.localPosition = Vector3.zero;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
