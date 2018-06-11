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

namespace ProcGen
{
    public class AgentBehaviours : MonoBehaviour
    {

        public void LogData(ProcData data)
        {
            Debug.Log(string.Format("Agent moved to position ({0},{1})", data.pos.x, data.pos.y));
        }

        public void LogType(ProcData data)
        {
            Debug.Log(string.Format("Cell is of type {0}", data.type));
        }

        void SetEmpty(ProcData data)
        {
            data.agent.DecrementType(data, CellType.empty);
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
            //var agent = data.agent;

            data.type = CellType.rest;
        }

        public void MoveToRandomCell(ProcData data)
        {
            ProcAgent agent = data.agent;
            agent.Vertex = agent.RandomPoint();
        }

        public void MoveToCenter(ProcData data)
        {
            var agent = data.agent;

            agent.Vertex = agent.center;
        }
    }
}