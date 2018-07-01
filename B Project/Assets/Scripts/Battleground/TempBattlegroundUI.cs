using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TargetShape { Single, Horizontal, Veritcal, Cross, None }

public class TempBattlegroundUI : MonoBehaviour
{
    public GameObject UnitPrefab;

    public Toggle ToggleTile;

    

    private Battleground _battleGround;

    void Awake()
    {
        _battleGround = FindObjectOfType<Battleground>();
    }

    public void PlaceRandomUnit()
    {
        int pos = Random.Range(0, 18);
        BaseUnit unit = Instantiate(UnitPrefab).GetComponent<BaseUnit>();
        _battleGround.PlaceUnitAt(unit, pos);
    }

    public void FillWithUnits()
    {
        for (int i = 0; i < 18; i++)
        {
            BaseUnit unit = Instantiate(UnitPrefab).GetComponent<BaseUnit>();
            _battleGround.PlaceUnitAt(unit, i);
        }
    }

    public void ClearBattleground()
    {
        _battleGround.ClearBattleground();
    }

    public void OnToggle()
    {
        _battleGround.SetTargetTile(ToggleTile.isOn);
    }

    public void OnShapeToggle(int shape)
    {
        _battleGround.SetTargetShape((TargetShape)shape, TargetEntity.Enemy );
    }

}
