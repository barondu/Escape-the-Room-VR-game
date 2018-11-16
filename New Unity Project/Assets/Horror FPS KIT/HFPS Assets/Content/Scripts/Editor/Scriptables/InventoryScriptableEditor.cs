using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryScriptable)), CanEditMultipleObjects]
public class InventoryScriptableEditor : Editor {

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty list = serializedObject.FindProperty("ItemDatabase");

        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);

            if (GUILayout.Button(new GUIContent("-", "Delete"), EditorStyles.miniButton, GUILayout.Width(20)))
            {
                InventoryScriptable database = target as InventoryScriptable;
                database.RemoveAtReseed(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button(new GUIContent("+", "Add"), EditorStyles.miniButton, GUILayout.Height(20)))
        {
            InventoryScriptable database = target as InventoryScriptable;
            database.Add(new InventoryScriptable.ItemMapper { Title = "New Item" });
        }

        serializedObject.ApplyModifiedProperties();
    }
}