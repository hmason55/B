using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGlobalStatus : BaseStatus
{
    public string Source;
    public bool Item;

    public BaseGlobalStatus(int strength, int duration, BaseUnit owner,string source) : base(strength, duration, owner, null)
    {
        Source = source;
    }

}
