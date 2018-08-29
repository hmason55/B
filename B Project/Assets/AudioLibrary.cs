using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLibrary : MonoBehaviour {

	private AudioClip attackHitArmor;
	private AudioClip attackHitFlesh;
	private AudioClip poisonAfflict;
	private AudioClip poisonTick;
	private AudioClip weaponSwoosh;

	private List<AudioClip> library;

	void Awake() {
		
	}

	// Use this for initialization
	void Start () {
		
	}
}
