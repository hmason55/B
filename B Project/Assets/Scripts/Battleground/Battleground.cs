using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleground : Singleton<Battleground>
{
    // Grid corner positions to set on Unity
    // Coordinates in viewport position [0-1]
    [HideInInspector]
    [SerializeField]
    private Vector2[] _cornersPositions = new Vector2[6];

    // Screen position of each tile
    private Vector2[,] _playerGrid = new Vector2[3, 3];
    private Vector2[,] _enemyGrid = new Vector2[3, 3];
    // Sprite order based on tile
    private int[] _spriteOrder;
    // Grid Mesh
    private Mesh _mesh;
    // Ground hazards
    private GameObject[] _hazards; // TODO change with hazard class eventually
    // Tile inhabitants
    private BaseUnit[] _units; // TODO eventually change with unit class or interface
    // Mouseover unit
    private List<BaseUnit> _mouseoverUnits;
    // Target Method
    private bool _targetTile = true;
    // Target shape
    private TargetShape _targetShape = TargetShape.Single;

    void Start()
    {
        //setup tiles
        _hazards = new GameObject[18];
        _units = new BaseUnit[18];
        _spriteOrder = new int[18];
        _mesh = new Mesh();
        _mouseoverUnits = new List<BaseUnit>();

        CalculateGridPositions();
        CalculateGridMesh();
        CalculateSpriteOrders();

    }

    void Update()
    {
        // Set target shape to none to not display anything on battleground
        if (_targetShape == TargetShape.None)
            return;

        // TEMP check mouse position and highlight eventual tile
        // TODO enable and disable based on game flow
        if (_targetTile)
            CheckMouseAgainstGrid();
        else
            CheckMouseAgainstUnits();
    }

    #region Grid calculations

    public void SetViewportCornerPositions(Vector2[] corners)
    {
        for (int i = 0; i < 6; i++)
        {
            _cornersPositions[i] = corners[i];
        }
    }

    public Vector2[] GetWorldCornerPositions()
    {
        Vector2[] positions = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            positions[i] = Camera.main.ViewportToWorldPoint(_cornersPositions[i]);
        }
        return positions;
    }

    public Vector2[] GetViewportCornerPositions()
    {        
        return _cornersPositions;
    }


    void CalculateGridPositions()
    {
        Vector2[] positions = GetWorldCornerPositions();
        Vector2 ptl, ptr, pbl; // Corners positions for player
        ptl = positions[0]; // Top left
        ptr = positions[1]; // Top right
        pbl = positions[2]; // Bottom left

        Vector2 etl, etr, ebl; // Corners positions for enemy
        etl = positions[3]; // Top left
        etr = positions[4]; // Top right
        ebl = positions[5]; // Bottom left

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _playerGrid[i, j] = Vector2.Lerp(ptl, ptr, (0.5f + i) / 3.0f) + Vector2.Lerp(ptl, pbl, (0.5f + j) / 3.0f) - ptl;

                _enemyGrid[i, j] = Vector2.Lerp(etl, etr, (0.5f + i) / 3.0f) + Vector2.Lerp(etl, ebl, (0.5f + j) / 3.0f) - etl;

            }
        }
    }

    void CalculateGridMesh()
    {
        Vector2[] positions = GetWorldCornerPositions();

        Vector3[] points = new Vector3[4 * 18];
        int[] triangles = new int[6 * 18];
        Vector2[] uvs = new Vector2[4 * 18];
        Vector3[] normals = new Vector3[4 * 18];

        for (int n = 0; n < 2; n++)
        {
            Vector2 right = positions[1 + n * 3] - positions[n * 3];
            Vector2 down = positions[2 + n * 3] - positions[n * 3];

            for (int i = 0; i < 9; i++)
            {
                int j = i % 3;
                int k = i / 3;

                points[(n * 9 + i) * 4] = positions[n * 3] + right * (j / 3f + 0.01f) + down * (k / 3f + 0.01f);
                points[(n * 9 + i) * 4 + 1] = positions[n * 3] + right * ((j + 1) / 3f - 0.01f) + down * (k / 3f + 0.01f);
                points[(n * 9 + i) * 4 + 2] = positions[n * 3] + right * (j / 3f + 0.01f) + down * ((k + 1) / 3f - 0.01f);
                points[(n * 9 + i) * 4 + 3] = positions[n * 3] + right * ((j + 1) / 3f - 0.01f) + down * ((k + 1) / 3f - 0.01f);

                triangles[(n * 9 + i) * 6] = 0 + (n * 9 + i) * 4;
                triangles[(n * 9 + i) * 6 + 1] = 1 + (n * 9 + i) * 4;
                triangles[(n * 9 + i) * 6 + 2] = 2 + (n * 9 + i) * 4;
                triangles[(n * 9 + i) * 6 + 3] = 2 + (n * 9 + i) * 4;
                triangles[(n * 9 + i) * 6 + 4] = 1 + (n * 9 + i) * 4;
                triangles[(n * 9 + i) * 6 + 5] = 3 + (n * 9 + i) * 4;

                uvs[(n * 9 + i) * 4] = new Vector2(0, 1);
                uvs[(n * 9 + i) * 4 + 1] = new Vector2(1, 1);
                uvs[(n * 9 + i) * 4 + 2] = new Vector2(0, 0);
                uvs[(n * 9 + i) * 4 + 3] = new Vector2(1, 0);

                normals[(n * 9 + i) * 4] = Vector3.back;
                normals[(n * 9 + i) * 4 + 1] = Vector3.back;
                normals[(n * 9 + i) * 4 + 2] = Vector3.back;
                normals[(n * 9 + i) * 4 + 3] = Vector3.back;
            }

        }

        _mesh.vertices = points;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;
        _mesh.normals = normals;

        GameObject obj = new GameObject();
        obj.name = "Grid Mesh Object";
        MeshFilter filter = obj.AddComponent<MeshFilter>();
        filter.mesh = _mesh;
        MeshRenderer rend = obj.AddComponent<MeshRenderer>();
        Texture2D text = new Texture2D(2, 1);
        text.filterMode = FilterMode.Point;
        text.SetPixel(0, 0, new Color(0f, 0.6f, 0.7f, 1f));
        text.SetPixel(1, 0, new Color(0.8f, 0f, 0f, 1f));
        text.Apply();
        Material material = new Material(Shader.Find("Unlit/Texture")) { mainTexture = text };
        rend.material = material;
    }

    void CalculateSpriteOrders()
    {
        List<KeyValuePair<int, Vector2>> positionList = new List<KeyValuePair<int, Vector2>>();

        // Order player positions
        positionList.Clear();
        for (int i = 0; i < 9; i++)
        {
            KeyValuePair<int, Vector2> pair = new KeyValuePair<int, Vector2>(i, _playerGrid[i % 3, i / 3]);
            positionList.Add(pair);
        }
        positionList.Sort(delegate (KeyValuePair<int, Vector2> pair1, KeyValuePair<int, Vector2> pair2) { return pair2.Value.y.CompareTo(pair1.Value.y); });
        for (int i = 0; i < 9; i++)
        {
            _spriteOrder[i] = positionList[i].Key;
        }

        // Order enemy positions
        positionList.Clear();
        for (int i = 0; i < 9; i++)
        {
            KeyValuePair<int, Vector2> pair = new KeyValuePair<int, Vector2>(i, _enemyGrid[i % 3, i / 3]);
            positionList.Add(pair);
        }
        positionList.Sort(delegate (KeyValuePair<int, Vector2> pair1, KeyValuePair<int, Vector2> pair2) { return pair2.Value.y.CompareTo(pair1.Value.y); });
        for (int i = 0; i < 9; i++)
        {
            _spriteOrder[i + 9] = positionList[i].Key;
        }

    }

    void CheckMouseAgainstUnits()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 0f);
        if (hits.Length > 0)
        {
            GameObject closer = gameObject; // Just random assignement
            float minDist = 9999f;
            for (int i = 0; i < hits.Length; i++)
            {
                float dist = (hits[i].collider.transform.position - mousePos).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    closer = hits[i].collider.gameObject;
                }
            }

            int tile = closer.transform.root.GetComponentInChildren<BaseUnit>().GetGridPosition();
            if (tile >= 0)
                SetTileHighlighted(tile);
            else
                ClearHighlights();
        }
    }

    void CheckMouseAgainstGrid()
    {
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        int result = -1;

        for (int n = 0; n < 2; n++)
        {
            Vector2 right = _cornersPositions[1 + n * 3] - _cornersPositions[n * 3];
            Vector2 down = _cornersPositions[2 + n * 3] - _cornersPositions[n * 3];

            Vector2 result1 = GetIntersectionPointCoordinates(_cornersPositions[n * 3], _cornersPositions[n * 3 + 1], mousePos, mousePos + down);
            float j = (result1.x - _cornersPositions[n * 3].x) / right.x;

            Vector2 result2 = GetIntersectionPointCoordinates(_cornersPositions[n * 3], _cornersPositions[n * 3 + 2], mousePos, mousePos + right);
            float k = (result2.y - _cornersPositions[n * 3].y) / down.y;

            if (j > 1f || j < 0f || k > 1f || k < 0f)
            {
                // Outside
            }
            else
            {
                // Inside
                j = Mathf.Floor(j * 3f);
                k = Mathf.Floor(k * 3f);

                result = (int)(j + k * 3) + n * 9;

            }
        }
        if (result >= 0)
            SetTileHighlighted(result);
        else
            ClearHighlights();

    }

    void SetTileHighlighted(int tile)
    {
        // Fine the list of tiles depending on mouse position and the shape selected
        List<int> positions = new List<int>();
        switch (_targetShape)
        {
            case TargetShape.Single:
                positions.Add(tile);
                break;
            case TargetShape.Horizontal:
                int row = (tile / 3) * 3;
                for (int i = 0; i < 3; i++)
                {
                    positions.Add(row + i);
                }
                break;
            case TargetShape.Veritcal:
                int column = tile % 3 + (tile / 9) * 9;
                for (int i = 0; i < 3; i++)
                {
                    positions.Add(column + i * 3);
                }
                break;
            case TargetShape.Cross:
                int x = tile % 3;
                int side = tile / 9;
                int y = (tile - side * 9) / 3;

                for (int i = side * 9 + y * 3 + Mathf.Max(0, x - 1); i < side * 9 + y * 3 + Mathf.Min(2, x + 1) + 1; i++)
                {
                    positions.Add(i);
                }
                for (int i = Mathf.Max(tile - 3, side * 9 + x + 3 - 3 * (y % 2)); i < Mathf.Min(tile + 9, side * 9 + x + 9 + 3 * (y % 2)); i += 6)
                {
                    positions.Add(i);
                }
                break;
        }

        // Highlight the tiles list
        HighlightTiles(positions.ToArray());
        // Highlight the unit upon tiles
        HighlightUnits(positions);
    }

    void HighlightUnits( List<int> positions)
    {
        // Reset old highlight
        if (_mouseoverUnits.Count > 0)
        {
            for (int i = 0; i < _mouseoverUnits.Count; i++)
            {
                SpriteRenderer rend = _mouseoverUnits[i].GetComponentInChildren<SpriteRenderer>();
                rend.color = Color.white;
                rend.transform.localScale = Vector2.one * 0.5f;
            }
        }

        // Find if there is a unit and highlight it
        _mouseoverUnits.Clear();
        if (positions.Count > 0)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                BaseUnit unit = _units[positions[i]];
                if (unit)
                {
                    _mouseoverUnits.Add(unit);
                    SpriteRenderer rend = unit.GetComponentInChildren<SpriteRenderer>();
                    rend.color = Color.Lerp(Color.white, Color.red, 0.5f + 0.2f * Mathf.Sin(Time.time * 15f));
                    rend.transform.localScale = Vector2.one * (0.6f + 0.1f * Mathf.Sin(Time.time * 4f + 1));
                }
            }

        }
        else
        {

        }
    }

    void HighlightTiles(int[] tiles)
    {
        //Debug.Log("highlighting tiles: " + numbers.Length);
        Vector2[] uvs = new Vector2[4 * 18];
        for (int i = 0; i < 18; i++)
        {
            // Check if actual tile is between the ones to highlight
            bool check = false;
            for (int n = 0; n < tiles.Length; n++)
            {
                if (i == tiles[n])
                    check = true;
            }

            // Set each UV based on highlight list
            if (check)
            {
                uvs[i * 4] = new Vector2(0.5f, 1f);
                uvs[i * 4 + 1] = new Vector2(1f, 1f);
                uvs[i * 4 + 2] = new Vector2(0.5f, 0f);
                uvs[i * 4 + 3] = new Vector2(1f, 0f);
            }
            else
            {
                uvs[i * 4] = new Vector2(0, 1);
                uvs[i * 4 + 1] = new Vector2(0.5f, 1);
                uvs[i * 4 + 2] = new Vector2(0, 0);
                uvs[i * 4 + 3] = new Vector2(0.5f, 0);
            }
        }
        _mesh.uv = uvs;
    }

    void ClearHighlights()
    {
        HighlightTiles(new int[] { });
        HighlightUnits(new List<int> { });
    }

    public void SetTargetType(bool tile)
    {
        _targetTile = tile;
    }

    public Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
    {
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

        if (tmp == 0)
        {
            // No solution!
            Debug.Log("parallel lines... some mistake!");
            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

        return new Vector2(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu
        );
    }

    public List<BaseUnit> GetTargetUnits()
    {
        return _mouseoverUnits;
    }

    #endregion

    #region Tiles methods

    /// <summary>
    /// Call this when the turn switch to update the ground hazards and apply effects
    /// </summary>
    /// <param name="player"> True for player turn </param>
    public void UpdateNewTurnHazards(bool player)
    {
        // TODO
        // Check if tile is occupied and eventually apply effect
        // Update timer and eventually clear the tile if the effect is expired
    }

    void ClearTile(int tile)
    {
        _hazards[tile] = null;
    }

    void AddHazard(int tile, GameObject hazard) // TODO change gameobject eventually
    {
        _hazards[tile] = hazard;
    }

    #endregion

    #region Unit methods

    public void PlaceUnitAt(BaseUnit unit, int position)
    {
        // Check if tile is occupied and remove the unit in case
        BaseUnit oldUnit = _units[position];
        if (oldUnit)
            RemoveUnitFromBattleGround(position);

        // Set position
        int j = position % 3;
        int k = position / 3;
        if (position < 9) // Player
            unit.transform.position = _playerGrid[j, k];
        else // Enemy
            unit.transform.position = _enemyGrid[j, k - 3];
        // Set sprite order both tile and UI
        unit.SetSpriteOrder(_spriteOrder[position]);

        unit.SetGridPosition(position);
        _units[position] = unit;

    }

    public void ClearBattleground()
    {
        for (int i = 0; i < 18; i++)
        {
            if (_units[i])
                RemoveUnitFromBattleGround(i);
        }
    }

    void RemoveUnitFromBattleGround(int position)
    {
        BaseUnit unit = _units[position];
        // TODO decide what to do with removed unit, place them in some graveyard or something
        unit.gameObject.SetActive(false);
        _units[position] = null;
    }


    public List<BaseUnit> GetUnitsSelected()
    {
        return _mouseoverUnits;
    }

    public void SetTargetShape(TargetShape shape)
    {
        _targetShape = shape;
    }

    #endregion
}