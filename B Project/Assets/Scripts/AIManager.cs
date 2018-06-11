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


    void Start()
    {
        _enemies = new List<EnemyUnit>();

        _partyManager = FindObjectOfType<PartyManager>();

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


        // cycle AI units
        List<BaseUnit> units = _partyManager.GetPlayerUnits();
        for (int i = 0; i < _enemies.Count; i++)
        {
            // Pick a target for the card
            // TODO random for now, change later
            BaseUnit target = units[Random.Range(0, units.Count)];

            // play the card

            yield return new WaitForSeconds(1);
            // TODO check animations and all effects for appropriate delay or add an event

        }
        // end turn

    }

    public void DealAICards()
    {
        // Deal cards

        // Show UI on enemies

    }

}
