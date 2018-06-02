using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Entity
{ 
    // Base interface for every type of entity lying on the battleground


    // Example methods that might be needed
    int GetActualHP();
    int GetMaxHP();
    bool IsPlayer();
    bool IsTargetable();
    void OnDeath();
    void SetGridPosition(int position);

}
