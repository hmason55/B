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
	SerializedProperty pDeckType;
	SerializedProperty pResourceCost;
	SerializedProperty pDescription;
	SerializedProperty pOmitFromDeck;

	SerializedProperty pBackgroundImageObj;
	SerializedProperty pArtworkImageObj;
	SerializedProperty pTitleTextObj;
	SerializedProperty pDescriptionImageObj;
	SerializedProperty pDescriptionTextObj;
	SerializedProperty pCostImageObj;
	SerializedProperty pCostTextObj;

	SerializedProperty pCardDataFile;

	void OnEnable() {
        // Setup the SerializedProperties.

        pTitle = serializedObject.FindProperty("title");
		pDeckType = serializedObject.FindProperty("deckType");
        pResourceCost = serializedObject.FindProperty("resourceCost");
		pDescription = serializedObject.FindProperty("description");
		pOmitFromDeck = serializedObject.FindProperty("omitFromDeck");
    }

	public override void OnInspectorGUI() {
		Card card = target as Card;

        serializedObject.Update();

		EditorGUILayout.PropertyField(pTitle, new GUIContent("Card Title"), null);
		EditorGUILayout.PropertyField(pDeckType, new GUIContent("Deck Type"), null);
		EditorGUILayout.IntSlider(pResourceCost, -1, 8, new GUIContent("Resource Cost"));
		EditorGUILayout.PropertyField(pDescription, new GUIContent("Description"), null);
		EditorGUILayout.PropertyField(pOmitFromDeck, new GUIContent("Omit From Deck"), null);

		EditorGUILayout.Space();

		if(GUILayout.Button("Save As Concept")) {
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

		if(GUILayout.Button("Save Card Data File")) {
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

		/*
		if(GUILayout.Button("Spawn Card")) {
			string prefabName = card.titleText.text;
			Debug.Log(prefabName);
			if(prefabName != null) {
				string conceptPath = "Assets/Resources/Prefabs/Cards/Concepts/" + prefabName + ".prefab";
				if(AssetDatabase.LoadAssetAtPath(conceptPath, typeof(GameObject))) {
					if(EditorUtility.DisplayDialog("Hey! Listen!", "A card concept named \'" + prefabName + "\' already exists, do you want to overwrite it?", "Yes", "No")) {
						CreateConcept(card.gameObject, conceptPath);
					}
				} else {
					CreateConcept(card.gameObject, conceptPath);
				}
			}
		}*/



		card.cardDataFile = EditorGUILayout.ObjectField(new GUIContent("Card Data File"), card.cardDataFile, typeof(TextAsset), true, null) as TextAsset;
		if(card.cardDataFile != null) {
			if(GUILayout.Button("Load Card")) {
				LoadJson(card, card.cardDataFile.text);
				Debug.Log("Loaded \'" + card.cardDataFile.name + "\' card data.");
			}
		}		

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		card.showObjectReferences = EditorGUILayout.Toggle("Show Object References", card.showObjectReferences);

		// Show or hide object references
		if(card.showObjectReferences) {
			card.backgroundImage = EditorGUILayout.ObjectField(new GUIContent("Background Image Asset"), card.backgroundImage, typeof(Image), true, null) as Image;
			card.artworkImage = EditorGUILayout.ObjectField(new GUIContent("Artwork Image Asset"), card.artworkImage, typeof(Image), true, null) as Image;
			card.titleText = EditorGUILayout.ObjectField(new GUIContent("Title Text Asset"), card.titleText, typeof(Text), true, null) as Text;
			card.descriptionImage = EditorGUILayout.ObjectField(new GUIContent("Description Image Asset"), card.descriptionImage, typeof(Image), true, null) as Image;
			card.descriptionText = EditorGUILayout.ObjectField(new GUIContent("Description Text Asset"), card.descriptionText, typeof(Text), true, null) as Text;
			card.costImage = EditorGUILayout.ObjectField(new GUIContent("Cost Image Asset"), card.costImage, typeof(Image), true, null) as Image;
			card.costText = EditorGUILayout.ObjectField(new GUIContent("Cost Text Asset"), card.costText, typeof(Text), true, null) as Text;
        }

		if(GUILayout.Button("Format All Data Files")) {
			FormatDataFiles(card);
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
