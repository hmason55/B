using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public GameObject PlayerUnitPrefab;

    // Units placed on battleground
    private List<BaseUnit> _playerUnits;

    void Awake()
    {
        _playerUnits = new List<BaseUnit>();
    }

    public List<BaseUnit> GetPlayerUnits()
    {
        return _playerUnits;
    }

    public void CreateRandomUnits()
    {
        // TEMP create a few player units
        for (int i = 0; i < 4; i++)
        {
            GameObject go = Instantiate(PlayerUnitPrefab);
            BaseUnit player = go.GetComponent<BaseUnit>();
            _playerUnits.Add(player);

            Battleground bg = FindObjectOfType<Battleground>();
            bg.PlaceUnitAt(player, i);
            
        }
    }
}



