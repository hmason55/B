using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Card : MonoBehaviour {

	public enum DeckClass {
		Neutral,
		Guardian,
		Necromancer,
		Thief,
		Warrior
	}

	[SerializeField] string title;
	[SerializeField] DeckClass deckType;
	[SerializeField] int resourceCost;
	[SerializeField] string description;

	public bool showObjectReferences;

	public Image backgroundImage;
	public Image artworkImage;
	public Text titleText;
	public Image descriptionImage;
	public Text descriptionText;
	public Image costImage;
	public Text costText;


	void Update() {

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
			titleText.text = title;
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

}
