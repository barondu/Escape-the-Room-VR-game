/*
 * ConfigHandler.cs - script by ThunderWire Games
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderWire.Configuration;

/// <summary>
/// Script which handles all config actions.
/// </summary>
public class ConfigHandler : MonoBehaviour {
	public string ConfigName = "GameConfig";
    public FilePath savePath;
    public bool showDebug;

    [Header("Spinner")]
    public GameObject Spinner;

    [HideInInspector]
    public bool Error = false;

    void Awake()
    {
        ConfigManager.SetFilePath(savePath);
        ConfigManager.EnableDebugging(showDebug);

        if (!GetComponent<HFPS_GameManager>())
        {
            ConfigManager.SetFilename(ConfigName);
            PlayerPrefs.SetString("GameConfig", ConfigName);

            if (!ConfigManager.ExistFile(ConfigName))
            {
                Debug.Log("<color=yellow>Config Warning: " + ConfigName + " does not exist in the Data folder! Config will be created automatically.</color>");
                Error = true;
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("GameConfig"))
            {
                ConfigName = PlayerPrefs.GetString("GameConfig");
            }

            ConfigManager.SetFilename(ConfigName);

            if (!ConfigManager.ExistFile(ConfigName))
            {
                Debug.LogError("Config Error: " + ConfigName + " does not exist in the Data folder!");
                Error = true;
            }
        }
    }

    public void ShowSpinner(float timeSpinning)
    {
        if (Spinner)
        {
            StartCoroutine(Spinning(timeSpinning));
        }
    }

    IEnumerator Spinning(float time)
    {
        Spinner.SetActive(true);
        yield return new WaitForSeconds(time);
        Spinner.SetActive(false);
    }
		
	public string Deserialize(string Section, string Key)
	{
		return ConfigManager.Deserialize (Section, Key);
	}

    public T Deserialize<T>(string Section, string Key)
    {
        return ConfigManager.Deserialize<T>(Section, Key);
    }

    public Dictionary<string, string> DeserializeSection(string Section)
    {
        return ConfigManager.DeserializeSection(Section);
    }

    public void Serialize(string Section, string Key, string Value)
	{
        ConfigManager.Serialize (Section, Key, Value);
	}

    public void Serialize(string Section, Dictionary<string, string> keyValuePair)
    {
        ConfigManager.Serialize(Section, keyValuePair);
    }

    public void CreateSection(string Section)
    {
        ConfigManager.CreateSection(Section);
    }

    public bool ContainsSection(string Section) 
	{
		return ConfigManager.ContainsSection (Section);
	}

	public bool ContainsSectionKey(string Section, string Key) {
        if (ConfigManager.ContainsSection(Section))
        {
            return ConfigManager.ContainsSectionKey(Section, Key);
        }
        return false;
    }

	public void RemoveSectionKey(string Section, string Key) {
        ConfigManager.RemoveSectionKey (Section, Key);
	}

    public void RemoveSection(string Section)
    {
        ConfigManager.RemoveSection(Section);
    }

    public bool ExistFile(string file)
	{
		return ConfigManager.ExistFile (file);
	}

	public string GetKey(int index)
	{
        return ConfigManager.ConfigKeysCache[index];
	}

	public int GetKeysCount()
	{
		return ConfigManager.ConfigKeysCache.Count;
	}

	public int GetKeysSectionCount(string Section)
	{
		return ConfigManager.GetSectionKeys (Section);
	}
}
