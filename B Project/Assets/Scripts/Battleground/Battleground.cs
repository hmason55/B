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


    void Start()
    {
        CalculateGridPositions();
    }

    void Update()
    {

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
    
}