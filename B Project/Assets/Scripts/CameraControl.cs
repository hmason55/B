using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	RectTransform boardRect;

	// Use this for initialization
	void Start () {
		boardRect = GameObject.FindGameObjectWithTag("Board").GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		Zoom();
	}

	void Zoom() {
		float scrollValue = Input.GetAxis("Mouse ScrollWheel")*0.5f;
		boardRect.localScale = new Vector3(boardRect.localScale.x + scrollValue, boardRect.localScale.y + scrollValue, boardRect.localScale.z + scrollValue);

		if(Input.GetMouseButton(2)) {
			boardRect.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
	}
}
