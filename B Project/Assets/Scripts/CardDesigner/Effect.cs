using UnityEngine;
using System;

[Serializable]
public class Effect {
	public Card.TargetType targetType;
	public Card.EffectType effectType;
	public int effectValue;
	public int duration;
}