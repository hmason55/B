using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool _playerTurn = true;

    // AI Manager cache
    private AIManager _aiManager;
    // Battleground cache
    private Battleground _battleground;

    void Start()
    {
        _aiManager = FindObjectOfType<AIManager>();
        _battleground = FindObjectOfType<Battleground>();
    }

    public void StartGame()
    {
        // Place enemies 

        // Draw enemy cards

    }

    public void StartPlayerTurn()
    {
        _playerTurn = true;

        // Draw hand

        // Turn hand UI and cards on

        // Enable battleground targeting
        _battleground.SetTargetShape(TargetShape.Single);

        // Draw next AI cards and show them 
        _aiManager.DealAICards();
        

    }

    public void StartEnemyTurn()
    {
        // Turn hand UI off

        // Disable battleground targeting
        _battleground.SetTargetShape(TargetShape.None);

        // Enable AI
        StartCoroutine(_aiManager.StartAITurn());
    }


    public void BattleWon()
    {
        Debug.Log("Battle is over you won!!!");
    }

    public void BattleLost()
    {
        Debug.Log("Battle is lost, game over!");
    }

}
