/*
 * Inventory.cs - script by ThunderWire Games
 * ver. 1.3
*/

using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using ThunderWire.Helper.Parser;

/// <summary>
/// Main Inventory Script
/// </summary>
public class Inventory : MonoBehaviour, IPointerClickHandler {

    public List<Item> ItemsData = new List<Item>();
    public List<Item> ItemsCache = new List<Item>();
    [HideInInspector] public List<GameObject> slots = new List<GameObject>();

    private HFPS_GameManager gameManager;
    [HideInInspector]
	public ItemSwitcher switcher;

    [Tooltip("Database of all inventory items.")]
    public InventoryScriptable inventoryDatabase;

    [Space(5)]
    [Header("Main")]
	public GameObject SlotsPanel;

	public Button UseButton;
    public Button DropButton;
    public Button CombineButton;

	public Text ItemLabel;
	public Text ItemDescription;

	[Header("Inventory Prefabs")]
	public GameObject inventorySlot;
	public GameObject inventoryItem;

    [Header("Slot Settings")]
    public Sprite slotsSprite;
    public Sprite itemSlotSprite;
    public Sprite itemSlotMask;

    [Header("Inventory Items")]
	public int slotAmout;
	public int maxSlots = 16;

	[HideInInspector]
	public int selectedID;

    [HideInInspector]
    public int selectedSwitcherID = -1;

    [Header("Inventory Settings")]
    public int itemDropStrenght = 10;
	public Color slotNormal = Color.white;
	public Color slotSelected = Color.white;
	public Color slotDisabled = Color.white;


    void Awake()
    {
        if (!inventoryDatabase) { Debug.LogError("Inventory Database does not set!"); return; }

        for (int i = 0; i < inventoryDatabase.ItemDatabase.Count; i++)
        {
            ItemsData.Add(new Item(i, inventoryDatabase.ItemDatabase[i]));
        }
    }

	void Start () {
        gameManager = GetComponent<HFPS_GameManager>();

		for (int i = 0; i < slotAmout; i++) {
			GameObject slot = Instantiate(inventorySlot);
            slots.Add(slot);
            slot.GetComponent<InventorySlot>().inventory = this;
            slot.GetComponent<InventorySlot> ().id = i;
            slot.transform.SetParent (SlotsPanel.transform);
		}

		ItemLabel.text = "";
		ItemDescription.text = "";

        selectedID = -1;
	}

	void Update()
	{
		if (!switcher) {
			switcher = gameManager.scriptManager.GetScript<ItemSwitcher>();
        }
        else
        {
            selectedSwitcherID = switcher.currentItem;
        }

		if (!gameManager.TabButtonPanel.activeSelf) {
            UseButton.interactable = false;
            UseButton.transform.GetChild(0).GetComponent<MenuEvents>().ChangeTextColor("Black");
            DropButton.interactable = false;
            DropButton.transform.GetChild(0).GetComponent<MenuEvents>().ChangeTextColor("Black");
            CombineButton.interactable = false;
            CombineButton.transform.GetChild(0).GetComponent<MenuEvents>().ChangeTextColor("Black");

            for (int i = 0; i < slots.Count; i++) {
				slots [i].GetComponent<InventorySlot> ().isCombining = false;
				slots [i].GetComponent<InventorySlot> ().isCombinable = false;
			}
		}
	}

    /// <summary>
    /// Get Item from item database by ItemID
    /// </summary>
    public Item GetItem(int itemID)
    {
        for (int i = 0; i < ItemsData.Count; i++)
            if (ItemsData[i].ID == itemID)
                return ItemsData[i];
        return null;
    }

