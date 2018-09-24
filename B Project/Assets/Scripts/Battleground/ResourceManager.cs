using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : Singleton<ResourceManager> {

	public PartyManager partyManager;
	public Hand hand;

	// Current turn's resource counter.
	public Image resourceCurrentImage;
	public Text resourceCurrentText;

	// Next turn's resource counter.
	public Image resourceNextImage;
	public Text resourceNextText;

	static int startingResources = 4;	// Starting resource count for a party.
	static int resourcesPerUnit = 1;	// Additional party resource count per unit in party.

	int baseResource;
	int resourceCurrent;
	int resourceNext;

	public int CurrentResources {
		get {return resourceCurrent;}
	}
    
	public bool SpendResources(int count, BaseUnit spender) {
        Debug.Log("spending " + count + " from " + spender);
		if(!spender.IsPlayer()) {return true;}

		if(resourceCurrent - count >= 0) {
			resourceCurrent -= count;
			UpdateResourceUI();
			return true;
		}

		Debug.Log("Not enough resources");
		return false;
	}

    public bool AddNextTurnResource(int value, BaseUnit spender)
    {
        Debug.Log("adding " + value + " resource");
        if (!spender.IsPlayer()) { return true; }

        if (resourceNext + value >= 0)
        {
            resourceNext += value;
            UpdateResourceUI();
            return true;
        }
        return false;
    }

	public void CalculateResources() {
		baseResource = startingResources + (partyManager.PlayerUnits.Count * resourcesPerUnit);

        if (resourceNext == 0 && resourceCurrent == 0)
            resourceNext = baseResource;
        // Set to the base amount because there are no additional ways to gain resource yet.
        resourceCurrent = resourceNext;
		resourceNext = baseResource;

		UpdateResourceUI();
	}

	public void UpdateResourceUI() {
		resourceCurrentText.text = resourceCurrent.ToString();
		resourceNextText.text = resourceNext.ToString();
		hand.UpdateResourceAvailability();
	}
}
