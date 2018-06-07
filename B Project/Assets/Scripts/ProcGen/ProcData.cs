using System.Collections;
using UnityEngine;

public enum CellType {
    treasure,
    merchant,
    rest,
    settlement,
    empty,
    nil
}

[System.Serializable]
public class ProcData {

    public CellType type;
    public GameObject obj;
    public Vector2Int pos;

    public ProcData(CellType type) {
        this.type = type;
    }

    public ProcData() {
        this.type = CellType.empty;
    }

    public override string ToString() {
        return "";
    }
}
