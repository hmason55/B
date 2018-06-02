using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesDisplay : MonoBehaviour {

	public RectTransform imageRect;

	public void Reset() {
		imageRect.anchoredPosition = Vector2.zero;
	}

	public void Move(Vector3 destination) {
		imageRect.position = destination;
	}
}
