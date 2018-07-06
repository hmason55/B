using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseStatus
{

    // Power of the status
    public int Strength;
    // Turn until expiration
    public int Duration;
    // Unit that casted the status
    public BaseUnit Owner;
    // Unit afflicted by status
    public BaseUnit Target;

    // Base constructor
    protected  BaseStatus(int strength, int duration, BaseUnit owner, BaseUnit target)
    {
        Strength = strength;
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


}
