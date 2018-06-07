using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProcAgent))]
[CanEditMultipleObjects]
public class AgentInspector : Editor {

    SerializedProperty restProbability;
    SerializedProperty merchantProbability;
    SerializedProperty settlementProbability;
    SerializedProperty treasureProbability;
    SerializedProperty nothingProbability;
    SerializedProperty tileProbability;
    SerializedProperty tileSize;
    SerializedProperty tilePaddingX;
    SerializedProperty tilepaddingY;
    SerializedProperty islandSize;
    SerializedProperty minAdjacency;
    SerializedProperty maxRests;
    SerializedProperty maxMerchants;
    SerializedProperty maxSettlements;
    SerializedProperty maxTreasure;
    SerializedProperty maxGlobalRests;
    SerializedProperty maxGlobalMerchants;
    SerializedProperty maxGlobalSettlements;
    SerializedProperty maxGlobalTreasure;
    SerializedProperty maxSteps;
    SerializedProperty center;
    SerializedProperty distribution;
    SerializedProperty fillHoles;
    SerializedProperty overWriteCells;
    SerializedProperty connectCells;
    SerializedProperty customCenter;
    SerializedProperty debug;
    SerializedProperty customProb;
    SerializedProperty tile;
    SerializedProperty parent;

    ProcAgent procGen;
    bool showMore = false;

    void OnEnable()
    {
        restProbability = serializedObject.FindProperty("restProbability");
        merchantProbability = serializedObject.FindProperty("merchantProbability");
        settlementProbability = serializedObject.FindProperty("settlementProbability");
        treasureProbability = serializedObject.FindProperty("treasureProbability");
        nothingProbability = serializedObject.FindProperty("nothingProbability");
        tileProbability = serializedObject.FindProperty("tileProbability");
        tileSize = serializedObject.FindProperty("tileSize");
        tilePaddingX = serializedObject.FindProperty("tilePaddingX");
        tilepaddingY = serializedObject.FindProperty("tilePaddingY");
        islandSize = serializedObject.FindProperty("islandSize");
        minAdjacency = serializedObject.FindProperty("minAdjacency");
        maxRests = serializedObject.FindProperty("maxRests");
        maxMerchants = serializedObject.FindProperty("maxMerchants");
        maxSettlements = serializedObject.FindProperty("maxSettlements");
        maxTreasure = serializedObject.FindProperty("maxTreasure");
        maxGlobalRests = serializedObject.FindProperty("maxGlobalRests");
        maxGlobalMerchants = serializedObject.FindProperty("maxGlobalMerchants");
        maxGlobalSettlements = serializedObject.FindProperty("maxGlobalSettlements");
        maxGlobalTreasure = serializedObject.FindProperty("maxGlobalTreasure");
        maxSteps = serializedObject.FindProperty("maxSteps");
        center = serializedObject.FindProperty("center");
        distribution = serializedObject.FindProperty("distribution");
        fillHoles = serializedObject.FindProperty("fillHoles");
        overWriteCells = serializedObject.FindProperty("overwriteCells");
        connectCells = serializedObject.FindProperty("connectCells");
        customCenter = serializedObject.FindProperty("customCenter");
        debug = serializedObject.FindProperty("debug");
        customProb = serializedObject.FindProperty("customProb");
        tile = serializedObject.FindProperty("tile");
        parent = serializedObject.FindProperty("parent");

        procGen = (ProcAgent)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;

        EditorGUILayout.LabelField("Map parameters", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        tileProbability.floatValue = EditorGUILayout.Slider(new GUIContent("Tile Probability"),
            tileProbability.floatValue, 0f, 1f);

        tileSize.floatValue = EditorGUILayout.FloatField(new GUIContent("Tile Size"), tileSize.floatValue);

        EditorGUILayout.PropertyField(islandSize, new GUIContent("Map Radius","Determines the radius of the map from its center"));
        EditorGUILayout.PropertyField(minAdjacency, new GUIContent("Minimal Adjacency", "Removes any cells from the map with less adjacent cells than the given number. The higher this value is, the smoother the map."));

        EditorGUILayout.PropertyField(connectCells, new GUIContent("Connect Cells", "Connects all disconnected subgraphs the the center tile. Making sure every place on the map is reachable."));
        EditorGUILayout.PropertyField(fillHoles, new GUIContent("Fill Holes", "Makes it impossible for small holes to appear in the map"));
        EditorGUILayout.PropertyField(customCenter, new GUIContent("Custom Center", "Assign a custom center to the map."));

        if (customCenter.boolValue)
        {
            center.vector2IntValue = EditorGUILayout.Vector2IntField(new GUIContent("Center", "Where should the center of the map be?"),
            center.vector2IntValue);
        }

        EditorGUILayout.PropertyField(distribution);
        if ((TileDistribution)distribution.enumValueIndex == TileDistribution.custom || (TileDistribution)distribution.enumValueIndex == TileDistribution.customCenter)
        {
            customProb.animationCurveValue = EditorGUILayout.CurveField(customProb.animationCurveValue);
        }

        EditorGUILayout.PropertyField(parent);
        EditorGUILayout.PropertyField(tile);

        Vector2 padding = EditorGUILayout.Vector2Field(new GUIContent("Padding"), new Vector2(tilePaddingX.floatValue, tilepaddingY.floatValue));
        tilePaddingX.floatValue = padding.x;
        tilepaddingY.floatValue = padding.y;
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Location parameters", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        restProbability.floatValue = EditorGUILayout.Slider(new GUIContent("Rest Probability", "The probability of a rest tile being placed in this map."),
            restProbability.floatValue, 0f, 1f);
        merchantProbability.floatValue = EditorGUILayout.Slider(new GUIContent("Merchant Probability", "The probability of a rest tile being placed in this map."),
            merchantProbability.floatValue, 0f, 1f);
        settlementProbability.floatValue = EditorGUILayout.Slider(new GUIContent("Settlement Probability", "The probability of a rest tile being placed in this map."),
            settlementProbability.floatValue, 0f, 1f);
        treasureProbability.floatValue = EditorGUILayout.Slider(new GUIContent("Treasure Probability", "The probability of a rest tile being placed in this map."),
            treasureProbability.floatValue, 0f, 1f);
        nothingProbability.floatValue = EditorGUILayout.Slider(new GUIContent("Empty Probability", "The probability of a rest tile being placed in this map."),
            nothingProbability.floatValue, 0f, 1f);

        EditorGUILayout.PropertyField(overWriteCells);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Information", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(new GUIContent("Number of Tiles: "), new GUIContent(procGen.tileCountString()));

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Map")) {
            if (!procGen.mapExists())
            {
                procGen.createMap();
            }
        }

        if (GUILayout.Button("Destroy")) {
            if (procGen.mapExists()) {
                procGen.destroyMapEdit();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate heatmap")) {
            //TODO
        }

        serializedObject.ApplyModifiedProperties();
    } 
}