    /// <summary>
    /// Function to add new item to specific slot
    /// </summary>
    public void AddItemSlot(int slotID, int itemID, int amount)
    {
        Item itemToAdd = GetItem(itemID);
        if (CheckInventorySpace() || CheckItemIDInventory(itemID))
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (i == slotID)
                {
                    ItemsCache.Add(itemToAdd);
                    GameObject item = Instantiate(inventoryItem);
                    InventoryItemData itemData = item.GetComponent<InventoryItemData>();
                    itemData.item = itemToAdd;
                    itemData.m_amount = amount;
                    itemData.slotID = i;
                    slots[i].GetComponent<InventorySlot>().itemData = itemData;
                    item.transform.SetParent(slots[i].transform);
                    item.GetComponent<Image>().sprite = itemToAdd.itemSprite;
                    item.GetComponent<RectTransform>().position = Vector2.zero;
                    item.name = itemToAdd.Title;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Function to add new item to slot
    /// </summary>
    public void AddItem(int itemID, int amount)
	{
		Item itemToAdd = GetItem (itemID);
		if (CheckInventorySpace() || CheckItemIDInventory(itemID)) {
			if (itemToAdd.isStackable && CheckItemInventory (itemToAdd) && GetSlotByItem (itemToAdd) != -1) {
				InventoryItemData itemData = slots [GetSlotByItem (itemToAdd)].transform.GetChild (1).GetComponent<InventoryItemData> ();
                itemData.m_amount = itemData.m_amount + amount;
            } else {
				for (int i = 0; i < slots.Count; i++) {
					if (slots [i].transform.childCount == 1) {
                        ItemsCache.Add (itemToAdd);
						GameObject item = Instantiate (inventoryItem);
                        InventoryItemData itemData = item.GetComponent<InventoryItemData>();
                        itemData.item = itemToAdd;
                        itemData.m_amount = amount;
                        itemData.slotID = i;
                        slots[i].GetComponent<InventorySlot>().itemData = itemData;
						item.transform.SetParent (slots [i].transform);
						item.GetComponent<Image> ().sprite = itemToAdd.itemSprite;
						item.GetComponent<RectTransform> ().position = Vector2.zero;
						item.name = itemToAdd.Title;
                        break;
					}
				}
			}
		}
	}

    /// <summary>
    /// Function to remove one item
    /// </summary>
    public void RemoveItem(int id)
    {
        Item itemToRemove = GetItem(id);
        if (itemToRemove.isStackable && CheckItemInventory(itemToRemove))
        {
            InventoryItemData data = slots[GetSlotByItem(itemToRemove)].transform.GetChild(1).GetComponent<InventoryItemData>();
            data.m_amount--;
            data.transform.GetChild(0).GetComponent<Text>().text = data.m_amount.ToString();
            if (data.m_amount == 0)
            {
                Destroy(slots[GetSlotByItem(itemToRemove)].transform.GetChild(1).gameObject);
                ItemsCache.Remove(itemToRemove);
                DeselectSelected();
            }
            if (data.m_amount == 1)
            {
                slots[GetSlotByItem(itemToRemove)].transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }
        else
        {
            Destroy(slots[GetSlotByItem(itemToRemove)].transform.GetChild(1).gameObject);
            ItemsCache.Remove(itemToRemove);
        }
    }

    /// <summary>
    /// Function to remove all item stacks
    /// </summary>
    public void RemoveItemAll(int id)
    {
        Item itemRemove = GetItem(id);

        if (CheckItemInventory(itemRemove))
        {
            Destroy(slots[GetSlotByItem(itemRemove)].transform.GetChild(1).gameObject);
            ItemsCache.Remove(itemRemove);
            DeselectSelected();
        }
    }

    /// <summary>
    /// Function to remove specific item amount
    /// </summary>
    public void RemoveItemAmount(int id, int amount)
    {
        Item itemToRemove = GetItem(id);
        if (CheckItemInventory(itemToRemove))
        {
            InventoryItemData data = slots[GetSlotByItem(itemToRemove)].transform.GetChild(1).GetComponent<InventoryItemData>();

            if(data.m_amount > amount)
            {
                data.m_amount = data.m_amount - amount;
                data.transform.GetChild(0).GetComponent<Text>().text = data.m_amount.ToString();
            }
            else
            {
                RemoveItemAll(id);
            }
        }
    }

    /// <summary>
    /// Function to set specific item amount
    /// </summary>
    public void SetItemAmount(int itemID, int amount)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 1)
            {
                if (slots[i].transform.GetChild(1).GetComponent<InventoryItemData>().item.ID == itemID)
                {
                    slots[i].transform.GetChild(1).GetComponent<InventoryItemData>().m_amount = amount;
                }
            }
        }
    }

