using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class BaseUnit : MonoBehaviour, Entity
{
    // Base unit class

    // Presets
	public enum ID {
		None,
		Thy,
		Guy,
		Sly,
		Die
	}

	public ID UnitID {
		get{return _unitID;}
		set{_unitID = value;}
	}

    // Name
    public string UnitName;

    // Max HP
    public int MaxHP;

    // Actual HP
    private int _actualHP;

    // Base HP
	[SerializeField] private int _baseHP;

    // Player or Enemy controlled
    protected bool _player=true;

    // For save/load
	private UnitData _unitData;

	// ID
	[SerializeField] private ID _unitID;

    // Deck Class

	[SerializeField] private Card.DeckClass _deckClass;

    // Tile position on grid
    private int _gridPosition = -1;

    // Unit UI
    //private TextMesh _textMeshUI;
    protected CharacterUI _characterUI;

    // Unit sprite
    private SpriteRenderer _spriteRenderer;
    private List<BaseStatus> _statuses = new List<BaseStatus>();

    // Threat level
    public float Threat;
    
    // Unit deck list
    protected Deck _deck;

    public Deck DeckList {
    	get{return _deck;}
    	set{_deck = value;}
    }

	public List<BaseStatus> Statuses {
		get{return _statuses;}
    }
    
    protected virtual void Awake()
    {
        _actualHP = MaxHP;

        // Cache renderer
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        /*
        // Initialize UI
        GameObject UI = new GameObject("Unit UI");
        _textMeshUI = UI.AddComponent<TextMesh>();
        UI.transform.SetParent(transform);
        UpdateUI();
        */
    }
    
    public int GetActualHP()
    {
    	return _actualHP;
    }

    public int GetMaxHP()
    {
        throw new NotImplementedException();
    }

    public bool IsPlayer()
    {
        return _player;
    }

    public bool IsTargetable()
    {
    	return true;
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
    	// Calculate the unit's block stacks.
    	int totalBlock = 0;
    	foreach(BaseStatus status in _statuses) {
    		if(status.GetType() == typeof(BlockStatus)) {
    			totalBlock = status.Strength;
    			goto damageCalc;
    		}
    	}

    	// Damage calculation including block mitigation.
		damageCalc:
		int totalDamage = damage - totalBlock;
    	if(totalDamage < 0) {
    		totalDamage = 0;
			SpawnBattleText("Blocked (-" + damage + ")");
    	} else {
        	_actualHP -= totalDamage;

        	if(totalBlock > 0) {
				// Partial Block
				SpawnBattleText(totalDamage.ToString() + " (-" + (damage-totalDamage) + ")");
			} else {
				// Clean Hit
				SpawnBattleText(totalDamage.ToString());
			}
    	}

		// Recalculate block stacks here.
		DamageBlock(damage);

		Debug.Log(UnitName + " takes " + totalDamage + " Damage.");

        if (_actualHP > MaxHP)
            _actualHP = MaxHP;
        if (_actualHP < 1)
            OnDeath();

        UpdateUI();
    }

    public void TickAllStatuses() {
		int numStatuses = _statuses.Count-1;
		for(int i = numStatuses; i >= 0; i--) {
			if(_statuses[i].Duration <= 1) {
				_statuses.RemoveAt(i);
			} else {
				_statuses[i].Duration--;
			}
    	}
    }

    // Remove block by means of damage.
	public void DamageBlock(int damage)
    {
		int numStatuses = _statuses.Count-1;
		for(int i = numStatuses; i >= 0; i--) {
			if(_statuses[i].GetType() == typeof(BlockStatus)) {
				_statuses[i].Strength -= damage;

				if(_statuses[i].Strength <= 0) {
					_statuses.RemoveAt(i);
				}
				return;
			}
		}
    }

    // Adding block to unit.
	public void GrantBlock(int block, int duration, BaseUnit owner)
    {
    	if(block <= 0) {
    		return;
    	}

		Debug.Log(UnitName + " gained "  + block + " Block.");

 		// Add the block status to the unit.
 		bool blockExists = false;
		int numStatuses = _statuses.Count-1;
		for(int i = numStatuses; i >= 0; i--) {
			if(_statuses[i].GetType() == typeof(BlockStatus)) {
				_statuses[i].Strength += block;

				if(_statuses[i].Strength > 0) {
					_statuses[i].Duration = 2;
				} else {
					_statuses.RemoveAt(i);
				}

				blockExists = true;
				goto done;
			}
		}

		done:
 		if(!blockExists) {
			_statuses.Add(new BlockStatus(block, duration, owner, this));
 		}

		SpawnBattleText("+" + block.ToString() + " Block");
		UpdateUI();
    }

    public void GrantDamageMultiplier(float multiplier, int duration, BaseUnit owner, Effect.RemovalCondition condition = Effect.RemovalCondition.None) {
		DamageMultiplierStatus status = new DamageMultiplierStatus(multiplier/100f, duration, owner, this);
		status.Condition = condition;
    	_statuses.Add(status);

		SpawnBattleText("+" + multiplier.ToString() + "% DMG");
		UpdateUI();
    }

    public void AssignUI(CharacterUI charUI)
    {
        charUI.transform.SetParent(transform);
        charUI.transform.localPosition = Vector2.zero;
        charUI.SetUnit(this);
        _characterUI = charUI;
        //Debug.Log("assigning " + charUI + " to " + this.UnitName);
    }

    public void UpdateUI()
    {
        if (_characterUI)
            _characterUI.SetUnit(this);
        /*
    	_textMeshUI.offsetZ = -1f;
        _textMeshUI.transform.position = transform.position + Vector3.up * 2.5f;
        _textMeshUI.text = UnitName + "\n" + _actualHP + "/" + MaxHP;
        _textMeshUI.fontSize = 24;
        _textMeshUI.characterSize = 0.1f;
        _textMeshUI.anchor = TextAnchor.MiddleCenter;  
        */
    }

    public void SetUIFocus(bool focus)
    {
        if (_characterUI)
            _characterUI.SetFocus(focus);
    }

    public void SetSpriteOrder(int n)
    {
        Vector3 pos = _spriteRenderer.transform.position;
        pos.z = 20 - n;
        _spriteRenderer.transform.position = pos;
        /*
        _spriteRenderer.sortingOrder = n;

        Renderer rend = _textMeshUI.GetComponent<Renderer>();
        rend.sortingOrder = n;
        
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingOrder = n;
        }
        */
        if (_characterUI)
        {
            pos = _characterUI.transform.position;
            pos.z = 20 - n;
            _characterUI.transform.position = pos;
        }
    }

    public void SpawnBattleText(string text)
    {
		GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
		if(canvas == null) {return;}

    	GameObject battleText = GameObject.Instantiate(Resources.Load("Prefabs/Battle Text")) as GameObject;
    	battleText.transform.SetParent(canvas.transform);
		Vector3 screenPosition = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z));
		battleText.transform.position = new Vector3(screenPosition.x * Screen.width, screenPosition.y * Screen.height, 0f);
		//Debug.Log(Camera.main.WorldToViewportPoint(transform.position));
    	battleText.GetComponent<Text>().text = text;
    }

    #region Save/Load methods
    public void SetUnitData(UnitData unitData) {
    	_unitData = unitData;
    }

	public void SaveUnitData() {
		if(_unitData == null) {
    		_unitData = new UnitData();
    	}

    	_unitData.UnitName = UnitName;
    	_unitData.UnitID = _unitID;
    	_unitData.DeckClass = _deckClass;
    	_unitData.BaseHP = _baseHP;
    	_unitData.SpritePath = _spriteRenderer.sprite.name;
    }

    public void LoadUnitData() {
    	if(_unitData == null) {
    		return;
    	}

		UnitName = _unitData.UnitName;
    	_unitID = _unitData.UnitID;
		_deckClass = _unitData.DeckClass;
		_baseHP = _unitData.BaseHP;
		_spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Units/"+_unitData.SpritePath);

		UpdateUI();
    }

	public void SaveToJson(int saveSlot) {
		SaveUnitData();

		string dataPath = "Assets/Resources/SaveData/default/UnitData/" + _unitID.ToString() + ".json";

		if(saveSlot >= 0) {
			dataPath = "Assets/Resources/SaveData/Slot_" + saveSlot + "/UnitData/" + _unitID.ToString() + ".json";
		}

		StreamWriter writer = null;
		if(!File.Exists(dataPath)) {
			writer = File.CreateText(dataPath);
		} else {
			writer = new StreamWriter(dataPath, false);
		}

		writer.WriteLine(JsonUtility.ToJson(_unitData));
    	writer.Close();

    }

    public void LoadFromJson(int saveSlot) {
		string dataPath = "SaveData/default/UnitData/" + _unitID.ToString();

		if(saveSlot >= 0) {
			dataPath = "SaveData/Slot_" + saveSlot + "/UnitData/" + _unitID.ToString();
		}

		TextAsset textAsset = Resources.Load<TextAsset>(dataPath);
		if(textAsset != null) {
			_unitData = JsonUtility.FromJson<UnitData>(textAsset.text);
			LoadUnitData();
		}
    }
    #endregion

    #region Status methods

    BaseStatus SearchStatusLike(BaseStatus status)
    {
        for (int i = 0; i < _statuses.Count; i++)
        {
            if (_statuses[i].GetType() ==  status.GetType())
                return _statuses[i];
        }
        return null;
    }

    public void AddStatus(BaseStatus newStatus)
    {
        BaseStatus oldStatus = SearchStatusLike(newStatus);
        if (oldStatus == null)
            _statuses.Add(newStatus);
        else
            oldStatus.Update(newStatus);
    }

    public void RemoveStatusOfType(Type type)
    {
        // Find all those with the same type
        List<int> found = new List<int>();
        for (int i = 0; i < _statuses.Count; i++)
        {
            if (_statuses[i].GetType() == type)
                found.Add(i);
        }

        // Delete them
        for (int i = found.Count-1; i >=0; i--)
        {
            _statuses.RemoveAt(found[i]);
        }
    }

    public void ExecuteStartTurnStatuses()
    {
        // Start turn update
        for (int i = 0; i < _statuses.Count; i++)
        {
            _statuses[i].StartTurnExecute();
        }

        // Remove those expired
        for (int i = _statuses.Count-1; i >=0; i--)
        {
            if (_statuses[i].Duration < 1)
                _statuses.RemoveAt(i);
        }

    }

    public void ClearAllStatuses()
    {
        _statuses.Clear();
    }

    #endregion
}
