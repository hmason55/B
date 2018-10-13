using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    // Item name
    public string ItemName;
    // Icon in the UI bar
    public Sprite Icon;
    // Character owning the item, if any
    public BaseUnit Owner;
    


    public abstract void Activate();

    public abstract void Deactivate();

    public abstract string GetDescription();
}
