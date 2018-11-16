using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphicScriptable)), CanEditMultipleObjects]
public class GraphicScriptableEditor : Editor {

    SerializedProperty list;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorList.Show(list = serializedObject.FindProperty("qualityLevels"), EditorListOption.Buttons | EditorListOption.ElementLabels);
        if (GUILayout.Button(new GUIContent("New Graphic Quality", "Add"), EditorStyles.miniButton, GUILayout.Height(25)))
        {
            list.arraySize += 1;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
