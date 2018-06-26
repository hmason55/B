using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	int saveSlot = 0;

	public int SaveSlot {
		get{return saveSlot;}
		set{saveSlot = value;}
	}

	void Awake() {

	}

	void Start () {
		
	}

}
