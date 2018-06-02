using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour, Entity
{
    // Base unit class
    
    public int MaxHP;

    private int _actualHP;
    private bool _player;
    private int _gridPosition = -1;


    public int GetActualHP()
    {
        throw new NotImplementedException();
    }

    public int GetMaxHP()
    {
        throw new NotImplementedException();
    }

    public bool IsPlayer()
    {
        throw new NotImplementedException();
    }

    public bool IsTargetable()
    {
        throw new NotImplementedException();
    }

    public void OnDeath()
    {
        throw new NotImplementedException();
    }

    public void SetGridPosition(int position)
    {
        _gridPosition = position;
    }

    public int GetGridPosition()
    {
        return _gridPosition;
    }
}
