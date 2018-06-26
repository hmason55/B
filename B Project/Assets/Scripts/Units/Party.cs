using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party {

	// The Party class

	// There can only be one character of each character ID per party. (Can't have 2 Thys and 2 Guys, but you can still have 2 Guardians and 2 Warriors)
	List<BaseUnit> party;
	Deck deck;
	int maxSize = 4;

	public Party() {
		party = new List<BaseUnit>();
		deck = new Deck();

		AddUnit(BaseUnit.ID.Guy);

		CombineDecks();
	}

	public List<BaseUnit> PartyList {
		get{return party;}
	}

	public Deck DeckList {
		get{return deck;}
	}

	public int PartySize() {
		return party.Count;
	}

	public void AddUnit(BaseUnit.ID id) {
		// Add character by id, generate their attributes accordingly.
		UnitData unitData = new UnitData();
		//unitData
	}

	public void AddUnit(BaseUnit unit) {
		// Add an existing character
	}

	public void RemoveUnit(BaseUnit.ID id) {
		// Remove a unit from the party by their id.
	}

	public void CombineDecks() {
		deck.ClearDeck();
		deck.ClearReferenceDeck();

		foreach(BaseUnit unit in party) {
			foreach(CardData cardData in unit.DeckList.ReferenceDeck) {
				deck.AddCardToReferenceDeck(cardData);
			}
		}

		foreach(CardData cardData in deck.ReferenceDeck) {
			Debug.Log(cardData.Title);
		}
	}

}
