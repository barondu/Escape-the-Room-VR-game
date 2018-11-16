using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(InputScriptable)), CanEditMultipleObjects]
public class InputScriptableEditor : Editor {

    SerializedProperty list;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);

            if (GUILayout.Button(new GUIContent("-", "Delete"), EditorStyles.miniButton, GUILayout.Width(20)))
            {
                list.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button(new GUIContent("Add Input", "Add"), EditorStyles.miniButton, GUILayout.Height(20)))
        {
            InputScriptable map = target as InputScriptable;
            map.inputMap.Add(new InputScriptable.InputMaper { InputName = "New Input" });
        }
    }
}
