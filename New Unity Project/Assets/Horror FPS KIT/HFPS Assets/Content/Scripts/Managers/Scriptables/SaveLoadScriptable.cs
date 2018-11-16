using System;
using UnityEngine;
using ThunderWire.JsonManager;

[Serializable]
public class SaveLoadScriptable : ScriptableObject
{
    public bool enableEncryption;
    public FilePath filePath = FilePath.GameDataPath;
    public string cipherKey;
}