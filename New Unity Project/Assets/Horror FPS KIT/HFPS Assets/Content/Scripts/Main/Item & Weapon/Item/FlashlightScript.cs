/*
 * FlashlightScript.cs - written by ThunderWire Studio
 * Script for Flashlight Controls
 * Updated ver 1.22
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class BatterySpritesClass
{
    public Sprite Battery_0_red;
    public Sprite Battery_5;
    public Sprite Battery_10;
    public Sprite Battery_15;
    public Sprite Battery_20;
    public Sprite Battery_25;
    public Sprite Battery_30;
    public Sprite Battery_35;
    public Sprite Battery_40;
    public Sprite Battery_45;
    public Sprite Battery_50;
    public Sprite Battery_55;
    public Sprite Battery_60;
    public Sprite Battery_65;
    public Sprite Battery_70;
    public Sprite Battery_75;
    public Sprite Battery_80;
    public Sprite Battery_85;
    public Sprite Battery_90;
    public Sprite Battery_95;
    public Sprite Battery_100;
}

public class FlashlightScript : MonoBehaviour {

    private ScriptManager scriptManager;
    private InputController inputManager;
    private ItemSwitcher switcher;
    private HFPS_GameManager gameManager;
    private Inventory inventory;
    private Animation AnimationComp;
    private Image BatterySprite;
    private GameObject BatterySpriteUI;

    [Header("Setup")]
    public BatterySpritesClass BatterySprites = new BatterySpritesClass();
    public int FlashlightInventoryID;
    public AudioSource audioS;

    [Header("Flashlight Settings")]
    public Light LightObject;
    public AudioClip ClickSound;
    public float batteryLifeInSec = 300f;
    public float batteryPercentage = 100;
    public float canReloadPercent;

    [Header("Animation")]
    public GameObject FlashlightGO;
    public string DrawAnim;
    [Range(0, 5)] public float DrawSpeed = 1f;
    public string HideAnim;
    [Range(0, 5)] public float HideSpeed = 1f;
    public string ReloadAnim;
    [Range(0, 5)] public float ReloadSpeed = 1f;
    public string IdleAnim;

    private KeyCode UseItemKey;

    [HideInInspector]
    public bool CanReload;

    private bool isOn;
    private bool isSelected;
    private bool isReloading;
    private bool isPressed;

    private float defaultLightPercentagle;
    private int switcherID;

    void Awake()
    {
        AnimationComp = FlashlightGO.GetComponent<Animation>();
        scriptManager = transform.root.GetComponentInChildren<ScriptManager>();
        switcher = transform.root.GetComponentInChildren<ItemSwitcher>();

        FlashlightGO.SetActive(false);
    }

    void Start()
    {    
        inputManager = scriptManager.GetScript<InputController>();
        gameManager = scriptManager.GetScript<HFPS_GameManager>();
        inventory = scriptManager.GetScript<Inventory>();
        switcherID = switcher.GetIDByObject(gameObject);

        BatterySpriteUI = gameManager.BatteryRemaining;
        BatterySprite = BatterySpriteUI.GetComponent<Image>();
        UIFader.SetColor(BatterySprite.color);

        AnimationComp[DrawAnim].speed = DrawSpeed;
        AnimationComp[HideAnim].speed = HideSpeed;
        AnimationComp[ReloadAnim].speed = ReloadSpeed;

        defaultLightPercentagle = batteryPercentage;
    }

    public void Reload()
    {
        if (FlashlightGO.activeSelf)
        {
            if (batteryPercentage < canReloadPercent)
            {
                StartCoroutine(ReloadCorountine());
                isReloading = true;
            }
        }
    }

    IEnumerator ReloadCorountine()
    {
        AnimationComp.Play(ReloadAnim);

        isOn = false;
        if (ClickSound)
        {
            audioS.clip = ClickSound;
            audioS.Play();
        }

        yield return new WaitUntil(() => !AnimationComp.isPlaying);

        batteryPercentage = defaultLightPercentagle;
        isReloading = false;
    }

    public void Select()
    {
        StartCoroutine(UIFader.FadeIn(2));
        FlashlightGO.SetActive(true);
        AnimationComp.Play(DrawAnim);
        isSelected = true;
    }

    public void Deselect()
    {
        if (FlashlightGO.activeSelf && !isReloading)
        {
            StartCoroutine(UIFader.FadeOut(2));
            StartCoroutine(DeselectCorountine());
        }
    }

    public void Disable()
    {
        isOn = false;
        isSelected = false;
        FlashlightGO.SetActive(false);   
    }

    IEnumerator DeselectCorountine()
    {
        AnimationComp.Play(HideAnim);

        isOn = false;
        if (ClickSound)
        {
            audioS.clip = ClickSound;
            audioS.Play();
        }

        yield return new WaitUntil(() => !AnimationComp.isPlaying);

        isSelected = false;
        FlashlightGO.SetActive(false);
    }

    public void Event_FlashlightOn()
    {
        isOn = !isOn;
        if (ClickSound)
        {
            audioS.clip = ClickSound;
            audioS.Play();
        }
    }

    public void LoaderSetItemEnabled()
    {
        FlashlightGO.SetActive(true);
        AnimationComp.Play(IdleAnim);
        isSelected = true;
        isOn = true;
    }

    void Update()
    {
        if (inputManager.HasInputs())
        {
            UseItemKey = inputManager.GetInput("Flashlight");
        }

        CanReload = batteryPercentage < canReloadPercent;

        if (inventory.CheckItemIDInventory(FlashlightInventoryID) && !isReloading && switcher.currentLightObject == switcherID)
        {
            if (Input.GetKeyDown(UseItemKey) && !AnimationComp.isPlaying && !isPressed)
            {
                if (!isSelected && switcher.currentItem != switcherID)
                {
                    switcher.selectItem(switcherID);
                }
                else
                {
                    Deselect();
                }

                isPressed = true;
            }
            else if (isPressed)
            {
                isPressed = false;
            }
        }

        if (isSelected)
        {
            BatterySprite.color = UIFader.GetUIColor();
        }

        if (isOn)
        {
            LightObject.enabled = true;
            batteryPercentage -= Time.deltaTime * (100 / batteryLifeInSec);
        }
        else
        {
            LightObject.enabled = false;
        }

        batteryPercentage = Mathf.Clamp(batteryPercentage, 0, 100);

        if (batteryPercentage > 95.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_100;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 1, Time.deltaTime);
        }
        else if (batteryPercentage <= 95.0f && batteryPercentage > 90.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_95;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.95f, Time.deltaTime);
        }
        else if (batteryPercentage <= 90.0f && batteryPercentage > 85.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_90;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.9f, Time.deltaTime);
        }
        else if (batteryPercentage <= 85.0f && batteryPercentage > 80.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_85;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.85f, Time.deltaTime);
        }
        else if (batteryPercentage <= 80.0f && batteryPercentage > 75.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_80;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.8f, Time.deltaTime);
        }
        else if (batteryPercentage <= 75.0f && batteryPercentage > 70.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_75;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.75f, Time.deltaTime);
        }
        else if (batteryPercentage <= 70.0f && batteryPercentage > 65.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_70;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.7f, Time.deltaTime);
        }
        else if (batteryPercentage <= 65.0f && batteryPercentage > 60.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_65;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.65f, Time.deltaTime);
        }
        else if (batteryPercentage <= 60.0f && batteryPercentage > 55.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_60;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.6f, Time.deltaTime);
        }
        else if (batteryPercentage <= 55.0f && batteryPercentage > 50.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_55;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.55f, Time.deltaTime);
        }
        else if (batteryPercentage <= 50.0f && batteryPercentage > 45.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_50;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.5f, Time.deltaTime);
        }
        else if (batteryPercentage <= 45.0f && batteryPercentage > 40.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_45;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.45f, Time.deltaTime);
        }
        else if (batteryPercentage <= 40.0f && batteryPercentage > 35.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_40;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.4f, Time.deltaTime);
        }
        else if (batteryPercentage <= 35.0f && batteryPercentage > 30.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_35;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.35f, Time.deltaTime);
        }
        else if (batteryPercentage <= 30.0f && batteryPercentage > 25.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_30;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.3f, Time.deltaTime);
        }
        else if (batteryPercentage <= 25.0f && batteryPercentage > 20.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_25;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.25f, Time.deltaTime);
        }
        else if (batteryPercentage <= 20.0f && batteryPercentage > 15.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_20;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.2f, Time.deltaTime);
        }
        else if (batteryPercentage <= 15.0f && batteryPercentage > 10.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_15;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.15f, Time.deltaTime);
        }
        else if (batteryPercentage <= 10.0f && batteryPercentage > 5.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_10;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.1f, Time.deltaTime);
        }
        else if (batteryPercentage <= 5.0f && batteryPercentage > 1.0f)
        {
            BatterySprite.sprite = BatterySprites.Battery_5;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0.05f, Time.deltaTime);
        }
        else if (batteryPercentage <= 1.0f)
        {
            BatterySprite.color = Color.red;
            BatterySprite.sprite = BatterySprites.Battery_0_red;
            LightObject.intensity = Mathf.Lerp(LightObject.intensity, 0, Time.deltaTime * 2);
        }
    }

    public void OnSave()
    {
        if (GetComponent<SaveHelper>())
        {
            GetComponent<SaveHelper>().SetArray(new Dictionary<string, object> {
                {"batteryPercentage", batteryPercentage}
            });
        }
    }

    public void OnLoad(Newtonsoft.Json.Linq.JToken token)
    {
        batteryPercentage = (float)token["batteryPercentage"];
    }
}