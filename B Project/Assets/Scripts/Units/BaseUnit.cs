using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour, Entity
{
    // Base unit class

    // Name
    public string UnitName;
    // Max HP
    public int MaxHP;
    // Actual HP
    private int _actualHP;
    // Player or Enemy controlled
    private bool _player;
    // Tile position on grid
    private int _gridPosition = -1;
    // Unit UI
    private TextMesh _textMeshUI;
    // Unit sprite
    private SpriteRenderer _spriteRenderer;


    protected virtual void Awake()
    {        
        _actualHP = MaxHP;

        // Cache renderer
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Initialize UI
        GameObject UI = new GameObject("Unit UI");
        _textMeshUI = UI.AddComponent<TextMesh>();
        UI.transform.SetParent(transform);
        UpdateUI();
    }


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
        // Remove from screen

        // Remove cards in case maybe ?

        // Check if there are more alive or is game over


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
    
    public void DealDamage(int damage)
    {
        _actualHP -= damage;
        if (_actualHP > MaxHP)
            _actualHP = MaxHP;
        if (_actualHP < 1)
            OnDeath();

        UpdateUI();
    }

    void UpdateUI()
    {
        _textMeshUI.transform.position = transform.position + Vector3.up * 2.5f;
        _textMeshUI.text = UnitName + "\n" + _actualHP + "/" + MaxHP;
        _textMeshUI.fontSize = 24;
        _textMeshUI.characterSize = 0.1f;
        _textMeshUI.anchor = TextAnchor.MiddleCenter;        
    }

    public void SetSpriteOrder(int n)
    {
        /*
        _spriteRenderer.sortingOrder = n;

        Renderer rend = _textMeshUI.GetComponent<Renderer>();
        rend.sortingOrder = n;
        */
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingOrder = n;
        }
    }

}
