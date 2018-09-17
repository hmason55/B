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
        Tile
	}

	public enum EffectType {
		Damage,
		Block,
		Heal,
		Stun,
		Push,
		Resource,
		DamageMultiplier,
        Miasma,
        Taunt2_single,
        Taunt,
        Link
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

	// ResourceManager cache
	ResourceManager _resourceManager;

	void Awake() {
		if(cardData == null) {
			cardData = new CardData();
		}

		_resourceManager = GameObject.FindObjectOfType<ResourceManager>();
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


    public void Play(int tile)
    {
        // Version with a tile target
        // TODO probably refactor both methods into a more compact way


        if (cardData.RequireTarget)
        {
            // Check if valid tile
            if (tile < 0)
            {
                Debug.Log("no targets, canceling drag.");
                GetComponent<CardDragHandler>().CancelDrag();
            }
            else if(_resourceManager.SpendResources(resourceCost, owner))
            {
                Debug.Log("Playing: " + title + " to tile: " + tile);

                foreach (Effect effect in effects)
                {
                    bool applied = false;

                    if (ApplySelfEffect(effect))
                    {
                        applied = true;
                    }

                    // Check to see if effect was already applied
                    if (!applied)
                    {
                        if (owner.GetActualHP() > 0)
                        {
                            // Owner of this card is alive 
            
                            ApplyToTile(tile, effect);
                        }
                        else
                        {
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
                {   //Remove enemy card upon playing
                    Destroy(gameObject);
                }
                CheckOnPlayRemovalConditions(owner);
            }
        }
    }

    public void Play(BaseUnit[] targets)
    {
        if (cardData.RequireTarget)
        {
			if (targets.Length > 0 && (_resourceManager.SpendResources(resourceCost, owner)))
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
		else if(_resourceManager.SpendResources(cardData.ResourceCost, owner))
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
            case EffectType.Taunt:
                PartyManager.Instance.ChangeThreat(owner, effect.effectValue*0.01f);
                return true;
            case EffectType.Link:
                LinkStatus link = new LinkStatus(effect.effectValue, effect.duration, owner);
                owner.AddStatus(link);
                return true;
		}

		return false;
    }

    void ApplyToTile(int tile, Effect effect)
    {
        // Get list of valid tiles
        List<int> targetTiles;
        if (areaOfEffect)
            targetTiles = Battleground.Instance.GetTilesFromShape(targetArea, tile);
        else
            targetTiles = new List<int> { tile };

        Debug.Log("tiles number: " + targetTiles.Count);
        switch (effect.effectType)
        {
            case EffectType.Miasma:


                // Spawn miasma on tiles
                foreach (int t in targetTiles)
                {
                    MiasmaFieldEffect miasma = new MiasmaFieldEffect(effect.effectValue, effect.duration, owner, t);
                    Battleground.Instance.AddFieldEffect(t, miasma);
                }
                break;

            case EffectType.Damage:
                float mult = 1.00f;
                for (int i = owner.Statuses.Count - 1; i >= 0; i--)
                {
                    BaseStatus status = owner.Statuses[i];
                    if (status.GetType() == typeof(DamageMultiplierStatus))
                    {
                        mult += status.Multiplier;

                        // Remove multiplier status 
                        // TODO change if the status can have more than 1 use

                    }
                }
                foreach (int t in targetTiles)
                {
                    BaseUnit target = Battleground.Instance.GetUnitOnTile(t);
                    
                    if (target == null)
                        continue;
                    if (ParseTargetType( TargetType.Enemy, target.IsPlayer()))
                    {

                        int damage = (int)Math.Round(effect.effectValue * mult);
                        target.DealDamage(damage, owner);

                        owner.RemoveStatusOfType(typeof(DamageMultiplierStatus));
                        owner.UpdateUI();

                        // Add threat to player units
                        if (owner.IsPlayer())
                        {
                            PartyManager.Instance.ChangeThreat(owner, damage * 0.01f);
                        }
                    }
                }
                break;
        }
    }


    void ApplyToTargets(BaseUnit[] targets, Effect effect)
    {
        Debug.Log("Applying effects");

        // Apply effects to specifically targeted units (dragged card onto or put AoE targeter on).
        switch (effect.effectType)
        {
            case EffectType.Damage:
                foreach (BaseUnit target in targets)
                {
                    if (ParseTargetType(effect.targetType, target.IsPlayer()))
                    {
                        float mult = 1.00f;
                        for (int i = owner.Statuses.Count - 1; i >= 0; i--)
                        {
                            BaseStatus status = owner.Statuses[i];
                            if (status.GetType() == typeof(DamageMultiplierStatus))
                            {
                                mult += status.Multiplier;

                                // Remove multiplier status 
                                // TODO change if the status can have more than 1 use
                                owner.RemoveStatusOfType(status.GetType());
                                owner.UpdateUI();
                            }
                        }

                        int damage = (int)Math.Round(effect.effectValue * mult);
                        target.DealDamage(damage,owner);

                        // Add threat to player units
                        if (owner.IsPlayer())
                        {
                            PartyManager.Instance.ChangeThreat(owner, damage * 0.01f);
                        }
                    }
                }
                break;

            case EffectType.Block:
                foreach (BaseUnit target in targets)
                {
                    if (ParseTargetType(effect.targetType, target.IsPlayer()))
                    {
                        target.GrantBlock(effect.effectValue, effect.duration, owner);
                    }
                }
                break;

            case EffectType.Taunt:
                foreach (BaseUnit target in targets)
                {
                    if (ParseTargetType(effect.targetType, target.IsPlayer()))
                    {
                        TauntStatus taunt = new TauntStatus(effect.duration, owner, target);
                        target.AddStatus(taunt);
                    }
                }
                break;
            case EffectType.Stun:
                foreach (BaseUnit target in targets)
                {
                    if (ParseTargetType(effect.targetType, target.IsPlayer()))
                    {
                        StunStatus stun = new StunStatus(effect.duration, owner, target);
                        target.AddStatus(stun);
                    }
                }
                break;
            case EffectType.Push:
                foreach (BaseUnit target in targets)
                {
                    if (ParseTargetType(effect.targetType, target.IsPlayer()))
                    {
                        // check if not on last col                        
                        int pos = target.GetGridPosition();
                        int col = pos % 3;
                        int side = pos / 9;
                        int row = (pos - side * 9) / 3;
                        List<int> tiles = new List<int>();
                        if (col != 0)
                            tiles.Add(pos - 1);
                        if (col != 2)
                            tiles.Add(pos + 1);
                        if (row != 0)
                            tiles.Add(pos - 3);
                        if (row != 2)
                            tiles.Add(pos + 3);
                        // check if tiles are empty
                        for (int i = tiles.Count-1; i >=0; i--)
                        {
                            if (Battleground.Instance.GetUnitOnTile(tiles[i]) != null)
                                tiles.RemoveAt(i);
                        }
                        if (tiles.Count < 1)
                            return;
                        int nextPos =tiles[ UnityEngine.Random.Range(0, tiles.Count)];
                        target.MoveToTile(nextPos);
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
