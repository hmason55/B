using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public delegate void LinkDelegate();

public class BaseUnit : MonoBehaviour, Entity
{
    // Base unit class

    // Presets
	public enum ID {
		None,
		Thy,
		Guy,
		Sly,
		Die,
		Droll,
		Ninjaku,
		Gambler,
		Ramfist,
		BunBun,
		Bloodfiend,
		Spellblade,
		Lizardman
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

    // Unit Sounds
    private AudioSource _audioSource;

    // Threat level
    public float Threat;
    
    // Unit deck list
    protected Deck _deck;

    // Link delegates
    private event LinkDelegate _linkEvent;


    public Deck DeckList {
    	get{return _deck;}
    	set{_deck = value;}
    }

	public List<BaseStatus> Statuses {
		get{return _statuses;}
    }

    static float _screenWidth = 60f;

    
    protected virtual void Awake()
    {
        _actualHP = MaxHP;

        // Cache renderer
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Cache audio source
        _audioSource = GetComponent<AudioSource>();


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
        _audioSource.panStereo = transform.position.x / (_screenWidth / 2f);
    }

    public int GetGridPosition()
    {
        return _gridPosition;
    }

    public void MoveToTile(int tile)
    {        
        StartCoroutine(Move(tile));
    }

    public IEnumerator Move(int tile)
    {
        Vector2 start = transform.position;
        Vector2 end =  Battleground.Instance.GetPositionFromTile(tile); ;
        
        float duration = 1f;
        while (duration > 0)
        {
            transform.position = Vector2.Lerp(end, start, duration);
            duration -= Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        Battleground.Instance.UpdateUnitposition(this, tile);
    }

    public void DealDamage(int damage,BaseUnit attacker)
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

        // Add link damage if attacker has it
        BaseStatus linkDamage = attacker.SearchStatusLike(typeof(LinkDamageStatus));
        if (linkDamage != null)
            damage += linkDamage.Strength;

        // Execute Link event
        attacker.ExecuteLinkEvent();

		int totalDamage = damage - totalBlock;
    	if(totalDamage < 0) {
    		totalDamage = 0;
			SpawnBattleText("Blocked (-" + damage + ")");

    	} else {
        	_actualHP -= totalDamage;

        	if(totalBlock > 0) {
				// Partial Block
				SpawnBattleText(totalDamage.ToString() + " (-" + (damage-totalDamage) + ")");

				// Shake Camera
				Camera.main.GetComponent<CameraRumble>().Rumble();

				// Play Sound Effect
				_audioSource.clip = Resources.Load<AudioClip>("Sounds/fx/attackHitArmor01");
				_audioSource.Play();
			} else {
				// Clean Hit
				SpawnBattleText(totalDamage.ToString());

				// Shake Camera
				Camera.main.GetComponent<CameraRumble>().Rumble();

				// Play Sound Effect
				_audioSource.clip = Resources.Load<AudioClip>("Sounds/fx/attackHitFlesh");
				_audioSource.Play();
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
		charUI.transform.localPosition = new Vector3(0f, 0f, _spriteRenderer.transform.position.z);
        charUI.SetUnit(this);
        _characterUI = charUI;
        //Debug.Log("assigning " + charUI + " to " + this.UnitName);
    }

    public void UpdateUI()
    {
        if (_characterUI)
            _characterUI.SetUnit(this);
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
		Vector3 screenPosition = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z));
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

    public BaseStatus SearchStatusLike(Type status)
    {
        //Debug.Log("search " + status + "   on " + UnitName+ "   with status n: "+_statuses.Count);
        for (int i = 0; i < _statuses.Count; i++)
        {
            //Debug.Log("status: " + _statuses[i]);
            
            if (status== _statuses[i].GetType())
                return _statuses[i];
        }
        return null;
    }

    public void AddStatus(BaseStatus newStatus)
    {
        BaseStatus oldStatus = SearchStatusLike(newStatus.GetType());
        if (oldStatus == null)
            _statuses.Add(newStatus);
        else
        {
            oldStatus.Update(newStatus);
            newStatus.DestroyStatusExecute();
        }
        UpdateUI();
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
            {
                _statuses[i].DestroyStatusExecute();
                _statuses.RemoveAt(i);
            }
        }
        UpdateUI();
    }
    
    public void ExecuteEndTurnStatuses()
    {
        // End turn update
        for (int i = 0; i < _statuses.Count; i++)
        {
            _statuses[i].EndTurnExecute();
        }

        // Remove those expired
        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            if (_statuses[i].Duration < 1)
            {
                _statuses[i].DestroyStatusExecute();
                _statuses.RemoveAt(i);
            }
        }
        UpdateUI();
    }    

    public void ClearAllStatuses()
    {
        _statuses.Clear();
    }

    public void ExecuteLinkEvent()
    {
        if (_linkEvent!=null)
            _linkEvent();
    }

    public void AddLinkEvent(LinkDelegate del)
    {
        _linkEvent += del;
    }

    public void RemoveLinkEvent(LinkDelegate del)
    {
        _linkEvent -= del;
    }

    #endregion
}
