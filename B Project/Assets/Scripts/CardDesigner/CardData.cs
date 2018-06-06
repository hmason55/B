using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CardData {

	public string Title;
	public Card.DeckClass DeckType;
	public int ResourceCost;
	public string Description;
	public bool OmitFromDeck;
	public bool RequireTarget;
	public bool AreaOfEffect;
	public bool[] TargetArea = new bool[9];
	public Card.TargetType TargetType;
	//public List<Card.Effect> Effects;
	public List<Card.Effect> Effects;
}

[Serializable]
public class EffectsWrapper {
	public List<Card.Effect> list;
}