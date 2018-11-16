using UnityEditor;
using UnityEngine;
using System;

[Flags]
public enum EditorListOption {
	None,
	ListSize,
	ListLabel,
	ElementLabels = 4,
	Buttons = 8,
	Default = ListSize | ListLabel | ElementLabels,
	NoElementLabels = ListSize | ListLabel,
	All = Default | Buttons
}

public static class EditorList {

    private static GUIContent
		deleteButtonContent = new GUIContent("-", "Delete"),
		addButtonContent = new GUIContent("+", "Add");

	public static void Show (SerializedProperty list, EditorListOption options = EditorListOption.Default) {
		if (!list.isArray) {
			EditorGUILayout.HelpBox(list.name + " is neither an array or a list!", MessageType.Error);
			return;
		}

        bool showListLabel = (options & EditorListOption.ListLabel) != 0;
		bool showListSize = (options & EditorListOption.ListSize) != 0;

		if (showListLabel) {
			EditorGUILayout.PropertyField(list);
			EditorGUI.indentLevel += 1;
		}
		if (!showListLabel || list.isExpanded) {
			SerializedProperty size = list.FindPropertyRelative("Array.size");
			if (showListSize) {
				EditorGUILayout.PropertyField(size);
			}
			if (size.hasMultipleDifferentValues) {
				EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
			}
			else {
				ShowElements(list, options);
			}
		}
		if (showListLabel) {
			EditorGUI.indentLevel -= 1;
		}
	}

	private static void ShowElements (SerializedProperty list, EditorListOption options) {
        bool showElementLabels = (options & EditorListOption.ElementLabels) != 0;
        bool showButtons = (options & EditorListOption.Buttons) != 0;

        for (int i = 0; i < list.arraySize; i++) {
			if (showButtons) {
				EditorGUILayout.BeginHorizontal();
			}
			if (showElementLabels) {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);
            }
			else {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
			}
            if (showButtons && GUILayout.Button(deleteButtonContent, EditorStyles.miniButton, GUILayout.Width(20)))
            {
                list.DeleteArrayElementAtIndex(i);
            }
            if (showButtons) {
				EditorGUILayout.EndHorizontal();
			}
        }
    }
}