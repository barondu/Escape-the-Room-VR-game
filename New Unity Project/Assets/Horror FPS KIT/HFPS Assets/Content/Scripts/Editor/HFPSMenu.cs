using System.Security.Cryptography;
using System.Text;
using System.IO;
using ThunderWire.JsonManager;
using UnityEngine;
using UnityEditor;

public class HFPSMenu : EditorWindow
{
    private bool encrypt;
    private FilePath filePath;
    private string key;

    [MenuItem("Tools/HFPS KIT/Setup/Game")]
    static void SetupGame()
    {
        if(GameObject.Find("Main Camera"))
        {
            DestroyImmediate(Camera.main.gameObject);
        }

        if(GameObject.Find("Directional Light"))
        {
            DestroyImmediate(GameObject.Find("Directional Light"));
        }

        GameObject GameManager = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Setup/Game/GAMEMANAGER")) as GameObject;
        GameObject Player = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Setup/Game/FPSPLAYER")) as GameObject;

        Player.transform.position = new Vector3(0, 0, 0);
        GameManager.GetComponent<HFPS_GameManager>().Player = Player;
        Player.GetComponentInChildren<ScriptManager>().m_GameManager = GameManager.GetComponent<HFPS_GameManager>();
        Player.GetComponentInChildren<ScriptManager>().m_InputController = GameManager.GetComponent<InputController>();
    }

    [MenuItem("Tools/HFPS KIT/Setup/Game", true)]
    static bool CheckSetupGame()
    {
        if (GameObject.Find("MENUMANAGER"))
        {
            return false;
        }

        if (GameObject.Find("GAMEMANAGER"))
        {
            return false;
        }

        if (GameObject.Find("FPSPLAYER"))
        {
            return false;
        }

        return true;
    }

    [MenuItem("Tools/HFPS KIT/Setup/MainMenu")]
    static void SetupMainMenu()
    {
        if (GameObject.Find("Main Camera"))
        {
            DestroyImmediate(Camera.main.gameObject);
        }

        if (GameObject.Find("Directional Light"))
        {
            DestroyImmediate(GameObject.Find("Directional Light"));
        }

        GameObject MenuManager =  PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Setup/MainMenu/MENUMANAGER")) as GameObject;
    }

    [MenuItem("Tools/HFPS KIT/Setup/MainMenu", true)]
    static bool CheckSetupMainMenu()
    {
        if (GameObject.Find("MENUMANAGER"))
        {
            return false;
        }

        if (GameObject.Find("GAMEMANAGER"))
        {
            return false;
        }

        if (GameObject.Find("FPSPLAYER"))
        {
            return false;
        }

        return true;
    }

    [MenuItem("Tools/HFPS KIT/Setup/Fix Setup")]
    static void FixGameSetup()
    {
        if (GameObject.Find("Main Camera"))
        {
            DestroyImmediate(Camera.main.gameObject);
        }

        if (GameObject.Find("Directional Light"))
        {
            DestroyImmediate(GameObject.Find("Directional Light"));
        }

        GameObject GameManager;
        GameObject Player;

        if (GameObject.Find("GAMEMANAGER"))
        {
            GameManager = GameObject.Find("GAMEMANAGER");
        }
        else
        {
            GameManager = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Setup/Game/GAMEMANAGER")) as GameObject;
        }

        if (GameObject.Find("FPSPLAYER"))
        {
            Player = GameObject.Find("FPSPLAYER");
        }
        else
        {
            Player = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Setup/Game/FPSPLAYER")) as GameObject;
        }

        GameManager.GetComponent<HFPS_GameManager>().Player = Player;
        Player.GetComponentInChildren<ScriptManager>().m_GameManager = GameManager.GetComponent<HFPS_GameManager>();
        Player.GetComponentInChildren<ScriptManager>().m_InputController = GameManager.GetComponent<InputController>();

        EditorUtility.SetDirty(GameManager.GetComponent<HFPS_GameManager>());
        EditorUtility.SetDirty(Player.GetComponentInChildren<ScriptManager>());

        Debug.Log("<color=green>Everything should be OK!</color>");
    }

    [MenuItem("Tools/HFPS KIT/SaveGame/" + "Set LoadGame")]
    static void SetLoadGame()
    {
        int display = EditorUtility.DisplayDialogComplex("Set LoadGame State", "Load SavedGame on Start?", "Yes", "No", "Cancel");
        if (display == 0)
        {
            PlayerPrefs.SetInt("LoadGame", 1);
            Debug.Log("LoadGame was set to \"True\"");
        }
        else if (display == 1)
        {
            PlayerPrefs.SetInt("LoadGame", 0);
            Debug.Log("LoadGame was set to \"False\"");
        }
        else if (display == 2)
        {
            return;
        }
    }

