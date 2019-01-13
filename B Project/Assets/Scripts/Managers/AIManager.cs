using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : Singleton<AIManager>
{
    // TEMP Enemy Prefab
    public int EnemyNumber;
    public GameObject[] EnemyPrefabs;
    public GameObject EnemyUIPrefab;

    // Enemy units
    private List<BaseUnit> _enemies;
    // Party Manager cache
    private PartyManager _partyManager;
    // Turn Manager cache
    private TurnManager _turnManager;

    // Difficulty
    private float _difficulty;

    void Start()
    {
        _enemies = new List<BaseUnit>();

        _partyManager = FindObjectOfType<PartyManager>();
        _turnManager = FindObjectOfType<TurnManager>();

    }
    
    public void TestDiff()
    {
        int[] n = new int[3];
        int total = 0;
        for (int i = 0; i < 3; i++)
        {

            float min = Mathf.Max(Mathf.Lerp(0, EnemyPrefabs.Length - 3, _difficulty * 0.01f), 0);
            float max = Mathf.Min(Mathf.Lerp(3, EnemyPrefabs.Length , _difficulty * 0.01f), EnemyPrefabs.Length );
            n[i] = (int)Random.Range(min, max);
            total += n[i];
            Debug.Log("picked: " + n[i]);
        }
        Debug.Log("total: " + total);
    }

    public void CreateRandomEnemies()
    {
        // TEMP create a few enemies
        for (int i = 0; i < EnemyNumber; i++)
        {
            float min = Mathf.Max(Mathf.Lerp(0, EnemyPrefabs.Length - 3, _difficulty * 0.01f), 0);
            float max =Mathf.Min( Mathf.Lerp(3, EnemyPrefabs.Length , _difficulty * 0.01f),EnemyPrefabs.Length);
            GameObject go = Instantiate(EnemyPrefabs[(int)Random.Range(min, max)]);
            EnemyUnit enemy = go.GetComponent<EnemyUnit>();
            enemy.name += " (" + i + ")";
            _enemies.Add(enemy);

            int pos= Random.Range(9, 18);
            while (Battleground.Instance.GetUnitOnTile(pos)!=null)
            {
                pos = Random.Range(9, 18);
            }
            Battleground bg = FindObjectOfType<Battleground>();
            bg.PlaceUnitAt(enemy, pos);

            //create UI
            CharacterUI UI = Instantiate(EnemyUIPrefab).GetComponent<CharacterUI>();
            enemy.AssignUI(UI);
            UI.SetEnemyTell(true);

            enemy.DealAnotherCard();
        }
    }

    public IEnumerator StartAITurn()
    {
        yield return new WaitForSeconds(1);

        // Execute start turn field effects
        yield return  StartCoroutine(Battleground.Instance.UpdateNewTurnHazards(false));

        // Apply all effects/dots on each enemy
        foreach (BaseUnit unit in _enemies)
        {
            unit.ExecuteStartTurnStatuses();
        }

        // Check if they are all dead
        if (_enemies.Count < 1)
        {
            _turnManager.BattleWon();
            yield break;
        }

        // cycle AI units
        List<BaseUnit> units = _partyManager.GetUnits();
        for (int i = 0; i < _enemies.Count; i++)
        {
            EnemyUnit enemy = _enemies[i] as EnemyUnit;
            // Check if stunned and pass if so
            BaseStatus stun = enemy.SearchStatusLike(typeof(StunStatus));
            if (stun!=null)
            {
                Debug.Log(enemy.UnitName + " is skipping turn for stun");
               continue;
            }

            // Pick a target for the card

            // TODO random for now, change later
            // BaseUnit[] target = { units[Random.Range(0, units.Count)] };

            // Check if taunted, if not
            // Pick a target based on thread
            BaseStatus taunt= enemy.SearchStatusLike(typeof(TauntStatus));
            BaseUnit[] target;
            if (taunt != null)
                target =new BaseUnit[] { taunt.Owner };
            else
                target = new BaseUnit[] { _partyManager.PickUnitWithThreat() };

            // play the card
            enemy.GetNextCard().Play(target);

            yield return new WaitForSeconds(1);
            // TODO check animations and all effects for appropriate delay or add an event

            // Update the player units left
            units = _partyManager.GetUnits();
            if (units.Count<1)
            {
                _turnManager.BattleLost();
            }
        }

		foreach(BaseUnit unit in _enemies) {
        	unit.ExecuteEndTurnStatuses();
        }

        // Execute end turn globals
        yield return new WaitForSeconds(1);
        GlobalsManager.Instance.ApplyEndTurn(false);
        yield return new WaitForSeconds(1);

        // End turn
        _turnManager.StartPlayerTurn();
    }

    public void DealAICards()
    {
        // Deal cards
        for (int i = 0; i < _enemies.Count; i++)
        {
            EnemyUnit enemy = _enemies[i] as EnemyUnit;
            enemy.DealAnotherCard();
        }
    }

    public List<BaseUnit> GetEnemies()
    {
        return _enemies;
    }

    public void RemoveEnemy(BaseUnit unit)
    {
        Battleground.Instance.RemoveUnitFromBattleGround(unit.GetGridPosition());
        _enemies.Remove(unit);
        if (_enemies.Count<1)
        {
            Debug.Log("VICTORY!!!");
            Debug.Break();
        }
    }

    public void SetDifficulty(float diff)
    {
        _difficulty = diff;
    }
}
