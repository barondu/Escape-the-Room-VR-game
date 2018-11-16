using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExamineItem)), CanEditMultipleObjects]
public class ExamineEditor : Editor {

    public SerializedProperty prop_type;
    public SerializedProperty prop_examineDistance;
    public SerializedProperty prop_examineSound;
    public SerializedProperty prop_isUsable;
    public SerializedProperty prop_examineObjectName;
    public SerializedProperty prop_paperReadText;
    public SerializedProperty prop_textSize;
    public SerializedProperty prop_paperRotation;

    void OnEnable()
    {
        prop_type = serializedObject.FindProperty("examineType");
        prop_examineDistance = serializedObject.FindProperty("examineDistance");
        prop_examineSound = serializedObject.FindProperty("examineSound");
        prop_isUsable = serializedObject.FindProperty("isUsable");
        prop_examineObjectName = serializedObject.FindProperty("examineObjectName");
        prop_paperReadText = serializedObject.FindProperty("paperReadText");
        prop_textSize = serializedObject.FindProperty("textSize");
        prop_paperRotation = serializedObject.FindProperty("paperRotation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ExamineItem.type type = (ExamineItem.type)prop_type.enumValueIndex;
        EditorGUILayout.PropertyField(prop_type);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(prop_examineDistance, new GUIContent("Examine Distance"));
        EditorGUILayout.PropertyField(prop_examineSound, new GUIContent("Examine Sound"));
        EditorGUILayout.Space();

        if (type == ExamineItem.type.Object)
        {
            EditorGUILayout.PropertyField(prop_isUsable, new GUIContent("Is Usable Object"));
            EditorGUILayout.PropertyField(prop_examineObjectName, new GUIContent("Examine Object Name"));
        }

        if (type == ExamineItem.type.Paper)
        {
            EditorGUILayout.PropertyField(prop_paperReadText, new GUIContent("Read Text"));
            EditorGUILayout.PropertyField(prop_textSize, new GUIContent("Text Size"));
            EditorGUILayout.PropertyField(prop_paperRotation, new GUIContent("Paper Rotation"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
