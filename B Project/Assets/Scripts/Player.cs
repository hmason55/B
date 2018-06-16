using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Template Player class 
public class Player : MonoBehaviour {

	
	

	/// The initial amount of resources the player gets each turn before modifiers and other unit's resources are summed.
	int baseResources = 4;

	/// The amount of resources remaining this turn.
	int currentResources;

	/// At the start of each turn, the player's resource count is set to this value.
	int turnResources;

	public int BaseResources {
		get{return baseResources;}
	}

	public int CurrentResources {
		get{return currentResources;}
	}

	public int TurnResources {
		get{return turnResources;}
	}

	int CalcTurnResources() {
		// How many resources does the player get each turn?
		// baseResource + each unit's resources + modifiers
		return turnResources;
	}

	int CalcCurrentResources() {
		// How many resources remain?
		return currentResources;
	}

	bool SpendResource(int cost) {
		if(currentResources - cost < 0) {
			Debug.Log("Insufficient resources");
			return false;
		}

		currentResources -= cost;
		return true;	// Resources successfully spent.
	}

	bool GainResource(int gain) {
		currentResources += gain;
		return true;	// Resources successfully gained.
	}


}
