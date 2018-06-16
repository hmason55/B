using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    // TEMP Enemy Prefab
    public GameObject EnemyPrefab;
    // Enemy units
    private List<EnemyUnit> _enemies;
    // Party Manager cache
    private PartyManager _partyManager;
    // Turn Manager cache
    private TurnManager _turnManager;


    void Start()
    {
        _enemies = new List<EnemyUnit>();

        _partyManager = FindObjectOfType<PartyManager>();
        _turnManager = FindObjectOfType<TurnManager>();

        
    }

    public void CreateRandomEnemies()
    {
        // TEMP create a few enemies
        for (int i = 0; i < 4; i++)
        {
            GameObject go = Instantiate(EnemyPrefab);
            EnemyUnit enemy = go.GetComponent<EnemyUnit>();
            _enemies.Add(enemy);

            Battleground bg = FindObjectOfType<Battleground>();
            bg.PlaceUnitAt(enemy, i + 9);

            enemy.DealAnotherCard();
        }
    }

    public IEnumerator StartAITurn()
    {
        yield return new WaitForSeconds(1);

        // Apply all effects/dots on each enemy

        // Check if they are all dead
        if (_enemies.Count < 1)
        {
            _turnManager.BattleWon();
            yield break;
        }

        // cycle AI units
        List<BaseUnit> units = _partyManager.GetPlayerUnits();
        for (int i = 0; i < _enemies.Count; i++)
        {
            // Pick a target for the card
            
            // TODO random for now, change later
            BaseUnit[] target = { units[Random.Range(0, units.Count)] };

            // play the card
            _enemies[i].GetNextCard().Play(target);

            yield return new WaitForSeconds(1);
            // TODO check animations and all effects for appropriate delay or add an event

            // Update the player units left
            units = _partyManager.GetPlayerUnits();
            if (units.Count<1)
            {
                _turnManager.BattleLost();
            }
        }
        // end turn
        _turnManager.StartPlayerTurn();
    }

    public void DealAICards()
    {
        // Deal cards
        for (int i = 0; i < _enemies.Count; i++)
        {
            EnemyUnit enemy = _enemies[i];
            enemy.DealAnotherCard();
        }
    }

    

}
