using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleground : MonoBehaviour
{
    // Grid corner positions to set on Unity
    [HideInInspector]
    public Vector2[] CornersPositions = new Vector2[6];

    // Screen position of each tile
    private Vector2[,] _playerGrid = new Vector2[3, 3];
    private Vector2[,] _enemyGrid = new Vector2[3, 3];
    // Grid Mesh
    private Mesh _mesh;
    
    void Start()
    {
        _mesh = new Mesh();
        CalculateGridPositions();
        CalculateGridMesh();        
    }

    void Update()
    {
        CheckMouseAgainstGrid();        
    }


    void CalculateGridPositions()
    {
        Vector2 ptl, ptr, pbl; // Corners positions for player
        ptl = CornersPositions[0]; // Top left
        ptr = CornersPositions[1]; // Top right
        pbl = CornersPositions[2]; // Bottom left

        Vector2 etl, etr, ebl; // Corners positions for enemy
        etl = CornersPositions[3]; // Top left
        etr = CornersPositions[4]; // Top right
        ebl = CornersPositions[5]; // Bottom left


        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _playerGrid[i, j] = (Vector2.Lerp(ptl, ptr, (0.5f + i) / 3.0f) + Vector2.Lerp(ptl, pbl, (0.5f + j) / 3.0f)) * 0.5f;

                _enemyGrid[i, j] = (Vector2.Lerp(etl, etr, (0.5f + i) / 3.0f) + Vector2.Lerp(etl, ebl, (0.5f + j) / 3.0f)) * 0.5f;
            }
        }
    }

    void CalculateGridMesh()
    {
        Vector3[] points = new Vector3[4 * 18];
        int[] triangles = new int[6 * 18];
        Vector2[] uvs = new Vector2[4 * 18];
        Vector3[] normals = new Vector3[4 * 18];

        for (int n = 0; n < 2; n++)
        {
            Vector2 right = CornersPositions[1 + n * 3] - CornersPositions[n * 3];
            Vector2 down = CornersPositions[2 + n * 3] - CornersPositions[n * 3];

            for (int i = 0; i < 9; i++)
            {
                int j = i % 3;
                int k = i / 3;

                points[(n * 9 + i) * 4] = CornersPositions[n * 3] + right * (j / 3f + 0.01f) + down * (k / 3f + 0.01f);
                points[(n * 9 + i) * 4 + 1] = CornersPositions[n * 3] + right * ((j + 1) / 3f - 0.01f) + down * (k / 3f + 0.01f);
                points[(n * 9 + i) * 4 + 2] = CornersPositions[n * 3] + right * (j / 3f + 0.01f) + down * ((k + 1) / 3f - 0.01f);
                points[(n * 9 + i) * 4 + 3] = CornersPositions[n * 3] + right * ((j + 1) / 3f - 0.01f) + down * ((k + 1) / 3f - 0.01f);

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
        MeshFilter filter= obj.AddComponent<MeshFilter>();
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

    void CheckMouseAgainstGrid()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int result = -1;

        for (int n = 0; n < 2; n++)
        {
            Vector2 right = CornersPositions[1 + n * 3] - CornersPositions[n * 3];
            Vector2 down = CornersPositions[2 + n * 3] - CornersPositions[n * 3];

            Vector2 result1 = GetIntersectionPointCoordinates(CornersPositions[n * 3], CornersPositions[n * 3 + 1], mousePos, mousePos + down);
            float j = (result1.x - CornersPositions[n * 3].x) / right.x;

            Vector2 result2 = GetIntersectionPointCoordinates(CornersPositions[n * 3], CornersPositions[n * 3 + 2], mousePos, mousePos + right);
            float k = (result2.y - CornersPositions[n * 3].y) / down.y;

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

        HighlightTiles(new int[] { result });
        
    }
    

    void HighlightTiles(int[] numbers)
    {
        Vector2[] uvs = new Vector2[4 * 18];
        for (int i = 0; i < 18; i++)
        {
            // Check if actual tile is between the ones to highlight
            bool check = false;
            for (int n = 0; n < numbers.Length; n++)
            {
                if (i == numbers[n])
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
}