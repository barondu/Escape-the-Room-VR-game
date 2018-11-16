using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ThunderWire.Scene;

public class SceneLoader : MonoBehaviour {
    public List<SceneInfo> sceneInfos = new List<SceneInfo>();

    [Space(7)]

    public FadePanelControl fadeController;
    public TipsManager tipsManager;
    public GameObject SpinnerGO;
    public Text sceneName;
    public Text sceneDescription;
    public Image backgroundImg;
    public GameObject manuallySwitchText;

    [Tooltip("Switch scene by pressing any button")]
    public bool SwitchManually;

    [Space(7)]

    [Tooltip("Background Loading Priority")]
    public UnityEngine.ThreadPriority threadPriority = UnityEngine.ThreadPriority.High;
    public int timeBeforeLoad;

    void Start()
    {
        Time.timeScale = 1f;
        SpinnerGO.transform.parent.gameObject.SetActive(true);
        manuallySwitchText.SetActive(false);
        if (tipsManager)
        {
            tipsManager.TipsText.gameObject.SetActive(true);
        }

        SceneTool.threadPriority = threadPriority;

        if (PlayerPrefs.HasKey("LevelToLoad"))
        {
            string scene = PlayerPrefs.GetString("LevelToLoad");
            LoadLevelAsync(scene);
        }
        else
        {
            SpinnerGO.GetComponent<Spinner>().isSpinning = false;
            Debug.LogError("Loading Error: There is no scene to load!");
        }
    }

	public void LoadLevelAsync(string scene)
	{
        sceneName.text = scene;

        if (sceneInfos.Count > 0)
        {
            SceneInfo sceneInfo = sceneInfos.SingleOrDefault(info => info.SceneName == scene);
            if (sceneInfo != null)
            {
                sceneDescription.text = sceneInfo.SceneDescription;
                backgroundImg.sprite = sceneInfo.Background;
            }
            else
            {
                sceneDescription.text = "";
            }
        }
        else
        {
            sceneDescription.text = "";
        }

        StartCoroutine(LoadScene(scene, timeBeforeLoad));
    }

    IEnumerator LoadScene(string scene, int timeWait)
    {
        yield return new WaitForSeconds(timeWait);

        if (!SwitchManually)
        {
            StartCoroutine(SceneTool.LoadSceneAsyncSwitch(scene));
        }
        else
        {
            StartCoroutine(SceneTool.LoadSceneAsync(scene));

            yield return new WaitUntil(() => SceneTool.LoadingDone);

            SpinnerGO.transform.parent.gameObject.SetActive(false);

            manuallySwitchText.SetActive(true);

            if (tipsManager)
            {
                tipsManager.TipsText.gameObject.SetActive(false);
            }

            yield return new WaitUntil(() => Input.anyKey);

            if (!fadeController)
            {
                SceneTool.AllowSceneActivation();
            }
            else
            {
                fadeController.FadeInPanel();
                yield return new WaitUntil(() => !fadeController.isFading());
                SceneTool.AllowSceneActivation();
            }
        }
    }
}

[System.Serializable]
public class SceneInfo
{
    public string SceneName;
    [Multiline]
    public string SceneDescription;
    public Sprite Background;

    public SceneInfo (string name, string desc, Sprite sprite)
    {
        SceneName = name;
        SceneDescription = desc;
        Background = sprite;
    }
}