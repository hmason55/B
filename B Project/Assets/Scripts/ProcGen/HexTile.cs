using System.Collections;
using UnityEngine;
using UnityEditor;

namespace ProcGen
{
    public class HexTile : MonoBehaviour
    {

        public ProcData data;

        void Start()
        {
        }

        void OnValidate()
        {

            if (data != null && data.agent != null)
            {
                Vector2 pos = data.agent.HexCoordinate(data.pos.x, data.pos.y);
                this.transform.position = new Vector3(pos.x, 0, pos.y);
                this.transform.Translate(0f, 0f, pos.y);
                //data.agent.setData(data.pos.x, data.pos.y, data);
                SetName();
            }
        }

        void OnDestroy()
        {
            data.obj = null;
        }

        /// <summary>
        /// A helper method used to retrieve the x,y position of the map present in the ProcAgent.
        /// </summary>
        /// <returns></returns>
        public Vector2Int getPos()
        {
            return data.pos;
        }

        void OnDrawGizmos()
        {
            if (data.agent.debug)
            {
                Handles.Label(transform.position, "(" + data.pos.x + "," + data.pos.y + ")");
            }
        }

        public void SetName()
        {
            data.obj.name = string.Format("Hex: ({0},{1})", data.pos.x, data.pos.y);
        }

        public void SetMaterial()
        {
            Renderer rend = data.obj.GetComponent<Renderer>();

            switch (data.type)
            {
                case CellType.empty:
                    rend.sharedMaterial = Resources.Load("Material/Tile", typeof(Material)) as Material;
                    break;
                case CellType.merchant:
                    rend.sharedMaterial = Resources.Load("Material/Merchant", typeof(Material)) as Material;
                    break;
                case CellType.rest:
                    rend.sharedMaterial = Resources.Load("Material/Rest", typeof(Material)) as Material;
                    break;
                case CellType.settlement:
                    rend.sharedMaterial = Resources.Load("Material/Settlement", typeof(Material)) as Material;
                    break;
                case CellType.treasure:
                    rend.sharedMaterial = Resources.Load("Material/Treasure", typeof(Material)) as Material;
                    break;
                default:
                    break;
            }
        }
    }
}