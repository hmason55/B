using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(Card))]
[CanEditMultipleObjects]
public class CardEditor : Editor {

	SerializedProperty pTitle;
	SerializedProperty pDeckType;
	SerializedProperty pResourceCost;
	SerializedProperty pDescription;

	SerializedProperty pBackgroundImageObj;
	SerializedProperty pArtworkImageObj;
	SerializedProperty pTitleTextObj;
	SerializedProperty pDescriptionImageObj;
	SerializedProperty pDescriptionTextObj;
	SerializedProperty pCostImageObj;
	SerializedProperty pCostTextObj;

	void OnEnable() {
        // Setup the SerializedProperties.

        pTitle = serializedObject.FindProperty("title");
		pDeckType = serializedObject.FindProperty("deckType");
        pResourceCost = serializedObject.FindProperty("resourceCost");
		pDescription = serializedObject.FindProperty("description");
        // Reference Objects
        /*
		pBackgroundImageObj = serializedObject.FindProperty("backgroundImage");
		pArtworkImageObj = serializedObject.FindProperty("artworkImage");
		pTitleTextObj = serializedObject.FindProperty("titleText");
		pDescriptionImageObj = serializedObject.FindProperty("descriptionImage");
		pDescriptionTextObj = serializedObject.FindProperty("descriptionText");
		pCostImageObj = serializedObject.FindProperty("costImage");
		pCostTextObj = serializedObject.FindProperty("costText");*/
    }

	public override void OnInspectorGUI() {
		Card card = target as Card;

        serializedObject.Update();

		EditorGUILayout.PropertyField(pTitle, new GUIContent("Card Title"), null);
		EditorGUILayout.PropertyField(pDeckType, new GUIContent("Deck Type"), null);
		EditorGUILayout.IntSlider(pResourceCost, -1, 8, new GUIContent("Resource Cost"));
		EditorGUILayout.PropertyField(pDescription, new GUIContent("Description"), null);

		EditorGUILayout.Space();

		if(GUILayout.Button("Save As Concept")) {
			string prefabName = card.titleText.text;
			Debug.Log(prefabName);
			if(prefabName != null) {
				string conceptPath = "Assets/Prefabs/Cards/Concepts/" + prefabName + ".prefab";
				if(AssetDatabase.LoadAssetAtPath(conceptPath, typeof(GameObject))) {
					if(EditorUtility.DisplayDialog("Hey! Listen!", "A card concept named \'" + prefabName + "\' already exists, do you want to overwrite it?", "Yes", "No")) {
						CreateConcept(card.gameObject, conceptPath);
					}
				} else {
					CreateConcept(card.gameObject, conceptPath);
				}
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

        serializedObject.ApplyModifiedProperties();
    }

    // Custom GUILayout progress bar.
    void ProgressBar (float value, string label) {
        // Get a rect for the progress bar using the same margins as a textfield:
        //Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
        //EditorGUI.ProgressBar (rect, value, label);
        //EditorGUILayout.Space ();
    }

    static void CreateConcept(GameObject obj, string path) {
    	Object prefab = PrefabUtility.CreatePrefab(path, obj);
    	PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }
}
