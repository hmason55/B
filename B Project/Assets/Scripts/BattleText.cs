using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleText : MonoBehaviour {

	float ySpeed = 5f;
	float xSpeed = 0;
	float friction = 5f;
	float gravity = 1f;
	float lifeTime = 1f;

	float scaleFactor = 0.01f;
	float spawnTime;

	// Use this for initialization
	void Start () {
		spawnTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
		float elapsedTime = CheckLifetime();
		float currentYSpeed = ySpeed - gravity * elapsedTime * elapsedTime;
		GetComponent<RectTransform>().anchoredPosition += new Vector2(Screen.width * xSpeed * scaleFactor * Time.deltaTime, Screen.height * currentYSpeed * scaleFactor * Time.deltaTime);
	}

	float CheckLifetime() {
		float elapsedTime = Time.realtimeSinceStartup - spawnTime;
		if(Time.realtimeSinceStartup - spawnTime > lifeTime) {
			Destroy(gameObject);
		} else {
			return elapsedTime;
		}
		return lifeTime;
	}

}