    /// <summary>
    /// Function to expand item slots
    /// </summary>
    public void ExpandSlots(int slotsAmount)
    {
        int extendedSlots = slotAmout + slotsAmount;

        if (extendedSlots > maxSlots)
        {
            gameManager.WarningMessage("Cannot carry more backpacks");
            return;
        }

        for (int i = slotAmout; i < extendedSlots; i++)
        {
            GameObject slot = Instantiate(inventorySlot);
            slots.Add(slot);
            slot.GetComponent<InventorySlot>().inventory = this;
            slot.GetComponent<InventorySlot>().id = i;
            slot.transform.SetParent(SlotsPanel.transform);
        }

        slotAmout = extendedSlots;
    }

    /// <summary>
    /// Check if there is space in Inevntory
    /// </summary>
    public bool CheckInventorySpace()
	{
		for (int i = 0; i < slots.Count; i++) {
			if (slots [i].transform.childCount < 2)
				return true;
		}
		return false;
	}

    /// <summary>
    /// Check if Item is in Inventory
    /// </summary>
    bool CheckItemInventory(Item item)
	{
		for (int i = 0; i < ItemsCache.Count; i++) {
			if (ItemsCache[i].ID == item.ID)
				return true;
		}
		return false;
	}

    /// <summary>
    /// Check if Item is in Inventory by ItemID
    /// </summary>
    public bool CheckItemIDInventory(int ItemID)
    {
        for (int i = 0; i < ItemsCache.Count; i++)
        {
            if (ItemsCache[i].ID == ItemID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check if Switcher Item is in Inventory
    /// </summary>
    public bool CheckSWIDInventory(int switcherID)
    {
        for (int i = 0; i < ItemsCache.Count; i++)
        {
            if (ItemsCache[i].useSwitcherID == switcherID)
            {
                return true;
            }
        }

        return false;
    }

    int GetSlotByItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 1 && slots[i].transform.GetChild(1).GetComponent<InventoryItemData>().item == item)
                return i;
        }
        return -1;
    }

    int GetSlotId(int itemID)
	{
		for (int i = 0; i < slots.Count; i++) {
			if (slots [i].transform.childCount > 1)
				if (slots [i].transform.GetChild (1).GetComponent<InventoryItemData> ().item.ID == itemID)
					return i;
		}
		return -1;
	}

    public int GetItemAmount(int itemID)
	{
		for (int i = 0; i < slots.Count; i++) {
			if (slots [i].transform.childCount > 1)
			if (slots [i].transform.GetChild (1).GetComponent<InventoryItemData> ().item.ID == itemID)
				return slots [i].transform.GetChild (1).GetComponent<InventoryItemData> ().m_amount;
		}
		return -1;
	}

    public void UseItem()
    {
        Item usableItem = slots[selectedID].transform.GetChild(1).GetComponent<InventoryItemData>().item;

        if (GetItemAmount(usableItem.ID) < 2 || usableItem.useItemSwitcher)
        {
            UseButton.interactable = false;
            DropButton.interactable = false;
            CombineButton.interactable = false;
            UseButton.transform.GetChild(0).GetComponent<MenuEvents>().ChangeTextColor("Black");

            if (usableItem.useItemSwitcher)
            {
                DeselectSelected();
            }
        }

        if (usableItem.itemType == typeEnum.Heal)
        {
            gameManager.healthManager.ApplyHeal(usableItem.healAmount);
            if (!gameManager.healthManager.isMaximum)
            {
                if (usableItem.useSound)
                {
                    AudioSource.PlayClipAtPoint(usableItem.useSound, Camera.main.transform.position, usableItem.soundVolume);
                }
                RemoveItem(usableItem.ID);
            }
        }

        if (usableItem.itemType == typeEnum.Light)
        {
            switcher.currentLightObject = usableItem.useSwitcherID;
        }

        if (usableItem.itemType == typeEnum.Weapon || usableItem.useItemSwitcher)
        {
            switcher.selectItem(usableItem.useSwitcherID);
            switcher.weaponItem = usableItem.useSwitcherID;
        }
    }

