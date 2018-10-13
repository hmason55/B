using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

[CustomEditor(typeof(Battleground))]
public class BattlegroundEditor : Editor
{
    private string[] _handleNames = new string[6]
    { "Player top left", "Player top right", "Player bottom left", "Enemy top left", "Enemy top right", "Enemy bottom left" };

    // Show additional buttons
    private bool _editorVisible =false;


    public override void OnInspectorGUI()
    {
        Battleground battleground = target as Battleground;

        DrawDefaultInspector();
        if (_editorVisible)       
            GUI.backgroundColor = new Color(0.75f, 0.75f, 0.75f, 1f);
        else
            GUI.backgroundColor = Color.gray;

        if (GUILayout.Button( "Show Grid", GUILayout.Width(100), GUILayout.Height(30)))
        {
            _editorVisible = !_editorVisible;
        }

        if (_editorVisible)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Reset player grid", GUILayout.Width(150), GUILayout.Height(30)))
            {
                Undo.RecordObject(battleground, "Move Handle");
                Vector2[] positions = battleground.GetViewportCornerPositions();
                positions[0] = new Vector2(0.2f, 0.7f);
                positions[1] = new Vector2(0.4f, 0.7f);
                positions[2] = new Vector2(0.2f, 0.3f);
                battleground.SetViewportCornerPositions(positions);
            }
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Reset enemy grid", GUILayout.Width(150), GUILayout.Height(30)))
            {
                Undo.RecordObject(battleground, "Move Handle");
                Vector2[] positions = battleground.GetViewportCornerPositions();
                positions[3] = new Vector2(0.6f, 0.7f);
                positions[4] = new Vector2(0.8f, 0.7f);
                positions[5] = new Vector2(0.6f, 0.3f);
                battleground.SetViewportCornerPositions(positions);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }   
    }
    
    void OnSceneGUI()
    {
        if (!_editorVisible)
            return;

        Battleground battleground = target as Battleground;
              
        // Draw lines
        Handles.color = Color.cyan;
        // Points along the border
        Vector2[] lefts = new Vector2[4];
        Vector2[] rights = new Vector2[4];
        Vector2[] tops = new Vector2[4];
        Vector2[] bottoms = new Vector2[4];

        Vector2[] positions = battleground.GetWorldCornerPositions();

        for (int n = 0; n < 2; n++)
        {
            // External border vectors
            Vector2 right = positions[1 + n * 3] - positions[n * 3];
            Vector2 down = positions[2 + n * 3] - positions[n * 3];
            for (int i = 0; i < 4; i++)
            {
                lefts[i] = Vector2.Lerp(positions[ n * 3], positions[2 + n * 3], i /3.0f);
                rights[i] = lefts[i] + right;
                tops[i]= Vector2.Lerp(positions[ n * 3], positions[1 + n * 3], i /3.0f);
                bottoms[i] = tops[i] + down;

                Handles.DrawLine(lefts[i], rights[i]);
                Handles.DrawLine(tops[i], bottoms[i]);
            }
        }

        // Draw corner handles
        Vector2 newPos;
        Vector2 oldPos;
        for (int i = 0; i < 6; i++)
        {
            Handles.color = Color.red;
            oldPos = positions[i];
            newPos = Handles.FreeMoveHandle(oldPos, Quaternion.identity, 0.1f, Vector3.zero, Handles.DotHandleCap);
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.textColor = Color.white;
            Handles.Label(oldPos + new Vector2(-0.5f, 0.3f), _handleNames[i], guiStyle);
            if (newPos != oldPos)
            {
                Undo.RecordObject(battleground, "Move Handle");                
            }
            positions[i] = Camera.main.WorldToViewportPoint( newPos);
        }
        battleground.SetViewportCornerPositions(positions);
    }

}