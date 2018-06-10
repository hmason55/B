using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains a bunch of behaviours to be used in agent scripting
//Events -
//OnBoundary - When agent is on a boundary cell
//OnMove - When the agent moves to another cell
//OnCenter - When the agent is on the center cell
//OnSet - When the agent changes the tile
//OnBarren - When there are no locations within 50% of this radius

public class AgentBehaviours {

    void SetEmpty(ProcData data) {
        data.type = CellType.empty;
    }

    void SetSettlement(ProcData data)
    {

    }

    void SetMerchant(ProcData data)
    {

    }

    void SetTreasure(ProcData data)
    {

    }

    void SetRest(ProcData data)
    {

    }

    void MoveToRandomCell(ProcData data)
    {
        
    }

    void MoveToCenter(ProcData data)
    {

    }
}
