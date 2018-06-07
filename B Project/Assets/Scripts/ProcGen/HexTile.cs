using System.Collections;
using UnityEngine;

public class HexTile : MonoBehaviour {

    public ProcData data;

	void Start () {
		
	}

    /// <summary>
    /// A helper method used to retrieve the x,y position of the map present in the ProcAgent.
    /// </summary>
    /// <returns></returns>
    public Vector2Int getPos() {
        return data.pos;
    }

    public void setMaterial()
    {
        Renderer rend = data.obj.GetComponent<Renderer>();

        switch (data.type)
        {
            case CellType.empty:
                rend.material.SetColor("Empty", new Color(0, 0, 0.8f, 1));
                break;
            case CellType.merchant:
                rend.material.SetColor("Merchant", new Color(0.95f, 0.95f, 0.05f, 1));
                break;
            case CellType.rest:
                rend.material.SetColor("Rest", new Color(0.7f, 0.5f, 0.3f, 1));
                break;
            case CellType.settlement:
                rend.material.SetColor("Settlement", new Color(0.9f, 0.0f, 0.0f, 1));
                break;
            case CellType.treasure:
                rend.material.SetColor("Treasure", new Color(0, 0, 0.9f, 1));
                break;
            default:
                break;
        }

    }
}
