using UnityEngine;
using UnityEngine.UI;

public class InteractManager : MonoBehaviour {
    private InputController inputManager;
    private HFPS_GameManager gameManager;
	private ItemSwitcher itemSelector;
	private Inventory inventory;

	[Header("Raycast")]
	public float RayLength = 3;
	public LayerMask cullLayers;
	public string InteractLayer;
	
	[Header("Crosshair Textures")]
	public Sprite defaultCrosshair;
	public Sprite interactCrosshair;
	private Sprite default_interactCrosshair;
	
	[Header("Crosshair")]
	private Image CrosshairUI;
	public int crosshairSize = 5;
	public int interactSize = 10;

    [Header("Texts")]
    public string UseText = "Use";
    public string UnlockText = "Unlock";
    public string GrabText = "Grab";
    public string ExamineText = "Examine";
    public string RemoveText = "Remove";

    private int default_interactSize;
    private int default_crosshairSize;

    [HideInInspector]
	public bool isHeld = false;

    [HideInInspector]
    public bool inUse;

    [HideInInspector]
	public Ray playerAim;

	[HideInInspector]
	public GameObject RaycastObject;
	
	private KeyCode UseKey;
	private KeyCode PickupKey;
	
	private Camera playerCam;
	private DynamicObject dynamic;

	private GameObject LastRaycastObject;

	private string RaycastTag;

	private bool correctLayer;

	private bool isPressed;
    private bool useTexture;

    void Start()
    {
        inputManager = GetComponent<ScriptManager>().GetScript<InputController>();
        gameManager = GetComponent<ScriptManager>().GetScript<HFPS_GameManager>();
        itemSelector = GetComponent<ScriptManager>().GetScript<ItemSwitcher>();
        CrosshairUI = gameManager.Crosshair;
        default_interactCrosshair = interactCrosshair;
        default_crosshairSize = crosshairSize;
        default_interactSize = interactSize;
        playerCam = Camera.main;
        RaycastObject = null;
        dynamic = null;
    }
	
	void Update () {
		inventory = GetComponent<ScriptManager>().GetScript<Inventory>();

		if(inputManager.HasInputs())
		{
            UseKey = inputManager.GetInput("Use");
            PickupKey = inputManager.GetInput("Pickup");
        }

		if(Input.GetKey(UseKey) && RaycastObject && !isPressed && !isHeld && !inUse){
			Interact(RaycastObject);
			isPressed = true;
		}

		if(Input.GetKeyUp(UseKey) && isPressed){
			isPressed = false;
		}
			
        Ray playerAim = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

        if (Physics.Raycast(playerAim, out hit, RayLength, cullLayers))
        {
            RaycastTag = hit.collider.gameObject.tag;

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(InteractLayer))
            {
                if(hit.collider.gameObject != RaycastObject)
                {
                    gameManager.HideSprites(hideType.Interact);
                }

                RaycastObject = hit.collider.gameObject;
                correctLayer = true;

                if (RaycastObject.GetComponent<DynamicObject>())
                {
                    dynamic = RaycastObject.GetComponent<DynamicObject>();
                }
                else
                {
                    dynamic = null;
                }

                if (RaycastObject.GetComponent<CrosshairReticle>())
                {
                    CrosshairReticle ChangeReticle = RaycastObject.GetComponent<CrosshairReticle>();
                    if (dynamic)
                    {
                        if (dynamic.useType != Type_Use.Locked)
                        {
                            interactCrosshair = ChangeReticle.interactSprite;
                            interactSize = ChangeReticle.size;
                        }
                    }
                    else
                    {
                        interactCrosshair = ChangeReticle.interactSprite;
                        interactSize = ChangeReticle.size;
                    }
                }

                useTexture = true;

                if (LastRaycastObject)
                {
                    if (!(LastRaycastObject == RaycastObject))
                    {
                        ResetCrosshair();
                    }
                }
                LastRaycastObject = RaycastObject;

                if (RaycastObject.GetComponent<UIObjectInfo>())
                {
                    UIObjectInfo objectInfo = RaycastObject.GetComponent<UIObjectInfo>();
                    gameManager.ShowInteractInfo(objectInfo.objectTitle);
                }

                if (!inUse)
                {
                    if (dynamic)
                    {
                        if (dynamic.useType == Type_Use.Locked)
                        {
                            if (dynamic.CheckHasKey())
                            {
                                gameManager.ShowInteractSprite(1, UnlockText, UseKey);
                            }
                            else
                            {
                                gameManager.ShowInteractSprite(1, UseText, UseKey);
                            }
                        }
                        else
                        {
                            gameManager.ShowInteractSprite(1, UseText, UseKey);
                        }
                    }
                    else
                    {
                        if (RaycastObject.GetComponent<DynamicObjectPlank>())
                        {
                            gameManager.ShowInteractSprite(1, RemoveText, UseKey);
                        }
                        else if (!(RaycastTag == "OnlyGrab"))
                        {
                            gameManager.ShowInteractSprite(1, UseText, UseKey);
                        }
                    }
                    if (RaycastTag == "OnlyGrab")
                    {
                        gameManager.ShowInteractSprite(1, GrabText, PickupKey);
                    }
                    else if (RaycastTag == "Grab")
                    {
                        gameManager.ShowInteractSprite(1, UseText, UseKey);
                        gameManager.ShowInteractSprite(2, GrabText, PickupKey);
                    }
                    else if (RaycastTag == "Paper")
                    {
                        gameManager.ShowInteractSprite(1, ExamineText, PickupKey);
                    }
                    if (RaycastObject.GetComponent<ExamineItem>())
                    {
                        if (RaycastObject.GetComponent<ExamineItem>().isUsable)
                        {
                            gameManager.ShowInteractSprite(1, UseText, UseKey);
                            gameManager.ShowInteractSprite(2, ExamineText, PickupKey);
                        }
                        else
                        {
                            gameManager.ShowInteractSprite(1, ExamineText, PickupKey);
                        }
                    }
                }
            }
            else if (RaycastObject)
            {
                correctLayer = false;
            }
        }
        else if (RaycastObject)
        {
            correctLayer = false;
        }

