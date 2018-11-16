using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleItem : MonoBehaviour {

	[Header("Setup")]
	public ScriptManager scriptManager;
	private InputController inputManager;
    private Inventory inventory;
    private ItemSwitcher switcher;

	[Header("Candle Others")]
	public int InventoyID;
	public AudioClip BlowOut;
    public GameObject Candle;
    public GameObject CandleFlame;
	public GameObject CandleLight;
    public Transform FlamePosition;

    [Header("Candle Inventory")]
    public int InventoryID;
    public bool blowOutKeepCandle;
    public float scaleKeepCandle;

    [Header("Candle Animations")]
	public GameObject CandleGO;
	public string DrawAnimation;
	public string HideAnimation;
    public string BlowOutAnimation;
    public string IdleAnimation;

	public float DrawSpeed = 1f;
	public float HideSpeed = 1f;

    [Header("Candle Settings")]
    public bool candleReduction;
    public float reductionRate;
    public float minScale;

    private KeyCode BlowOutKey;

    private bool isSelected;
    private bool IsPressed;

	void Start () {
		inputManager = scriptManager.GetScript<InputController>();
        inventory = scriptManager.GetScript<Inventory>();
        switcher = transform.parent.parent.GetComponent<ItemSwitcher>();

        if (isSelected)
        {
            Select();
        }
    }

	public void Select() {
		isSelected = true;
		CandleGO.SetActive (true);
		CandleGO.GetComponent<Animation>().Play(DrawAnimation);
        CandleFlame.SetActive (true);
		CandleLight.SetActive (true);

        if (switcher)
        {
            switcher.currentLightObject = switcher.ItemList.IndexOf(gameObject);
        }

        if (candleReduction)
        {
            StartCoroutine(Scale());
        }
	}

	public void Deselect()
	{
		if (CandleGO.activeSelf) {
            if (switcher)
            {
                switcher.currentLightObject = -1;
            }

            StopAllCoroutines();
            StartCoroutine(BlowOutHide());
			IsPressed = true;
		}
	}

    public void DeselectHide()
    {
        if (CandleGO.activeSelf)
        {
            if (switcher)
            {
                switcher.currentLightObject = -1;
            }

            StopAllCoroutines();
            CandleGO.GetComponent<Animation>().Play(HideAnimation);
            IsPressed = true;
        }
    }

    IEnumerator BlowOutHide()
    {
        CandleGO.GetComponent<Animation>().Play(BlowOutAnimation);

        yield return new WaitUntil(() => !CandleGO.GetComponent<Animation>().isPlaying);

        IsPressed = true;
    }

    public void LoaderSetItemEnabled()
    {
        isSelected = true;
        CandleGO.SetActive(true);
        CandleGO.GetComponent<Animation>().Play(IdleAnimation);
        CandleFlame.SetActive(true);
        CandleLight.SetActive(true);

        if (candleReduction)
        {
            StartCoroutine(Scale());
        }
    }

    public void BlowOut_Event()
	{
		AudioSource.PlayClipAtPoint (BlowOut, Camera.main.transform.position, 0.35f);
		CandleFlame.SetActive (false);
		CandleLight.SetActive (false);

        if(blowOutKeepCandle && Candle.transform.localScale.y > scaleKeepCandle)
        {
            if (inventory.CheckInventorySpace())
            {
                inventory.AddItem(InventoryID, 1);
            }
        }
	}

	void Update () {
		if(inputManager.HasInputs())
		{
			BlowOutKey = inputManager.GetInput("Flashlight");
		}

        CandleFlame.transform.position = FlamePosition.position;

        if (Input.GetKeyDown (BlowOutKey) && !IsPressed && isSelected && !CandleGO.GetComponent<Animation> ().isPlaying) {
			CandleGO.GetComponent<Animation>().Play(BlowOutAnimation);
            StopAllCoroutines();
			IsPressed = true;
		}

		if (IsPressed && !(CandleGO.GetComponent<Animation> ().isPlaying)) {
			CandleGO.SetActive (false);
			IsPressed = false;
		}
	}

    IEnumerator Scale()
    {
        while (minScale <= Candle.transform.localScale.y)
        {
            Vector3 temp = Candle.transform.localScale;
            temp.y -= temp.y * Time.deltaTime * reductionRate;
            Candle.transform.localScale = temp;
            yield return null;
        }

        FlameBurnOut();

        yield return new WaitForSeconds(1f);

        CandleGO.GetComponent<Animation>().Play(HideAnimation);

        yield return new WaitUntil(() => !CandleGO.GetComponent<Animation>().isPlaying);

        IsPressed = true;
    }

    void FlameBurnOut()
    {
        CandleFlame.SetActive(false);
        CandleLight.SetActive(false);
    }

    public void OnSave()
    {
        if (GetComponent<SaveHelper>())
        {
            GetComponent<SaveHelper>().SetArray(new Dictionary<string, object> {
                {"candleScale", Candle.transform.localScale.y}
            });
        }
    }

    public void OnLoad(Newtonsoft.Json.Linq.JToken token)
    {
        Vector3 scale = Candle.transform.localScale;
        scale.y = (float)token["candleScale"];
        Candle.transform.localScale = scale;
    }
}
