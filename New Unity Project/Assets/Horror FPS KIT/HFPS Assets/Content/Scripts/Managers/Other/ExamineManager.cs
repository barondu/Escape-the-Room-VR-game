using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ExamineManager : MonoBehaviour {
    private HFPS_GameManager gameManager;
    private InputController inputManager;
	private InteractManager interact;
	private PlayerFunctions pfunc;
	private GameObject paperUI;
	private Text paperText;
	private DelayEffect delay;

	[Header("Raycast")]
	public LayerMask CullLayers;
	public string InteractLayer = "Interact";
	public string TagExamine = "Examine";
	public string TagExaminePaper = "Paper";
	public float PickupRange = 3f; 
	public float rotationDeadzone = 0.1f;
	public float rotateSpeed = 10f;
    public float timeToExamine = 1f;
	public float spamWaitTime = 0.5f;
    private bool isPaper;

    [Header("Layering")]
    public LayerMask ExamineMainCamMask;
    public LayerMask ExamineArmsCamMask;

    private LayerMask DefaultMainCamMask;
    private LayerMask DefaultArmsCamMask;
    private Camera ArmsCam;

	private bool rotSet;
	private bool isPressedRead;
	private bool isReading;

	private bool antiSpam;
	private bool isPressed;
	private bool isObjectHeld;
	private bool isExaminig;
	private bool tryExamine;
	private bool otherHeld;

	private Vector3 objectPosition;
	private Quaternion objectRotation;
	private float distance;

	private Vector3 rotateAngle;

	private GameObject objectRaycast;
	private GameObject objectHeld;	
	private Camera playerCam;

	private Ray playerAim;

    private KeyCode useKey;
	private KeyCode rotateKey;
	private KeyCode grabKey;
    private KeyCode readKey;

    private ExamineItem examinedItem;

    private void Start()
    {
        if (GetComponent<ScriptManager>() && GetComponent<InteractManager>() && GetComponent<PlayerFunctions>())
        {
            inputManager = GetComponent<ScriptManager>().GetScript<InputController>();
            gameManager = GetComponent<ScriptManager>().GetScript<HFPS_GameManager>();
            interact = GetComponent<InteractManager>();
            pfunc = GetComponent<PlayerFunctions>();
            paperUI = gameManager.PaperTextUI;
            paperText = gameManager.PaperReadText;
        }
        else
        {
            Debug.LogError("Missing one or more scripts in " + gameObject.name);
        }

        delay = gameObject.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<DelayEffect>();
        playerCam = Camera.main;
        ArmsCam = GetComponent<ScriptManager>().ArmsCameraBlur.GetComponent<Camera>();
        DefaultMainCamMask = Camera.main.cullingMask;
        DefaultArmsCamMask = ArmsCam.cullingMask;
    }
	

	void Update ()
	{
		if(inputManager.HasInputs())
		{
            useKey = inputManager.GetInput("Use");
            grabKey = inputManager.GetInput("Pickup");
            readKey = inputManager.GetInput("Read");
            rotateKey = inputManager.GetInput("Fire");
        }

		//Prevent Interact Dynamic Object when player is holding other object
		otherHeld = GetComponent<DragRigidbody> ().CheckHold ();

        if (gameManager.isPaused) return;

		if (objectRaycast && !antiSpam && examinedItem) {
			if (Input.GetKeyDown (grabKey) && !isPressed && !otherHeld) {
				isPressed = true;
				isExaminig = !isExaminig;
			} else if (isPressed) {			
				isPressed = false;
			}
		}

		if (isExaminig){
			if (!isObjectHeld){
				FirstPhase();
				tryExamine = true;
			}else{
				HoldObject();
			}
		}else if(isObjectHeld){
			DropObject();
		}

		RaycastHit hit;
		playerAim = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

		if (Physics.Raycast (playerAim, out hit, PickupRange, CullLayers)) {
			if (hit.collider.gameObject.layer == LayerMask.NameToLayer (InteractLayer)) {
				if (hit.collider.tag == TagExamine || hit.collider.tag == TagExaminePaper) {
					objectRaycast = hit.collider.gameObject;
					if (objectRaycast.GetComponent<ExamineItem> ()) {
						examinedItem = objectRaycast.GetComponent<ExamineItem> ();
					}
				} else {
					if (!tryExamine) {
						objectRaycast = null;
						examinedItem = null;
					}
				}
			} else {
				if (!tryExamine) {
					objectRaycast = null;
					examinedItem = null;
				}
			}
		} else {
			if (!tryExamine) {
				objectRaycast = null;
				examinedItem = null;
			}
		}

		float rotationInputX = 0.0f;
		float rotationInputY = 0.0f;

		float x = Input.GetAxis("Mouse X");
		float y = Input.GetAxis("Mouse Y");

		if(Mathf.Abs(x) > rotationDeadzone){
			rotationInputX = -(x * rotateSpeed);
		}

		if(Mathf.Abs(y) > rotationDeadzone){
			rotationInputY = (y * rotateSpeed);
		}
			
		if (objectHeld && isObjectHeld) {
			if (Input.GetKey (rotateKey) && !isReading) {
				objectHeld.transform.Rotate (playerCam.transform.up, rotationInputX, Space.World);
				objectHeld.transform.Rotate (playerCam.transform.right, rotationInputY, Space.World);
			}

			if (isPaper) {
				if(Input.GetKeyDown(readKey) && !isPressedRead){
					isPressedRead = true;
					isReading = !isReading;
				}else if(isPressedRead){
					isPressedRead = false;
				}

				if (isReading) {
                    ExamineItem examine = objectRaycast.GetComponent<ExamineItem>();
                    paperText.text = examine.paperReadText;
                    paperText.fontSize = examine.textSize;
					paperUI.SetActive (true);
				} else {
					paperUI.SetActive (false);
				}
            }
            else if(examinedItem && examinedItem.isUsable)
            {
                if (Input.GetKeyDown(useKey) && !isPressed)
                {
                    isPressed = true;
                    interact.Interact(objectRaycast);
                    takeObject();
                }
                else if (isPressed)
                {
                    isPressed = false;
                }
            }
		}
	}

	void FirstPhase()
	{
		if(objectRaycast.tag == TagExamine || objectRaycast.tag == TagExaminePaper)
		{
			StartCoroutine (AntiSpam ());

			objectHeld = objectRaycast.gameObject;
			if (objectRaycast.tag == TagExaminePaper) {
				isPaper = true;
			} else {
				isPaper = false;
			}

            if (!(objectHeld.GetComponent<Rigidbody>()))
            {
                Debug.LogError(objectHeld.name + " need Rigidbody Component to pickup");
                return;
            }

            if (objectHeld.GetComponent<ExamineItem> ()) {
                examinedItem = objectHeld.GetComponent<ExamineItem> ();

                if (!isPaper)
                {
                    if (!examinedItem.isExamined)
                    {
                        ExamineObject(examinedItem.examineObjectName);
                    }
                    else
                    {
                        gameManager.ShowExamineText(examinedItem.examineObjectName);
                    }
                }

                distance = examinedItem.examineDistance;
				if (examinedItem.examineSound) {
					AudioSource.PlayClipAtPoint(examinedItem.examineSound, objectRaycast.transform.position, 0.75f);
				}
			}

			if (!isObjectHeld) {
				objectPosition = objectHeld.transform.position;
				objectRotation = objectHeld.transform.rotation;
				objectHeld.GetComponent<Collider> ().isTrigger = true;
			}

			if (gameManager.gameObject.GetComponent<UIFloatingItem> ().FloatingIcons.Contains (objectHeld)) {
                gameManager.gameObject.GetComponent<UIFloatingItem> ().SetItemVisible (objectHeld, false);
			}

			if (objectHeld.transform.childCount > 0) {
				objectHeld.layer = LayerMask.NameToLayer ("Examine");
				foreach (Transform child in objectHeld.transform) {
					if (child.GetComponent<MeshFilter> ()) {
						child.gameObject.layer = LayerMask.NameToLayer ("Examine");
					}
				}
			} else {
                objectHeld.layer = LayerMask.NameToLayer ("Examine");
			}

            playerCam.cullingMask = ExamineMainCamMask;
            ArmsCam.cullingMask = ExamineArmsCamMask;

            if (isPaper)
            {
                gameManager.ShowExamineSprites(readKey, "Read");
            }
            else
            {
                if (examinedItem.isUsable)
                {
                    gameManager.ShowExamineSprites();
                }
                else
                {
                    gameManager.ShowExamineSprites(btn2: false);
                }
            }

            delay.isEnabled = false;
            pfunc.enabled = false;
            gameManager.UIPreventOverlap(true);
            gameManager.HideSprites(hideType.Interact);
            GetComponent<ScriptManager>().SetScriptEnabledGlobal = false;

            Physics.IgnoreCollision(objectRaycast.GetComponent<Collider>(), transform.root.GetComponent<Collider>(), true);

            isObjectHeld = true;
        }
	}

	void HoldObject()
	{
		Vector3 nextPos = playerCam.transform.position + playerAim.direction * distance;
		Vector3 currPos = objectRaycast.transform.position;

		interact.CrosshairVisible (false);
        gameManager.LockStates (true, true, true, false, 1);

		objectHeld.GetComponent<Rigidbody> ().isKinematic = false;
		objectHeld.GetComponent<Rigidbody> ().useGravity = false;
		objectHeld.GetComponent<Rigidbody> ().velocity = (nextPos - currPos) * 10;

		if (!rotSet && isPaper) {
			Vector3 rotation = objectRaycast.GetComponent<ExamineItem> ().paperRotation;
			objectRaycast.transform.rotation = Quaternion.LookRotation (nextPos - currPos) * Quaternion.Euler (rotation);
			rotSet = true;
		}
	}
		
	void DropObject()
	{
        if (gameManager.gameObject.GetComponent<UIFloatingItem> ().FloatingIcons.Contains (objectHeld)) {
            gameManager.gameObject.GetComponent<UIFloatingItem> ().SetItemVisible (objectHeld, true);
		}
		distance = 0;

        StopAllCoroutines();

		if (objectHeld.transform.childCount > 0) {
			objectHeld.layer = LayerMask.NameToLayer ("Interact");
			foreach (Transform child in objectHeld.transform) {
				if (child.GetComponent<MeshFilter> ()) {
					child.gameObject.layer = LayerMask.NameToLayer ("Interact");
				}
			}
		} else {
			objectHeld.layer = LayerMask.NameToLayer ("Interact");
		}

        playerCam.cullingMask = DefaultMainCamMask;
        ArmsCam.cullingMask = DefaultArmsCamMask;

        pfunc.enabled = true;
        gameManager.UIPreventOverlap(false);
        gameManager.HideSprites(hideType.Examine);
        gameManager.HideHint();
        examinedItem = null;
        isObjectHeld = false;
		isExaminig = false;
		rotSet = false;
        isReading = false;
		paperUI.SetActive (false);
        interact.CrosshairVisible (true);
        gameManager.LockStates (false, true, true, false, 1);
		objectHeld.transform.position = objectPosition;
		objectHeld.transform.rotation = objectRotation;
		if (!isPaper) {
			objectHeld.GetComponent<Collider> ().isTrigger = false;
			objectHeld.GetComponent<Rigidbody> ().isKinematic = false;
			objectHeld.GetComponent<Rigidbody> ().useGravity = true;
		} else {
			objectHeld.GetComponent<Rigidbody> ().isKinematic = true;
			objectHeld.GetComponent<Rigidbody> ().useGravity = false;
		}
		tryExamine = false;
		objectRaycast = null;
		objectHeld = null;
		delay.isEnabled = true;
        GetComponent<ScriptManager>().SetScriptEnabledGlobal = true;
        StartCoroutine (AntiSpam ());
	}

    void takeObject()
    {
        gameManager.gameObject.GetComponent<UIFloatingItem>().SetItemsVisibe(true);
        distance = 0;
        StopAllCoroutines();
        playerCam.cullingMask = DefaultMainCamMask;
        ArmsCam.cullingMask = DefaultArmsCamMask;
        pfunc.enabled = true;
        gameManager.UIPreventOverlap(false);
        gameManager.HideSprites(hideType.Examine);
        gameManager.HideHint();
        examinedItem = null;
        isObjectHeld = false;
        isExaminig = false;
        rotSet = false;
        isReading = false;
        paperUI.SetActive(false);
        interact.CrosshairVisible(true);
        gameManager.LockStates(false, true, true, false, 1);
        tryExamine = false;
        objectRaycast = null;
        objectHeld = null;
        delay.isEnabled = true;
        GetComponent<ScriptManager>().SetScriptEnabledGlobal = true;
        StartCoroutine(AntiSpam());
    }

    private void ExamineObject(string ExamineName)
    {
        //StopAllCoroutines();
        StartCoroutine(DoExamine(ExamineName));
    }

    IEnumerator DoExamine(string ExamineName)
    {
        ExamineItem[] examineItems = FindObjectsOfType<ExamineItem>().Where(i => i.examineObjectName == ExamineName).ToArray();
        yield return new WaitForSeconds(timeToExamine);

        gameManager.ShowExamineText(ExamineName);

        foreach (var inst in examineItems)
        {
            inst.isExamined = true;
        }
    }

	IEnumerator AntiSpam()
	{
		antiSpam = true;
		yield return new WaitForSeconds (spamWaitTime);
		antiSpam = false;
	}
}