    public void DropItemGround()
    {
        Item item = slots[selectedID].transform.GetChild(1).GetComponent<InventoryItemData>().item;
        Transform dropPos = Camera.main.transform.parent.parent.GetComponent<PlayerFunctions>().inventoryDropPos;

        if(item.itemType == typeEnum.Weapon || item.useItemSwitcher)
        {
            if (switcher.currentItem == item.useSwitcherID)
            {
                switcher.DisableItems();
            }
        }

        if (item.itemType == typeEnum.Light && switcher.currentLightObject == item.useSwitcherID)
        {
            switcher.currentLightObject = -1;
        }

        GameObject worldItem = null;

        if (GetItemAmount(item.ID) >= 2 && item.itemType != typeEnum.Weapon)
        {
            worldItem = Instantiate(item.packDropObject, dropPos.position, dropPos.rotation);
            worldItem.name = "PackDrop_" + item.dropObject.name;

            if (worldItem.GetComponent<ExamineItem>() && string.IsNullOrEmpty(worldItem.GetComponent<ExamineItem>().examineObjectName))
            {
                worldItem.GetComponent<ExamineItem>().examineObjectName = "Sack of " + item.Title;
            }

            if (worldItem.GetComponent<InventoryItem>().messageType == InventoryItem.MessageType.ItemName && string.IsNullOrEmpty(worldItem.GetComponent<InventoryItem>().message))
            {
                worldItem.GetComponent<InventoryItem>().message = "Sack of " + item.Title;
            }

            worldItem.GetComponent<InventoryItem>().ItemType = InventoryItem.Type.InventoryItem;
            worldItem.GetComponent<InventoryItem>().InventoryID = item.ID;
        }
        else if(GetItemAmount(item.ID) == 1 || item.itemType == typeEnum.Weapon)
        {
            worldItem = Instantiate(item.dropObject, dropPos.position, dropPos.rotation);
            worldItem.name = "Drop_" + item.dropObject.name;
        }

        Physics.IgnoreCollision(worldItem.GetComponent<Collider>(), Camera.main.transform.root.GetComponent<Collider>());

        worldItem.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * (itemDropStrenght * 10));
        worldItem.GetComponent<InventoryItem>().disableType = InventoryItem.DisableType.Destroy;

        if (worldItem.GetComponent<SaveObject>())
        {
            Destroy(worldItem.GetComponent<SaveObject>());
        }

        if (GetItemAmount(item.ID) < 2 || item.useItemSwitcher || item.itemType == typeEnum.Bullets)
        {
            UseButton.interactable = false;
            DropButton.interactable = false;
            CombineButton.interactable = false;
            DropButton.transform.GetChild(0).GetComponent<MenuEvents>().ChangeTextColor("Black");
        }

