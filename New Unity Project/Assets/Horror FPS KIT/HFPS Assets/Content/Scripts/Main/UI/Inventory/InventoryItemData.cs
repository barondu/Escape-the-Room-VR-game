using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public Item item;
	public string itemTitle;

    public int m_amount;

    public int slotID;

	[HideInInspector]
	public bool selected;

	[HideInInspector]
	public bool isDisabled;

	private Text textAmount;
	private Inventory inventory;
    private Vector2 offset;

    void Awake()
    {
        m_amount = 0;
        transform.GetChild(0).GetComponent<Text>().text = "0";
    }

	void Start()
	{
		inventory = transform.root.GetComponent<HFPS_GameManager> ().inventoryScript;
		transform.position = transform.parent.position;
	}

	void Update()
	{
		textAmount = transform.GetChild (0).gameObject.GetComponent<Text> ();
		if (item.itemType == typeEnum.Bullets || item.itemType == typeEnum.Weapon) {
			textAmount.text = m_amount.ToString ();
		} else {
			if (m_amount > 1) {
				textAmount.text = m_amount.ToString ();
			} else if (m_amount == 1) {
				textAmount.text = "";
			}
		}

		itemTitle = item.Title;
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
		if (item != null && !isDisabled)
        {
            offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
            transform.SetParent(transform.parent.parent);
            transform.position = eventData.position - offset;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
		if (item != null && !isDisabled)
        {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
		if (!isDisabled) {
			transform.SetParent (inventory.slots [slotID].transform);
			transform.position = inventory.slots [slotID].transform.position;
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
		}
    }

	void OnDisable()
	{
		if(selected)
		inventory.Deselect (slotID);
	}
}
