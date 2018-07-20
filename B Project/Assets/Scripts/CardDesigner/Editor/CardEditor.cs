using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

[CustomEditor(typeof(Card))]
[CanEditMultipleObjects]
public class CardEditor : Editor {

	SerializedProperty pTitle;
	SerializedProperty pCharacterType;
	SerializedProperty pDeckType;
	SerializedProperty pResourceCost;
	SerializedProperty pDescription;
	SerializedProperty pOmitFromDeck;
	SerializedProperty pRequireTarget;
	SerializedProperty pAreaOfEffect;
	SerializedProperty pTargetArea;
	SerializedProperty pTargetType;

	SerializedProperty pBackgroundImageObj;
	SerializedProperty pArtworkImageObj;
	SerializedProperty pTitleTextObj;
	SerializedProperty pOwnerTextObj;
	SerializedProperty pDescriptionImageObj;
	SerializedProperty pDescriptionTextObj;
	SerializedProperty pCostImageObj;
	SerializedProperty pCostTextObj;

	SerializedProperty pCardDataFile;

	bool toggleGeneral = true;
	bool toggleTargeting = false;
	bool toggleEffects = false;
	bool toggleDataManagement = false;
	bool toggleReferences = false;

	void OnEnable() {
        // Setup the SerializedProperties.
        pTitle = serializedObject.FindProperty("title");
		pCharacterType = serializedObject.FindProperty("characterType");
		pDeckType = serializedObject.FindProperty("deckType");
        pResourceCost = serializedObject.FindProperty("resourceCost");
		pDescription = serializedObject.FindProperty("description");
		pOmitFromDeck = serializedObject.FindProperty("omitFromDeck");
		pRequireTarget = serializedObject.FindProperty("requireTarget");
		pAreaOfEffect = serializedObject.FindProperty("areaOfEffect");
		pTargetType = serializedObject.FindProperty("targetType");
    }

	public override void OnInspectorGUI() {
		Card card = target as Card;

        serializedObject.Update();

        // General Settings
		toggleGeneral = EditorGUILayout.Foldout(toggleGeneral, "General");
        if(toggleGeneral) {
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField(pTitle, new GUIContent("Title"), null);
			EditorGUILayout.PropertyField(pCharacterType, new GUIContent("Character"), null);
			EditorGUILayout.PropertyField(pDeckType, new GUIContent("Deck"), null);
			EditorGUILayout.IntSlider(pResourceCost, -1, 8, new GUIContent("Cost"));
			EditorGUILayout.PropertyField(pOmitFromDeck, new GUIContent("Omit From Deck"), null);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Description", GUILayout.MaxWidth(EditorGUIUtility.labelWidth-EditorGUI.indentLevel*24f));
			pDescription.stringValue = EditorGUILayout.TextArea(pDescription.stringValue, GUILayout.Height(128));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUI.indentLevel = 0;
		}

		// Targeting Settings
		toggleTargeting = EditorGUILayout.Foldout(toggleTargeting, "Targeting");
		if(toggleTargeting) {

			// Targeting
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField(pRequireTarget, new GUIContent("Require Target"), null);

			if(pRequireTarget.boolValue) {
				EditorGUI.indentLevel = 2;
				EditorGUILayout.PropertyField(pTargetType, new GUIContent("Target Type"), null);
				EditorGUILayout.Space();
			}

			// Area of Effect
			EditorGUI.indentLevel = 1;
			EditorGUILayout.PropertyField(pAreaOfEffect, new GUIContent("Area of Effect"), null);

			if(pAreaOfEffect.boolValue) {
				EditorGUI.indentLevel = 2;
				EditorGUILayout.Space();

				for(int y = 0; y < 3; y++) {
					EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(256));
					for(int x = 0; x < 3; x++) {
						//card.targetArea[y*3 + x] = EditorGUILayout.Toggle(card.targetArea[y*3 + x], GUILayout.MaxWidth(14));
						//EditorGUILayout.Toggle(area[y*3 + x], GUILayout.MaxWidth(16));
						//Debug.Log(
						card.targetArea[y*3 + x] = EditorGUILayout.Toggle(card.targetArea[y*3 + x], new GUILayoutOption[] {GUILayout.Width(48), GUILayout.Height(48)});
					}

					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.Space();
			}
			EditorGUI.indentLevel = 0;
			EditorGUILayout.Space();
		}

		// Effects List
		toggleEffects = EditorGUILayout.Foldout(toggleEffects, "Effects");
		if(toggleEffects) {
			EditorGUI.indentLevel = 1;

			// Display list of effects
			if(card.effects != null) {
				for(int i = 0; i < card.effects.Count; i++) {
					EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(256));
					card.effectFoldouts[i] = EditorGUILayout.Foldout(card.effectFoldouts[i], card.effects[i].effectType.ToString());

					// Delete button
					if(GUILayout.Button("Delete")) {
						card.effects.RemoveAt(i);
						card.effectFoldouts.RemoveAt(i);
						EditorGUILayout.EndHorizontal();
					} else {
						EditorGUILayout.EndHorizontal();
						// List effect
						if(card.effectFoldouts[i]) {
							EditorGUI.indentLevel = 2;
							EditorGUILayout.Space();

							Effect effect = card.effects[i];

							// Effect property
							EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(256));
							EditorGUILayout.LabelField("Effect", GUILayout.MaxWidth(88));
							effect.effectType = (Card.EffectType)EditorGUILayout.EnumPopup(effect.effectType, null);
							EditorGUILayout.EndHorizontal();

							// Target property
							EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(256));
							EditorGUILayout.LabelField("Target", GUILayout.MaxWidth(88));
							effect.targetType = (Card.TargetType)EditorGUILayout.EnumPopup(effect.targetType, null);
							EditorGUILayout.EndHorizontal();

							// Value property
							EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(512));
							EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(88));
							effect.effectValue = EditorGUILayout.IntSlider(effect.effectValue, -100, 100, null);
							EditorGUILayout.EndHorizontal();

							// Duration property
							EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(512));
							EditorGUILayout.LabelField("Duration", GUILayout.MaxWidth(88));
							effect.duration = EditorGUILayout.IntSlider(effect.duration, -1, 20, null);
							EditorGUILayout.EndHorizontal();

