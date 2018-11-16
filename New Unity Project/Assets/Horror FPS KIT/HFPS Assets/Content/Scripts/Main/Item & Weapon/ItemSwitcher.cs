/* ItemSwitcher.cs by ThunderWire Games - Script for Switching Items */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSwitcher : MonoBehaviour {

    private Inventory inventory;
    private HFPS_GameManager gameManager;

	public List<GameObject> ItemList = new List<GameObject>();
    public int currentItem = -1;
    public bool selectCurrItem;

    [Header("Wall Detecting")]
    public bool detectWall;
    public LayerMask HitMask;
    public float wallHitRange;

    public Animation WallDetectAnim;
    public string HideAnim;
    public string ShowAnim;

    [Header("Misc")]
    [Tooltip("ID must be always light object which you currently using!")]
    public int currentLightObject = 0;

    [HideInInspector]
    public int weaponItem = -1;

    private bool hit;
    private bool ladder;
    private bool switchItem;
    private bool isPressed;

	private int newItem = 0;
    private bool antiSpam;
    private bool spam;

    void Start()
    {
        if (selectCurrItem)
        {
            selectItem(currentItem);
        }

        inventory = transform.root.GetComponentInChildren<ScriptManager>().GetScript<Inventory>();
        gameManager = transform.root.GetChild(0).GetChild(0).GetComponent<ScriptManager>().GetScript<HFPS_GameManager>();
    }

    public void selectItem(int id)
    {
        newItem = id;

        if (!CheckActiveItem())
        {
            SelectItem();
        }
        else
        {
            StartCoroutine(SwitchItem());
        }
    }

    public void DeselectItems()
	{
        if (currentItem == -1) return;
        ItemList [currentItem].SendMessage ("Deselect", SendMessageOptions.DontRequireReceiver);
    }

    public void DisableItems()
    {
        if (currentItem == -1) return;
        ItemList[currentItem].SendMessage("Disable", SendMessageOptions.DontRequireReceiver);
    }

    public int GetIDByObject(GameObject switcherObject)
    {
        return ItemList.IndexOf(switcherObject);
    }

    /// <summary>
    /// Check if all Items are Deactivated
    /// </summary>
	bool CheckActiveItem()
	{
		for (int i = 0; i < ItemList.Count; i++) {
            bool ACState = ItemList[i].transform.GetChild(0).gameObject.activeSelf;
			if (ACState)
				return true;
		}
		return false;
	}

	IEnumerator SwitchItem()
	{
        switchItem = true;
        ItemList [currentItem].SendMessage ("Deselect", SendMessageOptions.DontRequireReceiver);

		yield return new WaitUntil (() => ItemList[currentItem].transform.GetChild(0).gameObject.activeSelf == false);

		ItemList [newItem].SendMessage ("Select", SendMessageOptions.DontRequireReceiver);
		currentItem = newItem;
        switchItem = false;
    }

	void SelectItem()
	{
        switchItem = true;
        ItemList [newItem].SendMessage ("Select", SendMessageOptions.DontRequireReceiver);
        currentItem = newItem;
        switchItem = false;
    }

    void Update()
    {
        if (WallDetectAnim && detectWall && !ladder && currentItem != -1)
        {
            if (WallHit())
            {
                if (!hit)
                {
                    WallDetectAnim.Play(HideAnim);
                    ItemList[currentItem].SendMessage("WallHit", true, SendMessageOptions.DontRequireReceiver);
                    hit = true;
                }
            }
            else
            {
                if (hit)
                {
                    WallDetectAnim.Play(ShowAnim);
                    ItemList[currentItem].SendMessage("WallHit", false, SendMessageOptions.DontRequireReceiver);
                    hit = false;
                }
            }
        }

        if (!gameManager.isGrabbed)
        {
            if (!antiSpam)
            {
                //Mouse ScrollWheel Backward - Deselect Current Item
                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    if (currentItem != -1)
                    {
                        DeselectItems();
                    }
                }

                //Mouse ScrollWheel Forward - Select Last Weapon Item
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    if (weaponItem != -1)
                    {
                        MouseWHSelectWeapon();
                    }
                }
            }
            else
            {
                if (!spam)
                {
                    StartCoroutine(AntiSwitchSpam());
                    spam = true;
                }
            }
        }
        else
        {
            antiSpam = true;
        }
    }

    void MouseWHSelectWeapon()
    {
        if (currentItem != weaponItem)
        {
            if (ItemList[weaponItem].GetComponent<WeaponController>() && inventory.CheckSWIDInventory(weaponItem))
            {
                selectItem(weaponItem);
            }
        }
    }

    IEnumerator AntiSwitchSpam()
    {
        antiSpam = true;
        yield return new WaitForSeconds(1f);
        antiSpam = false;
        spam = false;
    }

    void FixedUpdate()
    {
        if (!CheckActiveItem() && !switchItem)
        {
            currentItem = -1;
        }

        if (!inventory.CheckSWIDInventory(weaponItem))
        {
            weaponItem = -1;
        }
    }

    bool GetItemsActive()
    {
        bool response = true;
        for (int i = 0; i < ItemList.Count; i++)
        {
            if (ItemList[i].transform.GetChild(0).gameObject.activeSelf)
            {
                response = false;
                break;
            }
        }
        return response;
    }

    public void SetActiveItem(int switchID)
    {
        switchItem = true;
        ItemList[switchID].SendMessage("LoaderSetItemEnabled", SendMessageOptions.DontRequireReceiver);
        currentItem = switchID;
        newItem = switchID;
        switchItem = false;
    }

    public void Ladder(bool onLadder)
    {
        if (currentItem != -1)
        {
            if (onLadder && !ladder)
            {
                WallDetectAnim.Play(HideAnim);
                ItemList[currentItem].SendMessage("WallHit", true, SendMessageOptions.DontRequireReceiver);
                ladder = true;
            }
            if (!onLadder && ladder)
            {
                WallDetectAnim.Play(ShowAnim);
                ItemList[currentItem].SendMessage("WallHit", false, SendMessageOptions.DontRequireReceiver);
                ladder = false;
            }
        }
    }

    bool WallHit()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, wallHitRange, HitMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (detectWall)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward * wallHitRange));
        }
    }
}