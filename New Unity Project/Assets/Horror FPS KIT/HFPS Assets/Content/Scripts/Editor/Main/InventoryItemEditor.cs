using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventoryItem)), CanEditMultipleObjects]
public class InventoryItemEditor : Editor {

    public SerializedProperty
    ItemType_Prop,
    WeaponType_Prop,
    MessageType_Prop,
    DisableType_Prop,
    Amount_Prop,
    WeaponID_Prop,
    InventoryID_Prop,
    BackpackExpand_Prop,
    MarkAsLight_Prop,
    PickupSound_Prop,
    PickupVolume_Prop,
    MessageText_prop;


    void OnEnable () {
		ItemType_Prop = serializedObject.FindProperty ("ItemType");
		WeaponType_Prop = serializedObject.FindProperty ("weaponType");
        MessageType_Prop = serializedObject.FindProperty("messageType");
        DisableType_Prop = serializedObject.FindProperty("disableType");
        Amount_Prop = serializedObject.FindProperty("Amount");
		WeaponID_Prop = serializedObject.FindProperty ("WeaponID");
		InventoryID_Prop = serializedObject.FindProperty ("InventoryID");
		BackpackExpand_Prop = serializedObject.FindProperty ("BackpackExpand");
        MarkAsLight_Prop = serializedObject.FindProperty("markLightObject");
        PickupSound_Prop = serializedObject.FindProperty("PickupSound");
        PickupVolume_Prop = serializedObject.FindProperty("PickupVolume");
        MessageText_prop = serializedObject.FindProperty("message");
    }

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
        InventoryItem.Type type = (InventoryItem.Type)ItemType_Prop.enumValueIndex;
        InventoryItem.MessageType msg = (InventoryItem.MessageType)MessageType_Prop.enumValueIndex;
        InventoryItem.DisableType dis = (InventoryItem.DisableType)DisableType_Prop.enumValueIndex;

        EditorGUILayout.PropertyField(ItemType_Prop);
        EditorGUILayout.PropertyField(MessageType_Prop);
        EditorGUILayout.PropertyField(DisableType_Prop);

        switch (msg)
        {
            case InventoryItem.MessageType.Hint:
                EditorGUILayout.PropertyField(MessageText_prop, new GUIContent("Hint Message:"));
                break;
            case InventoryItem.MessageType.Message:
                EditorGUILayout.PropertyField(MessageText_prop, new GUIContent("Notification Message:"));
                break;
            case InventoryItem.MessageType.ItemName:
                EditorGUILayout.PropertyField(MessageText_prop, new GUIContent("Notification Item:"));
                break;
        }

        EditorGUILayout.PropertyField(PickupSound_Prop, new GUIContent("Pickup Sound:"));
        EditorGUILayout.PropertyField(PickupVolume_Prop, new GUIContent("Pickup Volume:"));

        switch (type)
        {
            case InventoryItem.Type.SwitcherItem:
                EditorGUILayout.PropertyField(WeaponID_Prop, new GUIContent("SwitcherID:"));
                EditorGUILayout.PropertyField(MarkAsLight_Prop, new GUIContent("Default Light Item:"));
                break;

            case InventoryItem.Type.InventoryItem:
                EditorGUILayout.PropertyField(Amount_Prop, new GUIContent("Amount:"));
                EditorGUILayout.PropertyField(InventoryID_Prop, new GUIContent("InventoryID:"));
                EditorGUILayout.PropertyField(MarkAsLight_Prop, new GUIContent("Default Light Item:"));
                break;

            case InventoryItem.Type.WeaponItem:
                EditorGUILayout.PropertyField(WeaponType_Prop);
                InventoryItem.WeaponType weapType = (InventoryItem.WeaponType)ItemType_Prop.enumValueIndex;

                EditorGUILayout.PropertyField(Amount_Prop, new GUIContent("Amount:"));
                EditorGUILayout.PropertyField(WeaponID_Prop, new GUIContent("WeaponID:"));
                EditorGUILayout.PropertyField(InventoryID_Prop, new GUIContent("InventoryID:"));
                EditorGUILayout.PropertyField(MarkAsLight_Prop, new GUIContent("Default Light Item:"));
                break;

            case InventoryItem.Type.BackpackExpand:
                EditorGUILayout.PropertyField(BackpackExpand_Prop, new GUIContent("Expand Amount:"));
                break;
        }

		serializedObject.ApplyModifiedProperties ();
	}
}
