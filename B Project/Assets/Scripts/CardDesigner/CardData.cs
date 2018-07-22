using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CardData {

	// Save and Load variables.
	public string Title;
	public Card.CharacterType CharacterType;
	public Card.DeckClass DeckType;
	public Card.Category Category;
	public int ResourceCost;
	public string Description;
	public bool OmitFromDeck;
	public bool RequireTarget;
	public bool AreaOfEffect;
	public bool[] TargetArea = new bool[9];
	public Card.TargetType TargetType;
	public List<Effect> Effects;

	// Used only at runtime.
	public BaseUnit Owner;
}

[Serializable]
public class EffectsWrapper {
	public List<Effect> list;
}