/* SaveGameHandler.cs by ThunderWire Games
 * Version 2.0
 */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;
using ThunderWire.JsonManager;

/// <summary>
/// Main script for Save/Load System
/// </summary>
public class SaveGameHandler : MonoBehaviour {

    public SaveLoadScriptable SaveLoadSettings;

    [Header("Settings")]

    [Tooltip("Serialize player data between scenes.")]
    public bool dataBetweenScenes;

    [Header("Other")]
    [Tooltip("Not necessary, if you does not want Fade when scene starts, leave this blank.")]
    public FadePanelControl fadeControl;

    ItemSwitcher switcher;
    private bool loadGame;

    private SaveObject[] saveableObjects;

    [HideInInspector]
    public string lastSave;
    

    void Start()
    {
        switcher = Camera.main.transform.root.GetComponentInChildren<ScriptManager>().GetScript<ItemSwitcher>();
        saveableObjects = FindObjectsOfType<SaveObject>();

        JsonManager.Settings(SaveLoadSettings, true);

        if (PlayerPrefs.HasKey("LoadGame"))
        {
            loadGame = Convert.ToBoolean(PlayerPrefs.GetInt("LoadGame"));

            if (loadGame && PlayerPrefs.HasKey("LoadSaveName"))
            {
                string filename = PlayerPrefs.GetString("LoadSaveName");

                if (File.Exists(JsonManager.GetFilePath(FilePath.GameSavesPath) + filename))
                {
                    JsonManager.DeserializeData(filename);
                    string loadScene = (string)JsonManager.Json()["scene"];
                    lastSave = filename;

                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == loadScene)
                    {
                        LoadSavedSceneData(true);
                    }
                }
                else
                {
                    Debug.Log("No load found: " + filename);
                    loadGame = false;
                }
            }

            if(!loadGame && dataBetweenScenes)
            {
                JsonManager.ClearArray();

                if (File.Exists(JsonManager.GetFilePath(FilePath.GameDataPath) + "_nextSceneData.dat"))
                {
                    JsonManager.DeserializeData(FilePath.GameDataPath, "_nextSceneData.dat");
                    LoadSavedSceneData(false);
                }
            }
        }

