using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using ThunderWire.JsonManager;

public class LoadManager : MonoBehaviour
{
    public SaveLoadScriptable SaveLoadSettings;

    [Space(10)]

    public GameObject SavedGamePrefab;
    public Transform SavedGameContent;

    public Text EmptyText;

    public Button loadButton;
    public Button deleteButton;

    private string filepath;
    private FileInfo[] fi;
    private DirectoryInfo di;

    private List<GameObject> saveCache = new List<GameObject>();
    private GameObject save;

    void Start()
    {
        JsonManager.Settings(SaveLoadSettings);
        Time.timeScale = 1f;

        LoadSaves();

        loadButton.onClick.AddListener(Load);
        deleteButton.onClick.AddListener(Delete);
    }

    void LoadSaves()
    {
        filepath = JsonManager.GetCurrentPath();
        if (Directory.Exists(filepath))
        {
            di = new DirectoryInfo(filepath);
            fi = di.GetFiles("Save?.sav");
            if (fi.Length > 0)
            {
                EmptyText.gameObject.SetActive(false);
                for (int i = 0; i < fi.Length; i++)
                {
                    JsonManager.DeserializeData(fi[i].Name);
                    GameObject savedGame = Instantiate(SavedGamePrefab);
                    savedGame.transform.SetParent(SavedGameContent);
                    savedGame.transform.localScale = new Vector3(1, 1, 1);
                    string scene = (string)JsonManager.Json()["scene"];
                    string date = (string)JsonManager.Json()["dateTime"];
                    savedGame.GetComponent<SavedGame>().SetSavedGame(fi[i].Name, scene, date);
                    saveCache.Add(savedGame);
                }
            }
            else
            {
                EmptyText.gameObject.SetActive(true);
            }
        }
    }

    public void Delete()
    {
        string pathToFile = filepath + save.GetComponent<SavedGame>().save;
        File.Delete(pathToFile);

        foreach (Transform g in SavedGameContent)
        {
            Destroy(g.gameObject);
        }

        saveCache.Clear();
        LoadSaves();
    }

    public void Load()
    {
        PlayerPrefs.SetInt("LoadGame", 1);
        PlayerPrefs.SetString("LoadSaveName", save.GetComponent<SavedGame>().save);
        PlayerPrefs.SetString("LevelToLoad", save.GetComponent<SavedGame>().scene);

        SceneManager.LoadScene(1);
    }

    public void NewGame(string Scene)
    {
        PlayerPrefs.SetInt("LoadGame", 0);
        PlayerPrefs.SetString("LevelToLoad", Scene);

        SceneManager.LoadScene(1);
    }

    void Update()
    {
        if(save != null)
        {
            loadButton.interactable = true;
            deleteButton.interactable = true;
        }
        else
        {
            loadButton.interactable = false;
            deleteButton.interactable = false;
        }

        if (EventSystem.current.currentSelectedGameObject)
        {
            GameObject select = EventSystem.current.currentSelectedGameObject;

            if (saveCache.Contains(select))
            {
                SelectSave(select);
            }
        }
        else
        {
            Deselect();
        }
    }

    private void SelectSave(GameObject SaveObject)
    {
        save = saveCache[saveCache.IndexOf(SaveObject)];
    }

    public void Deselect()
    {
        save = null;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
