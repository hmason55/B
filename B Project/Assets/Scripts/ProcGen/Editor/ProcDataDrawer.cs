using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ProcData))]
public class ProcDataDrawer : PropertyDrawer {

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {

        EditorGUI.BeginProperty(pos, label, prop);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate rects
        var typeRect = new Rect(pos.x, pos.y+20, pos.width, pos.height - 83);
        var objRect = new Rect(pos.x + 25, pos.y + 40, pos.width / 2, pos.height-80);
        var posRect = new Rect(pos.x + 10, pos.y + 70, pos.width - 90, pos.height+40);

        EditorGUI.PropertyField(typeRect, prop.FindPropertyRelative("type"), GUIContent.none);
        EditorGUI.PropertyField(objRect, prop.FindPropertyRelative("obj"), GUIContent.none);
        EditorGUI.PropertyField(posRect, prop.FindPropertyRelative("pos"), GUIContent.none);

        //EditorGUI.
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {

        return 100;
    }
}