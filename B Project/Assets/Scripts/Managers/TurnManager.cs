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
	// Resource manager cache
    private ResourceManager _resourceManager;

    void Start()
    {
        _aiManager = FindObjectOfType<AIManager>();
        _battleground = FindObjectOfType<Battleground>();
        _partyManager = FindObjectOfType<PartyManager>();
        _hand = FindObjectOfType<Hand>();
		_resourceManager = FindObjectOfType<ResourceManager>();
    }

    public void StartGame()
    {
        Debug.Log("Starting battle");
        _battleground.SetTargetTile(true);

        // Place enemies 
        _aiManager.CreateRandomEnemies();

        // Place player units (replace with human positioning in future)
        _partyManager.LoadDefaultParty();

        // Draw player cards
        _hand.gameObject.SetActive(true);
        _hand.DeckList = _partyManager.PartyDeck;
        _hand.Deal();
		_resourceManager.CalculateResources();
		Debug.Log("Party deck size: "+_hand.DeckList.ReferenceDeck.Count);

        // Setup Items
        GlobalsManager.Instance.SetupItems();
    }

    public void StartPlayerTurn()
    {
        Debug.Log("Start Player Turn");
        _playerTurn = true;

        // Party global effects
        //_partyManager.UpdateStartTurnGlobalEffects();

        // Start turn status check
        List<BaseUnit> units = _partyManager.GetUnits();
        for (int i = units.Count-1; i >=0; i--)
        {
            BaseUnit unit = units[i];
            unit.ExecuteStartTurnStatuses();
        }

        // Turn hand UI and cards on
        _hand.gameObject.SetActive(true);

        // Draw hand cards
        _hand.Deal();

        // Enable battleground targeting
        _battleground.SetTargetShape(TargetShape.Single, TargetEntity.Enemy);
        _battleground.SetTargetTile(true);

        // Draw next AI cards and show them 
        _aiManager.DealAICards();
        
        _resourceManager.CalculateResources();
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

        // Update chars effects
        List<BaseUnit> units = _partyManager.GetUnits();
        for (int i = units.Count - 1; i >= 0; i--)
        {
            BaseUnit unit = units[i];
            unit.ExecuteEndTurnStatuses();
        }

        // Update global effects
        GlobalsManager.Instance.ApplyEndTurn(true);

        StartEnemyTurn();
    }
}
