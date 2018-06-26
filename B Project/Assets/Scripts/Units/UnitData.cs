using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UnitData {
	public string UnitName;
	public BaseUnit.ID UnitID;
	public Card.DeckClass DeckClass;
	public int BaseHP;
	public string SpritePath;
}