							// Removal Condition property
							EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(256));
							EditorGUILayout.LabelField("Removal Condition", GUILayout.MaxWidth(88));
							effect.condition = (Effect.RemovalCondition)EditorGUILayout.EnumPopup(effect.condition, null);
							EditorGUILayout.EndHorizontal();

							// Save changes
							card.effects[i] = effect;
							EditorGUILayout.Space();
						}

						EditorGUILayout.Space();
					}
				}
			}

			// New effect button
			if(GUILayout.Button("New Effect")) {
				if(card.effects == null) {
					card.effects = new List<Effect>();
				}
				card.effects.Add(new Effect());
				card.effectFoldouts.Add(true);
			}

			EditorGUI.indentLevel = 0;
			EditorGUILayout.Space();
		}

		// Data Management Settings
		toggleDataManagement = EditorGUILayout.Foldout(toggleDataManagement, "Data Management");
		if(toggleDataManagement) {

			EditorGUI.indentLevel = 1;
			card.cardDataFile = EditorGUILayout.ObjectField(new GUIContent("Data File"), card.cardDataFile, typeof(TextAsset), true, null) as TextAsset;
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Save Concept")) {
				string prefabName = card.cardData.Title;
				if(prefabName != null) {
					string conceptPath = "Assets/Resources/Prefabs/Cards/Concepts/" + prefabName + ".prefab";
					if(AssetDatabase.LoadAssetAtPath(conceptPath, typeof(GameObject))) {
						if(EditorUtility.DisplayDialog("Hey! Listen!", "A card concept named \'" + prefabName + "\' already exists, do you want to overwrite it?", "Yes", "No")) {
							CreateConcept(card.gameObject, conceptPath);
							Debug.Log("Saved \'" + prefabName + "\' as a concept.");
						}
					} else {
						CreateConcept(card.gameObject, conceptPath);
						Debug.Log("Saved \'" + prefabName + "\' as a concept.");
					}
				}
			}

			if(GUILayout.Button("Save Data")) {
				string fileName = card.cardData.Title;
				if(fileName != null) {
					string filePath = "Assets/Resources/Prefabs/Cards/Data/" + fileName + ".json";
					if(AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject))) {
						if(EditorUtility.DisplayDialog("Hey! Listen!", "Card data for \'" + fileName + "\' already exists, do you want to overwrite it?", "Yes", "No")) {
							CreateJson(card.cardData, filePath);
							Debug.Log("Saved \'" + fileName + "\' card data.");
						}
					} else {
						CreateJson(card.cardData, filePath);
						Debug.Log("Saved \'" + fileName + "\' card data.");
					}
				}
			}

			if(card.cardDataFile != null) {
				if(GUILayout.Button("Load Data")) {
					LoadJson(card, card.cardDataFile.text);
					Debug.Log("Loaded \'" + card.cardDataFile.name + "\' card data.");
				}
			}
			EditorGUILayout.EndHorizontal();

			if(GUILayout.Button("Format All Data Files")) {
				FormatDataFiles(card);
			}

			EditorGUI.indentLevel = 0;
			EditorGUILayout.Space();
		}

		// Object References
 		toggleReferences = EditorGUILayout.Foldout(toggleReferences, "Object References");
		if(toggleReferences) {
			card.backgroundImage = EditorGUILayout.ObjectField(new GUIContent("Background Image Asset"), card.backgroundImage, typeof(Image), true, null) as Image;
			card.artworkImage = EditorGUILayout.ObjectField(new GUIContent("Artwork Image Asset"), card.artworkImage, typeof(Image), true, null) as Image;
			card.titleText = EditorGUILayout.ObjectField(new GUIContent("Title Text Asset"), card.titleText, typeof(Text), true, null) as Text;
			card.ownerText = EditorGUILayout.ObjectField(new GUIContent("Owner Text Asset"), card.ownerText, typeof(Text), true, null) as Text;
			card.descriptionImage = EditorGUILayout.ObjectField(new GUIContent("Description Image Asset"), card.descriptionImage, typeof(Image), true, null) as Image;
			card.descriptionText = EditorGUILayout.ObjectField(new GUIContent("Description Text Asset"), card.descriptionText, typeof(Text), true, null) as Text;
			card.costImage = EditorGUILayout.ObjectField(new GUIContent("Cost Image Asset"), card.costImage, typeof(Image), true, null) as Image;
			card.costText = EditorGUILayout.ObjectField(new GUIContent("Cost Text Asset"), card.costText, typeof(Text), true, null) as Text;
        }

        serializedObject.ApplyModifiedProperties();
    }

    static void CreateConcept(GameObject obj, string path) {
    	Object prefab = PrefabUtility.CreatePrefab(path, obj);
    	PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }

	static void CreateJson(CardData data, string path) {
    	StreamWriter writer = new StreamWriter(path, false);
		writer.WriteLine(JsonUtility.ToJson(data));
    	writer.Close();
    }

    void FormatDataFiles(Card card) {
		string dataPath = "Prefabs/Cards/Data/";
		UnityEngine.Object[] textAssets = Resources.LoadAll(dataPath, typeof(TextAsset));
		if(EditorUtility.DisplayDialog("Hey! Listen!", "You are about to format " + textAssets.Length + " data files, are you sure?", "Yes", "No")) {
			foreach(TextAsset textAsset in textAssets) {
				card.cardData = JsonUtility.FromJson<CardData>(textAsset.text);
				card.LoadCardData();
				card.SaveCardData();
				StreamWriter writer = new StreamWriter("Assets/Resources/Prefabs/Cards/Data/" + card.cardData.Title + ".json", false);
				writer.WriteLine(JsonUtility.ToJson(card.cardData));
		    	writer.Close();
			}
			Debug.Log(textAssets.Length + " data files were formatted.");
		}
    }

	void LoadJson(Card card, string json) {
		card.cardData = JsonUtility.FromJson<CardData>(json);
		card.LoadCardData();
		EditorUtility.SetDirty(target);
    }

}
