using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRumble : MonoBehaviour {

	[SerializeField] float magnitude = 0.5f;
	[SerializeField] float dampening = 1f;

	float currentMagnitude = 0f;
	static float cameraDefaultSize = 16.785f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)) {
			Rumble();
		}

		if(currentMagnitude > 0f) {
			// Shake camera
			Camera.main.transform.position = new Vector3(Random.Range(-currentMagnitude, currentMagnitude), Random.Range(-currentMagnitude, currentMagnitude), Camera.main.transform.position.z);
			Camera.main.orthographicSize = cameraDefaultSize - currentMagnitude;

			// Dampen
			currentMagnitude -= dampening * Time.deltaTime;
		} else {
			currentMagnitude = 0f;
			Camera.main.orthographicSize = cameraDefaultSize;
			Camera.main.transform.position = Vector3.zero;
		}
	}

	public void Rumble(float scale = 1f) {
		currentMagnitude += magnitude * scale;
	}
}
