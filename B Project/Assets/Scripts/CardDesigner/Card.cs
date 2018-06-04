using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

[ExecuteInEditMode]
public class Card : MonoBehaviour {

	public enum DeckClass {
		Neutral,
		Guardian,
		Necromancer,
		Thief,
		Warrior
	}

	public string title;
	public Card.DeckClass deckType;
	public int resourceCost;
	public string description;
	public bool omitFromDeck;
	public bool requireTarget;

	public bool showObjectReferences;

	public Image backgroundImage;
	public Image artworkImage;
	public Text titleText;
	public Image descriptionImage;
	public Text descriptionText;
	public Image costImage;
	public Text costText;

	public bool showCardData;
	public CardData cardData;
	public TextAsset cardDataFile;

	public int zIndex = 0;

	Vector2 handPosition;
	bool raycastState = true;

	public Vector2 HandPosition {
		get{return handPosition;}
		set{handPosition = value;}
	}

	void Awake() {
		if(cardData == null) {
			cardData = new CardData();
		}
	}

	void Update() {


		SaveCardData();

		if(backgroundImage) {
			//backgroundImage.sprite = background;
			switch(deckType) {
				case DeckClass.Neutral:
					backgroundImage.color = Color.gray;
				break;

				case DeckClass.Guardian:
					// 130, 212, 219
					backgroundImage.color = new Color(0.51f, 0.83f, 0.86f);
				break;

				case DeckClass.Necromancer:
					// 186, 116, 201
					backgroundImage.color = new Color(0.73f, 0.45f, 0.79f);
				break;

				case DeckClass.Thief:
					// 187, 240, 87
					backgroundImage.color = new Color(0.73f, 0.95f, 0.34f);
				break;

				case DeckClass.Warrior:
					// 215, 67, 67
					backgroundImage.color = new Color(0.84f, 0.26f, 0.27f);
				break;
			}
		} 

		if(artworkImage) {
			//artworkImage.sprite = artwork;
		}

		if(titleText) {
			titleText.text = ParseText(title);

		} 
	
		if(descriptionImage) {
			//artworkImage.sprite = artwork;
		}

		if(descriptionText) {
			descriptionText.text = description;
		}

		if(costImage) {
			//costImage.sprite = artwork;
		}

		if(costText) {
			if(resourceCost < 0) {
				costText.text = "X";
			} else {
				costText.text = resourceCost.ToString();
			}
		}
	}

	public void LoadCardData() {
		if(cardData != null) {
			title = cardData.Title;
			deckType = cardData.DeckType;
			resourceCost = cardData.ResourceCost;
			description = cardData.Description;
			omitFromDeck = cardData.OmitFromDeck;
			requireTarget = cardData.RequireTarget;
		}
	}

	public void SaveCardData() {
		if(cardData != null) {
			cardData.Title = title;
			cardData.DeckType = deckType;
			cardData.ResourceCost = resourceCost;
			cardData.Description = description;
			cardData.OmitFromDeck = omitFromDeck;
			cardData.RequireTarget = requireTarget;
		}
	}

	string ParseText(string text) {
		text = text.Replace("63", "?");
		text = text.Replace("33", "!");
		text = text.Replace("_", "");
		return text;
	}

	public void SnapToHand() {
		GetComponent<RectTransform>().anchoredPosition = handPosition;
	}

	public void DisableRaycast() {
		backgroundImage.raycastTarget = false;
	}

	public void ResumeRaycastState() {
		backgroundImage.raycastTarget = raycastState;
	}
}
