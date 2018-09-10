using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFieldEffect : BaseStatus
{
    // Battleground position
    public int Position;

    public BaseFieldEffect(int strength, int duration, BaseUnit owner,int position) : base(strength, duration, owner, null)
    {
        Position = position;
    }
    
}