        if (GetItemAmount(item.ID) > 1)
        {
            worldItem.GetComponent<InventoryItem>().Amount = GetItemAmount(item.ID);
            RemoveItemAll(item.ID);
        }
        else
        {
            RemoveItem(item.ID);
        }
    }

    public void CombineItem()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<InventorySlot>().isCombining = true;

            if (!IsCombineSlot(i))
            {
                slots[i].GetComponent<Image>().color = slotDisabled;
                slots[i].GetComponent<InventorySlot>().isCombinable = false;
            }
            else
            {
                slots[i].GetComponent<InventorySlot>().isCombinable = true;
            }
        }

        UseButton.interactable = false;
        DropButton.interactable = false;
        CombineButton.interactable = false;
        CombineButton.transform.GetChild(0).GetComponent<MenuEvents>().ChangeTextColor("Black");
    }

    bool IsCombineSlot(int slotID)
    {
        InventoryScriptable.ItemMapper.CombineSettings[] combineSettings = slots[selectedID].transform.GetChild(1).GetComponent<InventoryItemData>().item.combineSettings;

        foreach (var id in combineSettings) {
            if (slots[slotID].GetComponent<InventorySlot>().itemData != null)
            {
                if (slots[slotID].GetComponent<InventorySlot>().itemData.item.ID == id.combineWithID)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void CombineWith(Item SecondItem, int slotID)
    {
        if (slotID != selectedID)
        {
            Item SelectedItem = slots[selectedID].transform.GetChild(1).GetComponent<InventoryItemData>().item;
            InventoryScriptable.ItemMapper.CombineSettings[] selectedCombineSettings = SelectedItem.combineSettings;
            int CombinedItemID = -1;
            int CombineSwitcherID = -1;

            foreach(var id in selectedCombineSettings)
            {
                if(id.combineWithID == SecondItem.ID)
                {
                    CombinedItemID = id.resultCombineID;
                    CombineSwitcherID = id.combineSwitcherID;
                }
            }

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].GetComponent<InventorySlot>().isCombining = false;
                slots[i].GetComponent<InventorySlot>().isCombinable = false;
                slots[i].GetComponent<Image>().color = slotNormal;
            }

            if (SelectedItem.combineSound)
            {
                AudioSource.PlayClipAtPoint(SelectedItem.combineSound, Camera.main.transform.position, SelectedItem.soundVolume);
            }
            else
            {
                if (SecondItem.combineSound)
                {
                    AudioSource.PlayClipAtPoint(SecondItem.combineSound, Camera.main.transform.position, SecondItem.soundVolume);
                }
            }

            if (SelectedItem.itemType == typeEnum.ItemPart && SelectedItem.isCombinable)
            {
                int switcherID = GetItem(SelectedItem.combineSettings[0].combineWithID).useSwitcherID;
                GameObject MainObject = switcher.ItemList[switcherID];

                MonoBehaviour script = MainObject.GetComponents<MonoBehaviour>().SingleOrDefault(sc => sc.GetType().GetField("CanReload") != null);
                FieldInfo info = script.GetType().GetField("CanReload");

                if (info != null)
                {
                    bool canReload = Parser.Convert<bool>(script.GetType().InvokeMember("CanReload", BindingFlags.GetField, null, script, null).ToString());

                    if (canReload)
                    {
                        MainObject.SendMessage("Reload", SendMessageOptions.DontRequireReceiver);
                        RemoveItem(SelectedItem.ID);
                    }
                    else
                    {
                        gameManager.AddMessage("Cannot reload yet!");
                        DeselectSelected();
                    }
                }
                else
                {
                    Debug.Log(MainObject.name + " object does not have script with CanReload property!");
                }
            }
            else if (SelectedItem.isCombinable)
            {
                if (SelectedItem.combineGetSwItem && CombineSwitcherID != -1)
                {
                    if (CombineSwitcherID != -1)
                        switcher.selectItem(CombineSwitcherID);
                }

                if (SelectedItem.combineGetItem && CombinedItemID != -1)
                {
                    int a_count = GetItemAmount(SelectedItem.ID);
                    int b_count = GetItemAmount(SecondItem.ID);

                    if (a_count < 2 && b_count >= 2)
                    {
                        if (!SelectedItem.combineNoRemove)
                        {
                            StartCoroutine(WaitForRemoveAddItem(SelectedItem, CombinedItemID));
                        }
                        else
                        {
                            AddItem(CombinedItemID, 1);
                        }
                    }
                    if (a_count >= 2 && b_count < 2)
                    {
                        if (!SecondItem.combineNoRemove)
                        {
                            StartCoroutine(WaitForRemoveAddItem(SecondItem, CombinedItemID));
                        }
                        else
                        {
                            AddItem(CombinedItemID, 1);
                        }
                    }
                    if (a_count < 2 && b_count < 2)
                    {
                        if (!SelectedItem.combineNoRemove)
                        {
                            StartCoroutine(WaitForRemoveAddItem(SelectedItem, CombinedItemID));
                        }
                        else
                        {
                            AddItem(CombinedItemID, 1);
                        }
                    }
                    if (a_count >= 2 && b_count >= 2)
                    {
                        AddItem(CombinedItemID, 1);
                    }
                }

                if (!SelectedItem.combineNoRemove)
                {
                    RemoveItem(SelectedItem.ID);
                }
                if (!SecondItem.combineNoRemove)
                {
                    RemoveItem(SecondItem.ID);
                }
            }
        }
    }

	IEnumerator WaitForRemoveAddItem(Item item, int combinedID)
	{
		yield return new WaitUntil (() => !CheckItemInventory(item));
        AddItem(combinedID, 1);
	}

	public void Deselect(int id){
        slots[id].GetComponent<Image>().color = slotNormal;

        if (slots[id].transform.childCount > 1)
        {
            slots[id].transform.GetChild(1).GetComponent<InventoryItemData>().selected = false;
        }

		ItemLabel.text = "";
		ItemDescription.text = "";
		selectedID = -1;
	}

    void DeselectSelected()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<InventorySlot>().isCombining = false;
            slots[i].GetComponent<InventorySlot>().isCombinable = false;
            slots[i].GetComponent<InventorySlot>().isSelected = false;
        }
        if (selectedID != -1)
        {
            slots[selectedID].GetComponent<Image>().color = slotNormal;
            slots[selectedID].transform.GetChild(1).GetComponent<InventoryItemData>().selected = false;
            UseButton.interactable = false;
            DropButton.interactable = false;
            CombineButton.interactable = false;
            ItemLabel.text = "";
            ItemDescription.text = "";
            selectedID = -1;
        }
    }

    public void OnPointerClick (PointerEventData eventData)
	{
		if (gameManager.TabButtonPanel.activeSelf) {
			for (int i = 0; i < slots.Count; i++) {
				slots [i].GetComponent<InventorySlot> ().isCombining = false;
				slots [i].GetComponent<InventorySlot> ().isCombinable = false;
			}
			if (selectedID != -1) {
                DeselectSelected();
			}
		}
	}
}