		if(!correctLayer){
			ResetCrosshair ();
			useTexture = false;
			RaycastTag = null;
			RaycastObject = null;
            dynamic = null;
		}
		
		if(!RaycastObject)
		{
            gameManager.HideSprites(hideType.Interact);
            useTexture = false;
            dynamic = null;
		}

        CrosshairUpdate();
    }

    void CrosshairUpdate()
    {
        if(useTexture)
        {
			CrosshairUI.rectTransform.sizeDelta = new Vector2(interactSize, interactSize);
            CrosshairUI.sprite = interactCrosshair;
        }
        else
        {
			CrosshairUI.rectTransform.sizeDelta = new Vector2(crosshairSize, crosshairSize);
            CrosshairUI.sprite = defaultCrosshair;
        }
    }

	private void ResetCrosshair(){
		crosshairSize = default_crosshairSize;
		interactSize = default_interactSize;
		interactCrosshair = default_interactCrosshair;
	}

	public void CrosshairVisible(bool state)
	{
		switch (state) 
		{
		case true:
			CrosshairUI.enabled = true;
			break;
		case false:
			CrosshairUI.enabled = false;
			break;
		}
	}

	public bool GetInteractBool()
	{
		if (RaycastObject) {
			return true;
		} else {
			return false;
		}
	}

    public void Interact(GameObject InteractObject)
    {
        if (InteractObject.GetComponent<Message>())
        {
            Message message = InteractObject.GetComponent<Message>();

            if (message.messageType == Message.MessageType.Hint)
            {
                gameManager.ShowHint(message.message);
            }
            else if (message.messageType == Message.MessageType.Message)
            {
                gameManager.AddMessage(message.message);
            }
            else if(message.messageType == Message.MessageType.ItemName)
            {
                gameManager.AddPickupMessage(message.message);
            }
        }

        if (InteractObject.GetComponent<InventoryItem>())
        {
            InventoryItem invItem = InteractObject.GetComponent<InventoryItem>();
            Item item = inventory.GetItem(invItem.InventoryID);

            if (invItem.markLightObject)
            {
                itemSelector.currentLightObject = item.useSwitcherID;
            }

            if (invItem.ItemType == InventoryItem.Type.BackpackExpand)
            {
                inventory.ExpandSlots(invItem.BackpackExpand);
                Pickup(InteractObject);
            }

            if (inventory.CheckInventorySpace() || inventory.CheckItemIDInventory(invItem.InventoryID))
            {
                if (inventory.GetItemAmount(item.ID) < item.maxItemCount || item.maxItemCount == 0)
                {
                    if (invItem.ItemType == InventoryItem.Type.SwitcherItem)
                    {
                        itemSelector.selectItem(invItem.WeaponID);
                    }
                    else if (invItem.ItemType == InventoryItem.Type.InventoryItem)
                    {
                        inventory.AddItem(invItem.InventoryID, invItem.Amount);
                    }
                    else if (invItem.ItemType == InventoryItem.Type.WeaponItem)
                    {
                        if (invItem.weaponType == InventoryItem.WeaponType.Weapon)
                        {
                            inventory.AddItem(invItem.InventoryID, invItem.Amount);
                            itemSelector.selectItem(invItem.WeaponID);
                            if (item.itemType == typeEnum.Weapon)
                            {
                                itemSelector.weaponItem = invItem.WeaponID;
                            }
                        }
                        else if (invItem.weaponType == InventoryItem.WeaponType.Ammo)
                        {
                            inventory.AddItem(invItem.InventoryID, invItem.Amount);
                        }
                    }

                    if (invItem.messageType == InventoryItem.MessageType.Hint)
                    {
                        gameManager.ShowHint(invItem.message);
                    }
                    if (invItem.messageType == InventoryItem.MessageType.Message)
                    {
                        gameManager.AddMessage(invItem.message);
                    }
                    if (invItem.messageType == InventoryItem.MessageType.ItemName)
                    {
                        gameManager.AddPickupMessage(invItem.message);
                    }

                    Pickup(InteractObject);
                }
                else if (inventory.GetItemAmount(item.ID) >= item.maxItemCount)
                {
                    gameManager.ShowHint("You cannot carry more " + item.Title);
                }
            }
            else
            {
                gameManager.ShowHint("No Inventory Space!");
            }
        }
        else
        {
            Pickup(InteractObject);
        }
    }

	void Pickup(GameObject InteractObject)
	{
        gameManager.HideSprites (hideType.Interact);
        InteractObject.SendMessage ("UseObject", SendMessageOptions.DontRequireReceiver);
	}
}
