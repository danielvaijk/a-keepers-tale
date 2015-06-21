using UnityEngine;
using UnityEditor;

using System.Collections;

[CustomEditor(typeof(CraftingCombinations))]
public class CraftingCombinationsEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemCombinations"), true);

        serializedObject.ApplyModifiedProperties();
    }
}