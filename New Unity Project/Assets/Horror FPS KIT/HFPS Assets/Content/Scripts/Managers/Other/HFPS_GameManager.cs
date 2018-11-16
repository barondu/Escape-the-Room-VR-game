/*
 * HFPS_GameManager.cs - script written by ThunderWire Games
 * ver. 1.32
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;

public enum hideType
{
    Interact, Grab, Examine
}

/// <summary>
/// HFPS - Game Manager
/// </summary>
public class HFPS_GameManager : MonoBehaviour {

    private ConfigHandler configHandler;

    private PostProcessingBehaviour processingBehaviour;
    private PostProcessingProfile processingProfile;

    private ColorGradingModel.Settings colorGrading;

    [Header("Main")]
    public GameObject Player;
    public InputController inputManager;
    public Inventory inventoryScript;
    public string m_sceneLoader;

    private SaveGameHandler saveHandler;

    [HideInInspector]
    public ScriptManager scriptManager;

    [HideInInspector]
    public HealthManager healthManager;

    [Header("Cursor")]
    public bool m_ShowCursor = false;

    [Header("Game Panels")]
    public GameObject PauseGamePanel;
    public GameObject MainGamePanel;
    public GameObject PlayerDeadPanel;
    public GameObject TabButtonPanel;

    [Header("Pause UI")]
    public KeyCode ShowPauseMenuKey = KeyCode.Escape;
    public bool reallyPause = false;
    public bool useGreyscale = true;
    public float greyscaleFadeSpeed;

    private bool greyscale;
    private bool greyscaleIn = false;
    private bool greyscaleOut = false;

    [HideInInspector] public bool isPaused = false;

    [Header("Paper UI")]
    public GameObject PaperTextUI;
    public Text PaperReadText;

    [Header("UI Percentagles")]
    public GameObject BatteryRemaining;
    public GameObject OilRemaining;

    [Header("Valve UI")]
    public Slider ValveSlider;

    private float slideTime;
    private float slideValue;


    [Header("Notification UI")]
    public GameObject saveNotification;
    public GameObject NotificationPanel;
    public GameObject NotificationPrefab;
    public Sprite WarningSprite;
    public float saveFadeSpeed;

    private List<GameObject> Notifications = new List<GameObject>();

    [Header("Hints UI")]
    public Text HintText;

    [Header("Crosshair")]
    public Image Crosshair;

    [Header("UI Amounts")]
    public Text HealthText;
    public GameObject AmmoUI;
    public Text BulletsText;
    public Text MagazinesText;

    [Header("Interact UI")]
    public GameObject InteractUI;
    public GameObject InteractInfoUI;
    public GameObject KeyboardButton1;
    public GameObject KeyboardButton2;

    [Header("Down Examine Buttons")]
    public GameObject DownExamineUI;
    public GameObject ExamineButton1;
    public GameObject ExamineButton2;
    public GameObject ExamineButton3;

    [Header("Down Grab Buttons")]
    public GameObject DownGrabUI;
    public GameObject GrabButton1;
    public GameObject GrabButton2;
    public GameObject GrabButton3;

    public Sprite DefaultSprite;

    [HideInInspector]
    public bool isHeld;

    [HideInInspector]
    public bool canGrab;
    [HideInInspector]
    public bool isGrabbed;

    private float fadeHint;

    private KeyCode UseKey;
    private KeyCode GrabKey;
    private KeyCode ThrowKey;
    private KeyCode RotateKey;
    private KeyCode InventoryKey;

    private bool uiInteractive = true;
    private bool isOverlapping;
    private bool isPressed;
    private bool antiSpam;

    [HideInInspector]
    public bool ConfigError;

    void Awake()
    {
        configHandler = GetComponent<ConfigHandler>();
        healthManager = Camera.main.transform.root.gameObject.GetComponent<HealthManager>();
        scriptManager = Player.transform.GetChild(0).transform.GetChild(0).GetComponent<ScriptManager>();
        saveHandler = GetComponent<SaveGameHandler>();

        uiInteractive = true;
    }

    void Start()
    {
        TabButtonPanel.SetActive(false);
        saveNotification.SetActive(false);
        HideSprites(hideType.Interact);
        HideSprites(hideType.Grab);
        HideSprites(hideType.Examine);
        Unpause();

        if (m_ShowCursor) {
            Cursor.visible = (true);
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.visible = (false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        processingBehaviour = Camera.main.gameObject.GetComponent<PostProcessingBehaviour>();
        processingProfile = processingBehaviour.profile;
        colorGrading = processingProfile.colorGrading.settings;

        if (useGreyscale)
        {
            processingProfile.colorGrading.enabled = true;
            colorGrading.basic.saturation = 1;
        }

        HintText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (inputManager.HasInputs())
        {
            UseKey = inputManager.GetInput("Use");
            GrabKey = inputManager.GetInput("Pickup");
            ThrowKey = inputManager.GetInput("Throw");
            RotateKey = inputManager.GetInput("Fire");
            InventoryKey = inputManager.GetInput("Inventory");
        }

        if (configHandler.ContainsSectionKey("Game", "Volume"))
        {
            float volume = float.Parse(configHandler.Deserialize("Game", "Volume"));
            AudioListener.volume = volume;
        }

        if (!uiInteractive) return;

        if (Input.GetKeyDown(ShowPauseMenuKey) && !isPressed && !antiSpam)
        {
            isPressed = true;
            PauseGamePanel.SetActive(!PauseGamePanel.activeSelf);
            MainGamePanel.SetActive(!MainGamePanel.activeSelf);

            StartCoroutine(AntiPauseSpam());

            if (useGreyscale)
            {
                if (!greyscaleIn)
                {
                    GreyscaleScreen(true);
                }
                else if(!greyscaleOut)
                {
                    GreyscaleScreen(false);
                }
            }

            isPaused = !isPaused;
        }
        else if (isPressed)
        {
            isPressed = false;
        }

        if (PauseGamePanel.activeSelf && isPaused && isPressed)
        {
            Crosshair.enabled = false;
            LockStates(true, true, true, true, 3);
            scriptManager.GetScript<PlayerFunctions>().enabled = false;
            if (reallyPause)
            {
                Time.timeScale = 0;
            }
        }
        else if (isPressed)
        {
            Crosshair.enabled = true;
            LockStates(false, true, true, true, 3);
            scriptManager.GetScript<PlayerFunctions>().enabled = true;
            if (TabButtonPanel.activeSelf)
            {
                TabButtonPanel.SetActive(false);
            }
            if (reallyPause)
            {
                Time.timeScale = 1;
            }
        }

        if (Input.GetKeyDown(InventoryKey) && !isPressed && !isPaused && !isOverlapping)
        {
            isPressed = true;
            TabButtonPanel.SetActive(!TabButtonPanel.activeSelf);
        }
        else if (isPressed)
        {
            isPressed = false;
        }

        if (TabButtonPanel.activeSelf && isPressed)
        {
            Crosshair.enabled = false;
            LockStates(true, true, true, true, 0);
            HideSprites(hideType.Interact);
            HideSprites(hideType.Grab);
            HideSprites(hideType.Examine);
        }
        else if (isPressed)
        {
            Crosshair.enabled = true;
            LockStates(false, true, true, true, 0);
        }

        if (Notifications.Count > 3)
        {
            Destroy(Notifications[0]);
            Notifications.RemoveAll(GameObject => GameObject == null);
        }

        if (greyscale)
        {
            if (greyscaleIn)
            {
                if (colorGrading.basic.saturation > 0)
                {
                    colorGrading.basic.saturation -= Time.fixedDeltaTime * greyscaleFadeSpeed;
                }
                else if (colorGrading.basic.saturation <= 0)
                {
                    colorGrading.basic.saturation = 0;
                }
            }

            if (greyscaleOut)
            {
                if (colorGrading.basic.saturation <= 1)
                {
                    colorGrading.basic.saturation += Time.fixedDeltaTime * greyscaleFadeSpeed;
                }
                else if (colorGrading.basic.saturation >= 1)
                {
                    colorGrading.basic.saturation = 1;
                    greyscaleOut = false;
                }
            }
        }

        processingProfile.colorGrading.settings = colorGrading;
    }

    private void OnDisable()
    {
        colorGrading.basic.saturation = 1;
        processingProfile.colorGrading.settings = colorGrading;
    }

    IEnumerator AntiPauseSpam()
    {
        antiSpam = true;
        yield return new WaitForSecondsRealtime(0.5f);
        antiSpam = false;
    }

    public void GreyscaleScreen(bool Greyscale)
    {
        greyscale = true;

        switch (Greyscale)
        {
            case true:
                greyscaleIn = true;
                greyscaleOut = false;
                break;
            case false:
                greyscaleIn = false;
                greyscaleOut = true;
                break;
        }
    }

    public void Unpause()
    {
        if (TabButtonPanel.activeSelf)
        {
            TabButtonPanel.SetActive(false);
        }

        if (useGreyscale)
        {
            GreyscaleScreen(false);
        }

        Crosshair.enabled = true;
        LockStates(false, true, true, true, 3);
        PauseGamePanel.SetActive(false);
        MainGamePanel.SetActive(true);
        isPaused = false;

        if (reallyPause)
        {
            Time.timeScale = 1;
        }
    }


    /// <summary>
    /// Restrict some Player functions.
    /// </summary>
    /// <param name="LockState">True = Lock, False = Unlock</param>
    /// <param name="Interact">Restrict Player interact function?</param>
    /// <param name="Controller">Restrict Player movement?</param>
    /// <param name="CursorVisible">Show, Hide cursor?</param>
    /// <param name="BlurLevel">0,1,2,3 = Blur Levels</param>
    public void LockStates(bool LockState, bool Interact, bool Controller, bool CursorVisible, int BlurLevel) {
        switch (LockState) {
            case true:
                Player.transform.GetChild(0).GetChild(0).GetComponent<MouseLook>().enabled = false;
                if (Interact) {
                    scriptManager.GetScript<InteractManager>().inUse = true;
                }
                if (Controller) {
                    Player.GetComponent<PlayerController>().controllable = false;
                    scriptManager.GetScript<PlayerFunctions>().enabled = false;
                }
                if (BlurLevel > 0) {
                    if (BlurLevel == 1) { scriptManager.MainCameraBlur.enabled = true; }
                    if (BlurLevel == 2) { scriptManager.ArmsCameraBlur.enabled = true; }
                    if (BlurLevel == 3)
                    {
                        scriptManager.MainCameraBlur.enabled = true;
                        scriptManager.ArmsCameraBlur.enabled = true;
                    }
                }
                if (CursorVisible) {
                    ShowCursor(true);
                }
                break;
            case false:
                Player.transform.GetChild(0).GetChild(0).GetComponent<MouseLook>().enabled = true;
                if (Interact) {
                    scriptManager.GetScript<InteractManager>().inUse = false;
                }
                if (Controller) {
                    Player.GetComponent<PlayerController>().controllable = true;
                    scriptManager.GetScript<PlayerFunctions>().enabled = true;
                }
                if (BlurLevel > 0) {
                    if (BlurLevel == 1) { scriptManager.MainCameraBlur.enabled = false; }
                    if (BlurLevel == 2) { scriptManager.ArmsCameraBlur.enabled = false; }
                    if (BlurLevel == 3)
                    {
                        scriptManager.MainCameraBlur.enabled = false;
                        scriptManager.ArmsCameraBlur.enabled = false;
                    }
                }
                if (CursorVisible) {
                    ShowCursor(false);
                }
                break;
        }
    }

    public void UIPreventOverlap(bool State)
    {
        isOverlapping = State;
    }

    public void MouseLookState(bool State)
    {
        switch (State) {
            case true:
                Player.transform.GetChild(0).GetChild(0).GetComponent<MouseLook>().enabled = true;
                break;
            case false:
                Player.transform.GetChild(0).GetChild(0).GetComponent<MouseLook>().enabled = false;
                break;
        }
    }

    public void ShowCursor(bool state)
    {
        switch (state) {
            case true:
                Cursor.visible = (true);
                Cursor.lockState = CursorLockMode.None;
                break;
            case false:
                Cursor.visible = (false);
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }

    public void AddPickupMessage(string itemName)
    {
        GameObject PickupMessage = Instantiate(NotificationPrefab);
        Notifications.Add(PickupMessage);
        PickupMessage.transform.SetParent(NotificationPanel.transform);
        PickupMessage.GetComponent<ItemPickupNotification>().SetPickupNotification(itemName);
    }

    public void AddMessage(string message)
    {
        GameObject Message = Instantiate(NotificationPrefab);
        Notifications.Add(Message);
        Message.transform.SetParent(NotificationPanel.transform);
        Message.GetComponent<ItemPickupNotification>().SetNotification(message);
    }

    public void WarningMessage(string warning)
    {
        GameObject Message = Instantiate(NotificationPrefab);
        Notifications.Add(Message);
        Message.transform.SetParent(NotificationPanel.transform);
        Message.GetComponent<ItemPickupNotification>().SetNotificationIcon(warning, WarningSprite);
    }

    public void ShowExamineText(string text)
    {
        StopAllCoroutines();
        HintText.text = text;
        UIFader.SetColor(HintText.color);
        HintText.gameObject.SetActive(true);
        StartCoroutine(UIFader.FadeIn(1.2f));
        StartCoroutine(UpdateHintColor());
        StartCoroutine(FadeExamine());
    }

    public void ShowHint(string hint)
    {
        StopAllCoroutines();
        HintText.text = hint;
        UIFader.SetColor(HintText.color);
        HintText.gameObject.SetActive(true);
        StartCoroutine(UIFader.FadeIn(1.2f));
        StartCoroutine(UpdateHintColor());
        StartCoroutine(FadeHint());
    }

    public void HideHint()
    {
        if (!UIFader.isFaded)
        {
            Color col = HintText.color;
            col.a = 0f;
            HintText.color = col;
            HintText.gameObject.SetActive(false);
        }
        else
        {
            UIFader.canFadeOut = true;
        }
    }

    IEnumerator UpdateHintColor()
    {
        while (true)
        {
            HintText.color = UIFader.GetUIColor();
            yield return null;
        }
    }

    IEnumerator FadeExamine()
    {
        yield return new WaitUntil(() => UIFader.canFadeOut);
        StartCoroutine(UIFader.FadeOut(2.5f));

        yield return new WaitUntil(() => UIFader.isFaded);
        HintText.gameObject.SetActive(false);
    }

    IEnumerator FadeHint()
    {
        yield return new WaitForSecondsRealtime(3f);
        UIFader.canFadeOut = true;
        StartCoroutine(UIFader.FadeOut(2.5f));

        yield return new WaitUntil(() => UIFader.isFaded);
        HintText.gameObject.SetActive(false);
    }

    public void NewValveSlider(float start, float time)
    {
        ValveSlider.gameObject.SetActive(true);
        StartCoroutine(MoveValveSlide(start, 10f, time));
    }

    public void DisableValveSlider()
    {
        ValveSlider.gameObject.SetActive(false);
        StopCoroutine(MoveValveSlide(0,0,0));
    }

    public IEnumerator MoveValveSlide(float start, float end, float time)
    {
        var currentValue = start;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / (time * 10);
            ValveSlider.value = Mathf.Lerp(currentValue, end, t);
            yield return null;
        }
    }

    public void ShowSaveNotification(float time)
    {
        StartCoroutine(FadeInSave(time));
    }

    IEnumerator FadeInSave(float t)
    {
        saveNotification.SetActive(true);
        Color color = saveNotification.GetComponent<Image>().color;

        color.a = 0;
        saveNotification.GetComponent<Image>().color = color;

        while(saveNotification.GetComponent<Image>().color.a <= 0.95f)
        {
            color.a += Time.fixedDeltaTime * saveFadeSpeed;
            saveNotification.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 1;
        saveNotification.GetComponent<Image>().color = color;

        yield return new WaitForSecondsRealtime(t);
        StartCoroutine(FadeOutSave());
    }

    IEnumerator FadeOutSave()
    {
        Color color = saveNotification.GetComponent<Image>().color;

        while (saveNotification.GetComponent<Image>().color.a >= 0.1)
        {
            color.a -= Time.fixedDeltaTime * saveFadeSpeed;
            saveNotification.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 0;
        saveNotification.GetComponent<Image>().color = color;

        saveNotification.SetActive(false);
    }

    public bool CheckController()
	{
		return Player.GetComponent<PlayerController> ().controllable;
	}

    private void SetKey(Transform KeyObject, KeyCode Key, string customName = "")
    {
        string m_key = Key.ToString();
        KeyObject.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(customName))
        {
            KeyObject.GetChild(2).GetComponent<Text>().text = customName;
        }

        if (m_key == "Mouse0" || m_key == "Mouse1" || m_key == "Mouse2")
        {
            KeyObject.GetChild(0).GetComponent<Image>().sprite = GetKeySprite(m_key);
            KeyObject.GetChild(0).gameObject.SetActive(true);
            KeyObject.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            KeyObject.GetChild(1).GetChild(0).GetComponent<Text>().text = m_key;
            KeyObject.GetChild(0).gameObject.SetActive(false);
            KeyObject.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void ShowInteractSprite(int Row, string KeyName, KeyCode Key)
    {
        if (isHeld) return;
        InteractUI.SetActive(true);
        switch (Row)
        {
            case 1:
                SetKey(KeyboardButton1.transform, Key, KeyName);
                break;
            case 2:
                SetKey(KeyboardButton2.transform, Key, KeyName);
                break;
        }
    }

    public void ShowInteractInfo(string info)
    {
        InteractInfoUI.SetActive(true);
        InteractInfoUI.GetComponent<Text>().text = info;
    }

    public void ShowExamineSprites(bool btn1 = true, bool btn2 = true, bool btn3 = true, string useText = "Take")
    {
        if (btn1) { SetKey(ExamineButton1.transform, GrabKey); } else { ExamineButton1.gameObject.SetActive(false); }
        if (btn2) { SetKey(ExamineButton2.transform, UseKey, useText); } else { ExamineButton2.gameObject.SetActive(false); }
        if (btn3) { SetKey(ExamineButton3.transform, RotateKey); } else { ExamineButton3.gameObject.SetActive(false); }
        DownExamineUI.SetActive(true);
    }

    public void ShowExamineSprites(KeyCode ExamineKey, string ExamineText)
    {
        SetKey(ExamineButton1.transform, GrabKey);
        SetKey(ExamineButton2.transform, ExamineKey, ExamineText);
        SetKey(ExamineButton3.transform, RotateKey);
        DownExamineUI.SetActive(true);
    }

    public void ShowGrabSprites()
    {
        SetKey(GrabButton1.transform, GrabKey);
        SetKey(GrabButton2.transform, RotateKey);
        SetKey(GrabButton3.transform, ThrowKey);
        DownGrabUI.SetActive(true);
    }

    public Sprite GetKeySprite(string Key)
    {
        return Resources.Load<Sprite>(Key);
    }

    public void HideSprites(hideType type)
	{
		switch (type) {
            case hideType.Interact:
                KeyboardButton1.SetActive(false);
                KeyboardButton2.SetActive(false);
                InteractInfoUI.SetActive(false);
                InteractUI.SetActive(false);
                break;
            case hideType.Grab:
                DownGrabUI.SetActive(false);
                break;
            case hideType.Examine:
                DownExamineUI.SetActive(false);
                break;
		}
	}

    public void ShowDeadPanel()
    {
        LockStates(true, true, true, true, 0);
        scriptManager.GetScript<ItemSwitcher>().DisableItems();

        PauseGamePanel.SetActive(false);
        MainGamePanel.SetActive(false);
        PlayerDeadPanel.SetActive(true);

        uiInteractive = false;
    }

    public void ChangeScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void LoadNextScene(string scene)
    {
        if (saveHandler)
        {
            if (saveHandler.dataBetweenScenes)
            {
                saveHandler.SaveNextSceneData();

                if (!isPaused)
                {
                    LockStates(true, true, true, false, 0);
                }

                if (saveHandler.fadeControl)
                {
                    saveHandler.fadeControl.FadeInPanel();
                }

                StartCoroutine(LoadScene(scene, false));
            }
        }
    }

    public void Retry()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, true));
    }

    private IEnumerator LoadScene(string scene, bool LoadSceneData)
    {
        yield return new WaitUntil(() => !saveHandler.fadeControl.isFading());

        PlayerPrefs.SetString("LoadSaveName", GetComponent<SaveGameHandler>().lastSave);
        PlayerPrefs.SetInt("LoadGame", System.Convert.ToInt32(LoadSceneData));
        PlayerPrefs.SetString("LevelToLoad", scene);
        SceneManager.LoadScene(m_sceneLoader);
    }
}