        if (fadeControl)
        {
            fadeControl.FadeOutPanel();
        }
    }

    /* LOAD SECTION */
    private void LoadSavedSceneData(bool allData)
    {
        if (allData)
        {
            var posToken = JsonManager.Json()["playerData"]["playerPosition"];
            Camera.main.transform.root.position = posToken.ToObject<Vector3>();

            var rotToken = JsonManager.Json()["playerData"]["cameraRotation"];
            Camera.main.transform.parent.parent.GetComponent<MouseLook>().SetRotation(rotToken.ToObject<Vector2>());
        }

        var healthToken = JsonManager.Json()["playerData"]["playerHealth"];
        Camera.main.transform.root.GetComponent<HealthManager>().Health = (float)healthToken;

        //Load ItemSwitcher
        int switchID = (int)JsonManager.Json()["itemSwitcherData"]["switcherActiveItem"];
        if (switchID != -1)
        {
            switcher.SetActiveItem(switchID);
        }

        switcher.currentLightObject = (int)JsonManager.Json()["itemSwitcherData"]["switcherLightObject"];
        switcher.weaponItem = (int)JsonManager.Json()["itemSwitcherData"]["switcherWeaponItem"];

        //Load ItemSwitcher Item Data
        for (int i = 0; i < switcher.ItemList.Count; i++)
        {
            if (switcher.ItemList[i].GetComponent<SaveHelper>())
            {
                var switcherItemData = JsonManager.Json()["itemSwitcherData"]["switcheritem_" + switcher.ItemList[i].name];
                switcher.ItemList[i].GetComponent<SaveHelper>().LoadSavedValues(switcherItemData);
            }
        }

        //Loading Inventory Data
        StartCoroutine(LoadInventory(JsonManager.Json()["inventoryData"]));

        if (allData)
        {
            //Setting data to saved objects
            for (int i = 0; i < saveableObjects.Length; i++)
            {
                saveableObjects[i].SetObjectData(JsonManager.Json()[saveableObjects[i].uniqueName]);
            }
        }
        else
        {
            File.Delete(JsonManager.GetFilePath(FilePath.GameDataPath) + "_nextSceneData.dat");
        }
    }

    /* LOAD SECTION INVENTORY */
    private IEnumerator LoadInventory(JToken token)
    {
        yield return new WaitUntil(() => GetComponent<Inventory>().slots.Count > 0);

        int slotsCount = (int)token["inv_slots_count"];
        int neededSlots = slotsCount - GetComponent<Inventory>().slots.Count;

        if(neededSlots != 0)
        {
            GetComponent<Inventory>().ExpandSlots(neededSlots);
        }

        for (int i = 0; i < GetComponent<Inventory>().slots.Count; i++)
        {
            JToken slotToken = token["slotsData"]["inv_slot_" + i];
            string slotString = slotToken.ToString();

            if (slotString != "null")
            {
                GetComponent<Inventory>().AddItemSlot((int)slotToken["slotID"], (int)slotToken["itemID"], (int)slotToken["itemAmount"]);
            }
        }
    }

    /* SAVE SECTION */
    public void Save(bool allData)
    {
        JsonManager.ClearArray();
        Dictionary<string, object> playerData = new Dictionary<string, object>();

        if (allData)
        {
            JsonManager.UpdateArray("scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            JsonManager.UpdateArray("dateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            playerData.Add("playerPosition", Camera.main.transform.root.position);
            playerData.Add("cameraRotation", Camera.main.transform.parent.parent.GetComponent<MouseLook>().GetRotation());
        }

        playerData.Add("playerHealth", Camera.main.transform.root.GetComponent<HealthManager>().Health);
        JsonManager.UpdateArray("playerData", playerData);

        Dictionary<string, object> switcherData = new Dictionary<string, object>
        {
            { "switcherActiveItem", switcher.currentItem }, //Save Item Switcher Current Item
            { "switcherLightObject", switcher.currentLightObject },
            { "switcherWeaponItem", switcher.weaponItem }
        };

        //Save Item Switcher Item Data
        for (int i = 0; i < switcher.ItemList.Count; i++)
        {
            if (switcher.ItemList[i].GetComponent<SaveHelper>())
            {
                switcher.ItemList[i].GetComponent<SaveHelper>().CallScriptGetValues();
                switcherData.Add("switcheritem_" + switcher.ItemList[i].name, switcher.ItemList[i].GetComponent<SaveHelper>().GetArray());
            }
        }

        JsonManager.UpdateArray("itemSwitcherData", switcherData);

        Dictionary<string, object> inventoryData = new Dictionary<string, object>
        {
            //Save Inventory Slots Count
            { "inv_slots_count", GetComponent<Inventory>().slots.Count }
        };

        Dictionary<string, object> slotData = new Dictionary<string, object>();
        //Save Inventory Data
        for (int i = 0; i < GetComponent<Inventory>().slots.Count; i++)
        {
            if(GetComponent<Inventory>().slots[i].transform.childCount > 1)
            {
                Dictionary<string, object> itemDataArray = new Dictionary<string, object>();
                InventoryItemData itemData = GetComponent<Inventory>().slots[i].transform.GetChild(1).GetComponent<InventoryItemData>();
                itemDataArray.Clear();

                itemDataArray.Add("slotID", itemData.slotID);
                itemDataArray.Add("itemID", itemData.item.ID);
                itemDataArray.Add("itemAmount", itemData.m_amount);

                slotData.Add("inv_slot_" + i, itemDataArray);
            }
            else
            {
                slotData.Add("inv_slot_" + i, "null");
            }
        }

        inventoryData.Add("slotsData", slotData);

        //InventoryData Serialize
        JsonManager.UpdateArray("inventoryData", inventoryData);

        if (allData)
        {
            for (int i = 0; i < saveableObjects.Length; i++)
            {
                foreach (KeyValuePair<string, object> data in saveableObjects[i].GetObjectData())
                {
                    JsonManager.UpdateArray(data.Key, data.Value);
                }
            }
        }

        SaveGame(!allData);
    }

    public void SaveNextSceneData()
    {
        JsonManager.ClearArray();
        Save(false);
    }

    async void SaveGame(bool betweenScenes)
    {
        string filepath = JsonManager.GetFilePath(FilePath.GameSavesPath);
        GetComponent<HFPS_GameManager>().ShowSaveNotification(1);

        if (!betweenScenes)
        {
            if (Directory.Exists(filepath))
            {
                DirectoryInfo di = new DirectoryInfo(filepath);
                FileInfo[] fi = di.GetFiles("Save?.sav");

                if (fi.Length > 0)
                {
                    string SaveName = "Save" + fi.Length;
                    lastSave = SaveName + ".sav";
                    FileStream file = new FileStream(JsonManager.GetCurrentPath() + SaveName + ".sav", FileMode.OpenOrCreate);
                    await Task.Run(() => JsonManager.SerializeJsonDataAsync(file));
                }
                else
                {
                    lastSave = "Save0.sav";
                    FileStream file = new FileStream(JsonManager.GetCurrentPath() + "Save0.sav", FileMode.OpenOrCreate);
                    await Task.Run(() => JsonManager.SerializeJsonDataAsync(file));
                }
            }
            else
            {
                lastSave = "Save0.sav";
                FileStream file = new FileStream(JsonManager.GetCurrentPath() + "Save0.sav", FileMode.OpenOrCreate);
                await Task.Run(() => JsonManager.SerializeJsonDataAsync(file));
            }
        }
        else
        {
            FileStream file = new FileStream(JsonManager.GetFilePath(FilePath.GameDataPath) + "_nextSceneData.dat", FileMode.OpenOrCreate);
            await Task.Run(() => JsonManager.SerializeJsonDataAsync(file, true));
        }
    }
}
