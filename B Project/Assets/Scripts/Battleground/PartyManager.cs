using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PartyManager : MonoBehaviour
{
    // TEMP
    public GameObject PlayerUnitPrefab;
    public GameObject PlayerUIPrefab;

    // Units placed on battleground
    private List<BaseUnit> _playerUnits;

    private Deck _deck;

    public Deck PartyDeck {
    	get{return _deck;}
    	set{_deck = value;}
    }

    void Awake()
    {
        _playerUnits = new List<BaseUnit>();
        _deck = new Deck();
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

            player.Threat = 0.25f;

            Battleground bg = FindObjectOfType<Battleground>();
            bg.PlaceUnitAt(player, i);

            //create UI
            CharacterUI UI = Instantiate(PlayerUIPrefab).GetComponent<CharacterUI>();
            player.AssignUI(UI);

             
        }
    }

    public void ChangeThreat(BaseUnit unit, float value)
    {
        if( _playerUnits.Count==1)
        {
            // only 1 guy left
            if (_playerUnits.Contains(unit))
                unit.Threat = 1.0f;
            else
                Debug.Log("something is wrong...");
        }
        float opposite = -value / (_playerUnits.Count - 1);
        for (int i = 0; i < _playerUnits.Count; i++)
        {
            BaseUnit u = _playerUnits[i];
            if (u == unit)
                u.Threat += value;
            else
                u.Threat += opposite;
            u.UpdateUI();
        }

    }

    #region Save/Load methods
    public void LoadDefaultParty() {
    	LoadPartyFromFile(-1);
    }

	void SavePartyToFile(int saveSlot) {
		string partyDataPath = "SaveData/default/party";
		string unitDataPath = "SaveData/default/UnitData/";
		string deckDataPath = "SaveData/default/DeckData/";

		if(saveSlot >= 0) {
			partyDataPath = "SaveData/Slot_" + saveSlot + "/party";
			unitDataPath = "SaveData/Slot_" + saveSlot + "/UnitData/";
			deckDataPath = "SaveData/Slot_" + saveSlot + "/DeckData/";
		}

		StreamWriter writer = new StreamWriter("Assets/Resources/"+partyDataPath, false);
		for(int i = 0; i < _playerUnits.Count; i++) {
			if(i < _playerUnits.Count-1) {
				writer.WriteLine(_playerUnits[i].UnitID + ",");
			} else {
				writer.WriteLine(_playerUnits[i].UnitID);
			}
		}
	    writer.Close();
    }

	void LoadPartyFromFile(int saveSlot) {
		string partyDataPath = "SaveData/default/party";
		string unitDataPath = "SaveData/default/UnitData/";
		string deckDataPath = "SaveData/default/DeckData/";

		if(saveSlot >= 0) {
			partyDataPath = "SaveData/Slot_" + saveSlot + "/party";
			unitDataPath = "SaveData/Slot_" + saveSlot + "/UnitData/";
			deckDataPath = "SaveData/Slot_" + saveSlot + "/DeckData/";
		}

		TextAsset partyFile = Resources.Load<TextAsset>(partyDataPath);

		if(partyFile == null) {
			return;
		}

		// Remove newline characters
		string formattedPartyFile = partyFile.text.Replace(System.Environment.NewLine, String.Empty);
		string[] unitIDs = formattedPartyFile.Split(',');

		Debug.Log("Loading party of " + unitIDs.Length);
		for(int i = 0; i < unitIDs.Length; i++) {

			UnitData unitData = null;
			Deck deck = null;

			// Load the UnitData
			TextAsset unitTextAsset = Resources.Load<TextAsset>(unitDataPath + unitIDs[i]);
			if(unitTextAsset != null) {
				unitData = JsonUtility.FromJson<UnitData>(unitTextAsset.text);
			}

			// Load the DeckData
			TextAsset deckTextAsset = Resources.Load<TextAsset>(deckDataPath + unitIDs[i]);
			if(deckTextAsset != null) {
				deck = new Deck(deckTextAsset);
			}

			if(unitData != null && deck != null) {
				GameObject go = Instantiate(PlayerUnitPrefab);
				BaseUnit unit = go.GetComponent<BaseUnit>();
				unit.UnitID = unitData.UnitID;
				unit.SetUnitData(unitData);
				unit.LoadFromJson(-1);

				foreach(CardData cardData in deck.ReferenceDeck) {
					cardData.Owner = unit;
					_deck.AddCardToReferenceDeck(cardData);
				}

				_playerUnits.Add(unit);

                // set base threat
                unit.Threat = 1.0f/ unitIDs.Length;

                //create UI
                CharacterUI UI = Instantiate(PlayerUIPrefab).GetComponent<CharacterUI>();
                unit.AssignUI(UI);
                UI.SetEnemyTell(false);

                Battleground bg = FindObjectOfType<Battleground>();
           		bg.PlaceUnitAt(unit, i);
			}
		}
	}
	#endregion
}



