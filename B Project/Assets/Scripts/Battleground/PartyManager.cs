using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{

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
}



