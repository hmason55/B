using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseStatus
{
    // Power of the status
    public int Strength;
    public float Multiplier;
    // Turn until expiration
    public int Duration;
    // Unit that casted the status
    public BaseUnit Owner;
    // Unit afflicted by status
    public BaseUnit Target;
    // Icon sprite
    public Sprite Icon;

    public Effect.RemovalCondition Condition;

    // Base constructor
    protected  BaseStatus(int strength, int duration, BaseUnit owner, BaseUnit target)
    {
        Strength = strength;
        Duration = duration;
        Owner = owner;
        Target = target;
    }

	// Float constructor
    protected  BaseStatus(float multiplier, int duration, BaseUnit owner, BaseUnit target)
    {
		Multiplier = multiplier;
        Duration = duration;
        Owner = owner;
        Target = target;
    }

    // What happens with another identical status is applied
    public abstract void Update(BaseStatus newStatus);

    // Apply effect at start of turn, if any
    public abstract void StartTurnExecute();

    // Apply effect when status end, if any
    public abstract void EndStatusExecute();

    // Status description
    public abstract string GetDescription();

}
