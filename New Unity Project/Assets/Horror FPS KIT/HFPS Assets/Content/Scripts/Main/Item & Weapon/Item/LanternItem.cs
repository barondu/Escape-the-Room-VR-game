using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternItem : MonoBehaviour {

    private ScriptManager scriptManager;
    private InputController inputControl;
    private ItemSwitcher switcher;
    private Inventory Inventory;
    private Animation anim;
    private AudioSource audioS;
    private Image OilSprite;

    [Header("Main")]
    public Light LanternLight;
    public string spritePrefix = "Oil_";
    [Space(7)]
    public float oilLifeInSec = 300f;
    public float oilPercentage = 100;
    public float lightReductionRate = 5f;
    public float canReloadPercent;
    public float hideIntensitySpeed;
    public float oilReloadSpeed;
    public float timeWaitToReload;

    [Header("Inventory")]
    public int lanternInventoryID;
    private int switcherID;

    [Header("Animation")]
    public GameObject LanternGO;
    public string DrawAnim;
    [Range(0, 5)] public float DrawSpeed = 1f;
    public string HideAnim;
    [Range(0, 5)] public float HideSpeed = 1f;
    public string ReloadAnim;
    [Range(0, 5)] public float ReloadSpeed = 1f;
    public string IdleAnim;

    [Header("Sounds")]
    public AudioClip ShowSound;
    [Range(0, 1)] public float ShowVolume;
    public AudioClip HideSound;
    [Range(0, 1)] public float HideVolume;
    public AudioClip ReloadOilSound;
    [Range(0, 1)] public float ReloadVolume;

    private KeyCode UseItemKey;

    private bool showSprite;
    private bool isSelected;
    private bool isSelecting;
    private bool isReloading;
    private bool isPressed;

    private float reductionFactor;
    private float reduceIntensity;
    private float oldIntensity;

    private float fullIntnesity;
    private float defaultOilPercentagle;
    private string currentSprite;

    private Color FlameTint;

    [HideInInspector]
    public bool CanReload;

    void Awake()
    {
        anim = LanternGO.GetComponent<Animation>();
        scriptManager = transform.root.GetComponentInChildren<ScriptManager>();
        switcher = transform.root.GetComponentInChildren<ItemSwitcher>();

        if (LanternGO.GetComponent<AudioSource>())
        {
            audioS = LanternGO.GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        inputControl = scriptManager.GetScript<InputController>();
        Inventory = scriptManager.GetScript<Inventory>();
        OilSprite = scriptManager.GetScript<HFPS_GameManager>().OilRemaining.GetComponent<Image>();
        switcherID = switcher.GetIDByObject(gameObject);

        UIFader.SetColor(OilSprite.color);

        anim[DrawAnim].speed = DrawSpeed;
        anim[HideAnim].speed = HideSpeed;
        anim[ReloadAnim].speed = ReloadSpeed;

        defaultOilPercentagle = oilPercentage;
        fullIntnesity = LanternLight.intensity;
        oldIntensity = LanternLight.intensity;
        reduceIntensity = LanternLight.intensity;
        FlameTint = LanternLight.transform.GetChild(0).GetComponent<MeshRenderer>().material.GetColor("_TintColor");
        FlameTint.a = 0f;
        LanternLight.intensity = 0f;
        currentSprite = spritePrefix + oilPercentage;
        reductionFactor = oilPercentage - lightReductionRate;
    }

    public void Reload()
    {
        if (LanternGO.activeSelf)
        {
            if (oilPercentage < canReloadPercent && !isReloading)
            {
                StartCoroutine(ReloadCorountine());
                isReloading = true;
            }
        }
    }

    IEnumerator ReloadCorountine()
    {
        anim.Play(ReloadAnim);

        yield return new WaitForSeconds(timeWaitToReload);

        if (audioS && ReloadOilSound)
        {
            audioS.clip = ReloadOilSound;
            audioS.volume = ReloadVolume;
            audioS.Play();
        }

        while (LanternLight.intensity <= fullIntnesity)
        {
            LanternLight.intensity += Time.deltaTime * oilReloadSpeed;
            yield return null;
        }

        oilPercentage = defaultOilPercentagle;
        reductionFactor = oilPercentage - lightReductionRate;
        LanternLight.intensity = fullIntnesity;
        reduceIntensity = fullIntnesity;
        FlameTint.a = fullIntnesity;

	    int spriteInt = Mathf.RoundToInt(reductionFactor + lightReductionRate);
	    currentSprite = spritePrefix + spriteInt;

        isReloading = false;
    }

    public void Select()
    {
        showSprite = true;
        isSelecting = true;
        LanternGO.SetActive(true);
        LanternLight.gameObject.SetActive(true);

        anim.Play(DrawAnim);

        if (audioS && ShowSound)
        {
            audioS.clip = ShowSound;
            audioS.volume = ShowVolume;
            audioS.Play();
        }

        StartCoroutine(UIFader.FadeIn(2));
        StartCoroutine(SelectCoroutine());
    }

    IEnumerator SelectCoroutine()
    {
        while (LanternLight.intensity <= oldIntensity)
        {
            LanternLight.intensity += Time.deltaTime * hideIntensitySpeed;
            FlameTint.a += Time.deltaTime * hideIntensitySpeed;
            yield return null;
        }

        FlameTint.a = oldIntensity;
        LanternLight.intensity = oldIntensity;
        isSelected = true;
    }

    public void Deselect()
    {
        isSelecting = false;
        oldIntensity = LanternLight.intensity;

        if (audioS && HideSound)
        {
            audioS.clip = HideSound;
            audioS.volume = HideVolume;
            audioS.Play();
        }

        if (LanternGO.activeSelf)
        {
            StartCoroutine(UIFader.FadeOut(2));
            StartCoroutine(DeselectCoroutine());
        }
    }

    IEnumerator DeselectCoroutine()
    {
        anim.Play(HideAnim);

        while (LanternLight.intensity >= 0.01f)
        {
            LanternLight.intensity -= Time.deltaTime * hideIntensitySpeed;
            FlameTint.a -= Time.deltaTime * hideIntensitySpeed;
            yield return null;
        }

        LanternLight.intensity = 0f;

        yield return new WaitUntil(() => !anim.isPlaying);

        isSelected = false;
        showSprite = false;
    }

    public void Disable()
    {
        LanternLight.intensity = 0f;
        isSelecting = false;
        isSelected = false;
        showSprite = false;
    }

    public void LoaderSetItemEnabled()
    {
        LanternGO.SetActive(true);
        anim.Play(IdleAnim);
        isSelected = true;
    }

    void Update()
    {
        if (inputControl.HasInputs())
        {
            UseItemKey = inputControl.GetInput("Flashlight");
        }

        CanReload = oilPercentage < canReloadPercent;

        if (Inventory.CheckItemIDInventory(lanternInventoryID) && switcher.currentLightObject == switcherID)
        {
            if (Input.GetKeyDown(UseItemKey) && !anim.isPlaying && !isPressed)
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
            if (oilPercentage > 0)
            {
                oilPercentage -= Time.deltaTime * (100 / oilLifeInSec);

                if (oilPercentage <= reductionFactor)
                {
                    reduceIntensity -= lightReductionRate / 100;
                    reductionFactor -= lightReductionRate;
                    StartCoroutine(Reduce());

                    int spriteInt = Mathf.RoundToInt(reductionFactor + lightReductionRate);
                    currentSprite = spritePrefix + spriteInt;
                }
            }

            OilSprite.sprite = Resources.Load<Sprite>("Icons/OilPercentagle/" + currentSprite);
        }
        else
        {
            if (!isSelecting)
            {
                LanternLight.gameObject.SetActive(false);
                LanternGO.SetActive(false);
            }
        }

        if (showSprite)
        {
            OilSprite.color = UIFader.GetUIColor();
        }

        oilPercentage = Mathf.Clamp(oilPercentage, 0, 100);
        reductionFactor = Mathf.Clamp(reductionFactor, 0, 100);
        LanternLight.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_TintColor", FlameTint);
    }

    IEnumerator Reduce()
    {
        while (LanternLight.intensity >= reduceIntensity)
        {
            LanternLight.intensity -= Time.deltaTime * 0.15f;
            FlameTint.a -= Time.deltaTime * 0.15f;
            yield return null;
        }

        LanternLight.intensity = (float)System.Math.Round(reduceIntensity, 2);
    }

    public void OnSave()
    {
        if (GetComponent<SaveHelper>())
        {
            GetComponent<SaveHelper>().SetArray(new Dictionary<string, object> {
                {"lightPercentage", oilPercentage},
                {"lightIntensity", oldIntensity},
                {"flameAlpha", FlameTint.a}
            });
        }
    }

    public void OnLoad(Newtonsoft.Json.Linq.JToken token)
    {
        oilPercentage = (float)token["lightPercentage"];
        oldIntensity = (float)token["lightIntensity"];
        FlameTint.a = (float)token["flameAlpha"];
        reductionFactor = oilPercentage - lightReductionRate;
    }
}