public class Item
{
    //Main
    public int ID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public typeEnum itemType { get; set; }
    public Sprite itemSprite { get; set; }
    public GameObject dropObject { get; set; }
    public GameObject packDropObject { get; set; }

    //Toggles
    public bool isStackable { get; set; }
    public bool isUsable { get; set; }
    public bool isCombinable { get; set; }
    public bool isDroppable { get; set; }
    public bool combineGetItem { get; set; }
    public bool combineNoRemove { get; set; }
    public bool combineGetSwItem { get; set; }
    public bool useItemSwitcher { get; set; }

    //Sounds
    public AudioClip useSound { get; set; }
    public AudioClip combineSound { get; set; }
    public float soundVolume { get; set; }

    //Settings
    public int maxItemCount { get; set; }
    public int useSwitcherID { get; set; }
    public int healAmount { get; set; }

    //Combine Settings
    public InventoryScriptable.ItemMapper.CombineSettings[] combineSettings { get; set; }

    public Item(int itemId, InventoryScriptable.ItemMapper mapper)
    {
        ID = itemId;
        Title = mapper.Title;
        Description = mapper.Description;
        itemType = mapper.itemType;
        itemSprite = mapper.itemSprite;
        dropObject = mapper.dropObject;
        packDropObject = mapper.packDropObject;

        isStackable = mapper.itemToggles.isStackable;
        isUsable = mapper.itemToggles.isUsable;
        isCombinable = mapper.itemToggles.isCombinable;
        isDroppable = mapper.itemToggles.isDroppable;
        combineGetItem = mapper.itemToggles.CombineGetItem;
        combineNoRemove = mapper.itemToggles.CombineNoRemove;
        combineGetSwItem = mapper.itemToggles.CombineGetSwItem;
        useItemSwitcher = mapper.itemToggles.UseItemSwitcher;

        useSound = mapper.itemSounds.useSound;
        combineSound = mapper.itemSounds.combineSound;
        soundVolume = mapper.itemSounds.soundVolume;

        maxItemCount = mapper.itemSettings.maxItemCount;
        useSwitcherID = mapper.itemSettings.useSwitcherID;
        healAmount = mapper.itemSettings.healAmount;

        combineSettings = mapper.combineSettings;
    }
}
