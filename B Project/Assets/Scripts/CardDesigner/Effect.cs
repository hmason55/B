using UnityEngine;
using System;

[Serializable]
public class Effect {
	public enum RemovalCondition {
		None,
		PlayAttackCard
	}

	public Card.TargetType targetType;
	public Card.EffectType effectType;
	public int effectValue;
	public int duration;
	public RemovalCondition condition;
}