    [MenuItem("Tools/HFPS KIT/SaveGame/" + "Delete SavedGame")]
    static void DeleteSavedGame()
    {
        if (Directory.Exists(GetPath()))
        {
            string[] files = Directory.GetFiles(GetPath(), "Save?.sav");
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }

                EditorUtility.DisplayDialog("SaveGame Deleted", "Deleting SavedGame is completed.", "Okay");
            }
            else
            {
                EditorUtility.DisplayDialog("Directory empty", "Folder is empty.", "Okay");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Directory not found", "Failed to find Directory:  " + GetPath(), "Okay");
        }
    }

    [MenuItem("Tools/HFPS KIT/" + "Scriptables" + "/Create Inventory Database")]
    static void CreateInventoryDatabase()
    {
        CreateAssetFile<InventoryScriptable>("InventoryDatabase");
    }

    [MenuItem("Tools/HFPS KIT/" + "Scriptables" + "/Create Graphic Scriptable")]
    static void CreateOptionsAsset()
    {
        CreateAssetFile<GraphicScriptable>("GraphicSettings");
    }

    [MenuItem("Tools/HFPS KIT/" + "Scriptables" + "/Create Input Scriptable")]
    static void CreateInputsAsset()
    {
        CreateAssetFile<InputScriptable>("InputMapper");
    }

    [MenuItem("Tools/HFPS KIT/" + "Scriptables" + "/Create JsonManager Scriptable")]
    public static void ShowWindow()
    {
        GetWindow<HFPSMenu>(false, "Save/Load Editor", true);
    }

    [MenuItem("Tools/HFPS KIT/Add FloatingIcon")]
    static void AddFloatingIcon()
    {
        UIFloatingItem uIFloatingItem = GameObject.Find("GAMEMANAGER").GetComponent<UIFloatingItem>();
        uIFloatingItem.FloatingIcons.Add(Selection.activeGameObject);
        EditorUtility.SetDirty(uIFloatingItem);
        Debug.Log("<color=green>" + Selection.activeGameObject.name + " is marked as Floating Icon</color>");
    }

    void OnGUI()
    {
        encrypt = EditorGUILayout.Toggle("Encrypt Data:", encrypt);
        filePath = (FilePath)EditorGUILayout.EnumPopup("File Path:", FilePath.GameDataPath);
        key = EditorGUILayout.TextField("Cipher Key", key);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create", GUILayout.Width(100), GUILayout.Height(30)))
        {
            SaveLoadScriptable asset = CreateInstance<SaveLoadScriptable>();

            asset.enableEncryption = encrypt;
            asset.filePath = filePath;
            asset.cipherKey = MD5Hash(key);

            AssetDatabase.CreateAsset(asset, "Assets/Horror FPS Kit/HFPS Assets/Scriptables/Game Settings/" + "SaveLoadSettings" + ".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }

    private static void CreateAssetFile<T>(string AssetName) where T : ScriptableObject
    {
        var asset = CreateInstance<T>();

        ProjectWindowUtil.CreateAsset(asset, "Assets/Horror FPS Kit/HFPS Assets/Scriptables/Game Settings/New " + AssetName + ".asset");
    }

    public static string MD5Hash(string Data)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(Data));

        StringBuilder stringBuilder = new StringBuilder();

        foreach (byte b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }

        return stringBuilder.ToString();
    }

    private static string GetPath()
    {
        string path = "Assets/Horror FPS Kit/HFPS Assets/Scriptables/Game Settings/";

        if (Directory.Exists(path))
        {
            if (Directory.GetFiles(path).Length > 0)
            {
                return JsonManager.GetFilePath(AssetDatabase.LoadAssetAtPath<SaveLoadScriptable>(path + "SaveLoadSettings.asset").filePath);
            }
            return JsonManager.GetFilePath(FilePath.GameDataPath);
        }
        else
        {
            return JsonManager.GetFilePath(FilePath.GameDataPath);
        }
    }
}

public static class ScriptableFinder
{
    public static T GetScriptable<T>(string AssetName) where T : ScriptableObject
    {
        string path = "Assets/Horror FPS Kit/HFPS Assets/Scriptables/Game Settings/";

        if (Directory.Exists(path))
        {
            if (Directory.GetFiles(path).Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath<T>(path + AssetName + ".asset");
            }
            return null;
        }
        else
        {
            return null;
        }
    }
}
