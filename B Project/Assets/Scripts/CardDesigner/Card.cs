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

	public enum DeckClass {
		Neutral,
		Guardian,
		Necromancer,
		Thief,
		Warrior
	}

	public enum Category {
		Skill,
		Spell,
		Tactic
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
		Resource,
		DamageMultiplier
	}

	// Variables used by CardData
	public string title;
	public Card.CharacterType characterType;
	public Card.DeckClass deckType;
	public Card.Category category;
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
	public Image categoryImage;
	public Text categoryText;
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

		if(ownerText) {
			if(owner) {
				ownerText.text = ParseText(owner.UnitName);
			}
		}

		if(descriptionImage) {
			//artworkImage.sprite = artwork;
		}

		if(descriptionText) {
			descriptionText.text = description;
		}

		if(categoryImage) {
			categoryImage.color = backgroundImage.color;
		}

		if(categoryText) {
			categoryText.text = category.ToString();
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

                foreach (Effect effect in effects)
                {
                	bool applied = false;

                	if(ApplySelfEffect(effect)) {
                		applied = true;
                	}

                	// Check to see if effect was already applied
                	if(!applied) {
	                	if(owner.GetActualHP() > 0) {
							// Owner of this card is alive
							ApplyToTargets(targets, effect);
	                	} else {
							// Owner of this card is dead
							goto endEffectLoop;
	                	}
	                }
                }

                // Resume
                endEffectLoop:

                if (transform.parent) // Added check in case the card belong to AI, so no GO
                {// Player card
                    Hand hand = transform.parent.GetComponent<Hand>();
                    if (hand != null)
                    {
                        hand.Remove(this);
                    }
				} 
				else 
				{	//Remove enemy card upon playing
                	Destroy(gameObject);
                }
				CheckOnPlayRemovalConditions(owner);
            }
            else
            {
                Debug.Log("no targets, canceling drag.");
                GetComponent<CardDragHandler>().CancelDrag();
            }
        }
        else
        {
			//Play card since it doesn't need a target.
            Debug.Log("Playing card, no target required.");

			foreach (Effect effect in effects)
            {
            	bool applied = false;

            	if(ApplySelfEffect(effect)) {
            		applied = true;
            	}
            }

			if (transform.parent) // Added check in case the card belong to AI, so no GO
            {// Player card
                Hand hand = transform.parent.GetComponent<Hand>();
                if (hand != null)
                {
                    hand.Remove(this);
                }
			} 
			else 
			{	//Remove enemy card upon playing
            	Destroy(gameObject);
            }
			CheckOnPlayRemovalConditions(owner);
        }
    }

	bool ApplySelfEffect(Effect effect) {
		switch(effect.effectType) {
			case EffectType.DamageMultiplier:
				owner.GrantDamageMultiplier((float)effect.effectValue, effect.duration, owner, effect.condition);
				return true;
			break;

			case EffectType.Block:
				owner.GrantBlock(effect.effectValue, effect.duration, owner);
				return true;
			break;
		}

		return false;
    }

	void ApplyToTargets(BaseUnit[] targets, Effect effect) {
		Debug.Log("Applying effects");

		// Apply effects to specifically targeted units (dragged card onto or put AoE targeter on).
		switch(effect.effectType) {
			case EffectType.Damage:
				foreach(BaseUnit target in targets) {
				if(ParseTargetType(effect.targetType, target.IsPlayer())) {
						float mult = 1.00f;
						foreach(BaseStatus status in owner.Statuses) {
							if(status.GetType() == typeof(DamageMultiplierStatus)) {
								mult += status.Multiplier;
							}
						}

						int damage = (int)Math.Round(effect.effectValue * mult);
						target.DealDamage(damage);

                        // Add threat to player units
                        if (owner.IsPlayer())
                        {
                            PartyManager.Instance.ChangeThreat(owner, damage * 0.01f);
                        }
					}
				}
			break;

			case EffectType.Block:
				foreach(BaseUnit target in targets) {
					if(ParseTargetType(effect.targetType, target.IsPlayer())) {
						target.GrantBlock(effect.effectValue, effect.duration, owner);
					}
				}
			break;
		}
	}

	public void CheckOnPlayRemovalConditions(BaseUnit unit) {
		if(unit.Statuses == null) {return;}
		int numStatuses = unit.Statuses.Count-1;
		for(int i = numStatuses; i >= 0; i--) {
			switch(unit.Statuses[i].Condition) {

				case Effect.RemovalCondition.PlaySkillCard:
					if(category == Category.Skill) {
						Debug.Log("Removing Status");
						owner.Statuses.RemoveAt(i);
					}
				break;
			}
		}
	}

	public void LoadCardData() {
		if(cardData != null) {
			title = cardData.Title;
			characterType = cardData.CharacterType;
			deckType = cardData.DeckType;
			category = cardData.Category;
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
			cardData.Category = category;
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

	bool ParseTargetType(TargetType type, bool isPlayer = false) {

		if(type == TargetType.Self) {
			return true;
		} else if(cardData.Owner.IsPlayer() && isPlayer){
			// Targeting own team
			if(type == TargetType.Ally) {
				return true;
			}
		} else {
			// Targeting enemy team
			if(type == TargetType.Enemy) {
				return true;
			}
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
