using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

[ExecuteInEditMode]
public class Card : MonoBehaviour {

	public enum CharacterType {
		Any,
		Thy,
		Guy,
		Sly,
		Die
	}

	public enum DeckClass {
		Neutral,
		Guardian,
		Necromancer,
		Thief,
		Warrior
	}

	public enum TargetType {
		None,
		Self,
		Enemy,
		Ally,
		RandomEnemy,
		RandomAlly,
	}

	public enum EffectType {
		Damage,
		Block,
		Heal,
		Stun,
		Push,
		Resource
	}

	// Variables used by CardData
	public string title;
	public Card.CharacterType characterType;
	public Card.DeckClass deckType;
	public int resourceCost;
	public string description;
	public bool omitFromDeck;
	public bool requireTarget;
	public Card.TargetType targetType;
	public bool areaOfEffect;
	public bool[] targetArea = new bool[9];
	public List<Effect> effects = new List<Effect>();


	public bool showObjectReferences;

	public List<bool> effectFoldouts = new List<bool>();
	public Image backgroundImage;
	public Image artworkImage;
	public Text titleText;
	public Text ownerText;
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

	BaseUnit owner;

	public BaseUnit Owner {
		get{return owner;}
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

		if(owner) {
			ownerText.text = ParseText(owner.UnitName);
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

    public void Play(BaseUnit[] targets)
    {
        if (cardData.RequireTarget)
        {
            if (targets.Length > 0)
            {
                Debug.Log("Playing: " + title + " to " + targets[0].UnitName);
                Debug.Log(effects.Count + " effects");

                foreach (Effect effect in effects)
                {
                    ApplyToTargets(targets, effect);
                }
                
                if (transform.parent) // Added check in case the card belong to AI, so no GO
                {// Player card
                    Hand hand = transform.parent.GetComponent<Hand>();
                    if (hand != null)
                    {
                        hand.Remove(this);
                    }
                }

            }
            else
            {
                Debug.Log("no targets, canceling drag.");
                GetComponent<CardDragHandler>().CancelDrag();
            }
        }
        else
        {
            Debug.Log("Playing card, no target required.");
            //Play card since it doesn't need a target.
        }
    }

	void ApplyToTargets(BaseUnit[] targets, Effect effect) {
		Debug.Log("Applying effects");
		switch(effect.effectType) {
			case EffectType.Damage:
				foreach(BaseUnit target in targets) {
					if(ParseTargetType(effect.targetType, target.IsPlayer())) {
						Debug.Log("Deal " + effect.effectValue);
						target.DealDamage(effect.effectValue);
					}
				}
			break;

			case EffectType.Block:
				foreach(BaseUnit target in targets) {

				}
			break;
		}
	}

	public void LoadCardData() {
		if(cardData != null) {
			title = cardData.Title;
			characterType = cardData.CharacterType;
			deckType = cardData.DeckType;
			resourceCost = cardData.ResourceCost;
			description = cardData.Description;
			omitFromDeck = cardData.OmitFromDeck;
			requireTarget = cardData.RequireTarget;
			targetType = cardData.TargetType;
			areaOfEffect = cardData.AreaOfEffect;
			targetArea = cardData.TargetArea;
			effects = cardData.Effects;

			// Do not save.
			owner = cardData.Owner;
		}
	}

	public void SaveCardData() {
		if(cardData != null) {
			cardData.Title = title;
			cardData.CharacterType = characterType;
			cardData.DeckType = deckType;
			cardData.ResourceCost = resourceCost;
			cardData.Description = description;
			cardData.OmitFromDeck = omitFromDeck;
			cardData.RequireTarget = requireTarget;
			cardData.TargetType = targetType;
			cardData.AreaOfEffect = areaOfEffect;
			cardData.TargetArea = targetArea;
			cardData.Effects = effects;
		}
	}

	bool ParseTargetType(TargetType type, bool isPlayer) {
		if(type == TargetType.Ally && isPlayer) {
			return true;
		} else if(type == TargetType.Enemy && !isPlayer) {
			return true;
		}
		return false;
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

[Serializable]
public class EffectList {
	public List<Effect> list;
}
