using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler {
	public int id;

    [HideInInspector]
    public Inventory inventory;

    [HideInInspector]
    public InventoryItemData itemData; 

    [HideInInspector]
	public bool isCombining;

	[HideInInspector]
	public bool isCombinable;

    [HideInInspector]
    public bool isSelected;

    void Update()
    {
        if (!inventory) return;

        if (transform.childCount > 1)
        {
            itemData = transform.GetChild(1).GetComponent<InventoryItemData>();
            transform.GetChild(0).GetComponent<Image>().sprite = inventory.itemSlotSprite;
            GetComponent<Image>().enabled = true;

            if (inventory.itemSlotMask)
            {
                GetComponent<Image>().sprite = inventory.itemSlotMask;
            }

            if (itemData.selected)
            {
                GetComponent<Image>().color = inventory.slotSelected;
            }
            else if (!isCombining)
            {
                GetComponent<Image>().color = inventory.slotNormal;
            }

            if (isCombining)
            {
                itemData.isDisabled = true;
            }
            else
            {
                itemData.isDisabled = false; ;
            }
        }
        else if (transform.childCount < 2)
        {
            itemData = null;
            GetComponent<Image>().enabled = false;
            transform.GetChild(0).GetComponent<Image>().sprite = inventory.slotsSprite;
            transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }

        if (isCombining)
        {
            inventory.CombineButton.interactable = false;
        }
    }

	public void OnDrop (PointerEventData eventData)
	{
		InventoryItemData itemDrop = eventData.pointerDrag.GetComponent<InventoryItemData> ();
        itemData = itemDrop;

		if (!isCombining) {
			if (inventory.slots[id].transform.childCount < 2) {
				itemDrop.slotID = id;
			} else if(itemDrop.slotID != id) {
				Transform item = transform.GetChild (1);
				item.GetComponent<InventoryItemData> ().slotID = itemDrop.slotID;
				item.transform.SetParent (inventory.slots [itemDrop.slotID].transform);
				item.transform.position = inventory.slots [itemDrop.slotID].transform.position;

				itemDrop.slotID = id;
				itemDrop.transform.SetParent (transform);
				itemDrop.transform.position = transform.position;
			}
			if (itemDrop.selected) {
				inventory.selectedID = itemDrop.slotID;
			}
		}
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.childCount > 1)
        {
            if (isCombinable)
            {
                StopAllCoroutines();
                inventory.CombineWith(itemData.item, id);
                isSelected = false;
            }
            else if(!isCombining)
            {
                for (int i = 0; i < inventory.slots.Count; i++)
                {
                    inventory.slots[i].GetComponent<Image>().color = inventory.slotNormal;
                    if (inventory.slots[i].transform.childCount > 1)
                    {
                        inventory.slots[i].transform.GetChild(1).GetComponent<InventoryItemData>().selected = false;
                        inventory.slots[i].GetComponent<InventorySlot>().isSelected = false;
                    }
                }

                GetComponent<Image>().color = inventory.slotSelected;
                inventory.ItemLabel.text = itemData.item.Title;
                inventory.ItemDescription.text = itemData.item.Description;
                inventory.selectedID = itemData.slotID;
                itemData.selected = true;

                inventory.UseButton.interactable = false;
                inventory.DropButton.interactable = false;
                inventory.CombineButton.interactable = false;

                isSelected = true;
                StartCoroutine(PressedUpdate());
            }
        }
    }

    IEnumerator PressedUpdate()
    {
        while (isSelected)
        {
            if (transform.childCount > 1)
            {
                InventoryScriptable.ItemMapper.CombineSettings[] combineSettings = itemData.item.combineSettings;

                if (!isCombining)
                {
                    for (int i = 0; i < combineSettings.Length; i++)
                    {
                        if (inventory.CheckItemIDInventory(itemData.item.combineSettings[i].combineWithID))
                        {
                            if (itemData.item.isCombinable)
                            {
                                if (itemData.item.itemType == typeEnum.ItemPart)
                                {
                                    if (inventory.GetItem(itemData.item.combineSettings[0].combineWithID).useSwitcherID == inventory.switcher.currentItem)
                                    {
                                        inventory.CombineButton.interactable = true;
                                    }
                                    else
                                    {
                                        inventory.CombineButton.interactable = false;
                                    }
                                }
                                else
                                {
                                    inventory.CombineButton.interactable = true;
                                }
                            }
                            else
                            {
                                inventory.CombineButton.interactable = false;
                            }
                        }
                    }

                    if (itemData.item.isUsable)
                    {
                        if (itemData.item.useItemSwitcher)
                        {
                            if (inventory.selectedSwitcherID != itemData.item.useSwitcherID)
                            {
                                inventory.UseButton.interactable = true;
                            }
                            else
                            {
                                inventory.UseButton.interactable = false;
                            }
                        }
                        else
                        {
                            inventory.UseButton.interactable = true;
                        }
                    }
                    else
                    {
                        inventory.UseButton.interactable = false;
                    }

                    inventory.DropButton.interactable = itemData.item.isDroppable;
                }
                else
                {
                    isSelected = false;
                }
            }
            else
            {
                isSelected = false;
            }

            yield return null;
        }
    }
}
