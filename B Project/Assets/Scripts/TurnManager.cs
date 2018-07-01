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
    // Party manager cache
    private PartyManager _partyManager;
    // Hand cache
    private Hand _hand;

    void Start()
    {
        _aiManager = FindObjectOfType<AIManager>();
        _battleground = FindObjectOfType<Battleground>();
        _partyManager = FindObjectOfType<PartyManager>();
        _hand = FindObjectOfType<Hand>();
    }

    public void StartGame()
    {
        Debug.Log("Starting battle");
        _battleground.SetTargetTile(false);

        // Place enemies 
        _aiManager.CreateRandomEnemies();

        // Place player units (replace with human positioning in future)
        _partyManager.LoadDefaultParty();

        // Draw player cards
        _hand.gameObject.SetActive(true);
        _hand.DeckList = _partyManager.PartyDeck;
        _hand.Deal();
		Debug.Log("Party deck size: "+_hand.DeckList.ReferenceDeck.Count);
    }

    public void StartPlayerTurn()
    {
        Debug.Log("Start Player Turn");
        _playerTurn = true;
             
        // Turn hand UI and cards on
        _hand.gameObject.SetActive(true);

        // Draw hand cards
        _hand.Deal();

        // Enable battleground targeting
        _battleground.SetTargetShape(TargetShape.Single, TargetEntity.Enemy);

        // Draw next AI cards and show them 
        _aiManager.DealAICards();
        

    }

    public void StartEnemyTurn()
    {
        Debug.Log("Start Enemy Turn");

        _playerTurn = false;

        // Turn hand UI off
        _hand.gameObject.SetActive(false);

        // Disable battleground targeting
        _battleground.SetTargetShape(TargetShape.None, TargetEntity.Enemy);

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

    public void EndPlayerTurn()
    {
        if (!_playerTurn)
            return;

        StartEnemyTurn();
    }
}
