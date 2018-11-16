/*
 * PlayerFunctions.cs - wirted by ThunderWire Games
 * ver. 1.2
*/

using UnityEngine;

public class PlayerFunctions : MonoBehaviour {

    public enum LeanDirections { Normal, Left, Right }

    private InputController inputManager;

	[Header("Player Lean")]
	public LayerMask LeanMask;
    public float LeanRay;
	public float LeanAngle;
	public float LeanPos;
	public float LeanSpeed;
	public float LeanBackSpeed;
    public float LeanBackDistance;

    [Header("Zoom Effects")]
	public Camera WeaponCamera;
    public float ZoomSpeed = 5f;
    public float NormalFOV;
    public float ZoomFOV;

    [Header("Other")]
    public Transform inventoryDropPos;

    private KeyCode ZoomKey = KeyCode.Mouse1;
	private KeyCode LeanRight;
	private KeyCode LeanLeft;

    private Camera MainCamera;
    RaycastHit hit;

    [HideInInspector]
    public bool zoomEnabled = true;

	void Start () {
        MainCamera = Camera.main;
        inputManager = GetComponent<ScriptManager>().GetScript<InputController>();
    }

    void Update()
    {
        if (inputManager.HasInputs())
        {
            LeanLeft = inputManager.GetInput("LeanLeft");
            LeanRight = inputManager.GetInput("LeanRight");
        }

        LeanUpdate();

        if (zoomEnabled)
        {
            if (Input.GetKey(ZoomKey))
            {
                MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, ZoomFOV, ZoomSpeed * Time.deltaTime);
                if (WeaponCamera)
                {
                    WeaponCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, ZoomFOV, ZoomSpeed * Time.deltaTime);
                }
            }
            else
            {
                MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, NormalFOV, ZoomSpeed * Time.deltaTime);
                if (WeaponCamera)
                {
                    WeaponCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, NormalFOV, ZoomSpeed * Time.deltaTime);
                }
            }
        }
    }

    void Lean(LeanDirections Direction)
    {
        switch (Direction)
        {
            case LeanDirections.Normal:
                MainCamera.transform.localRotation = Quaternion.Slerp(MainCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * LeanBackSpeed);
                MainCamera.transform.localPosition = Vector3.Lerp(MainCamera.transform.localPosition, new Vector3(0, 0, 0), Time.deltaTime * LeanSpeed);
                break;
            case LeanDirections.Right:
                float leanAngle = -LeanAngle;
                MainCamera.transform.localRotation = Quaternion.Slerp(MainCamera.transform.localRotation, Quaternion.Euler(0, 0, leanAngle), Time.deltaTime * LeanSpeed);
                MainCamera.transform.localPosition = Vector3.Lerp(MainCamera.transform.localPosition, new Vector3(LeanPos, 0, 0), Time.deltaTime * LeanSpeed);
                break;
            case LeanDirections.Left:
                float leanPos = -LeanPos;
                MainCamera.transform.localRotation = Quaternion.Slerp(MainCamera.transform.localRotation, Quaternion.Euler(0, 0, LeanAngle), Time.deltaTime * LeanSpeed);
                MainCamera.transform.localPosition = Vector3.Lerp(MainCamera.transform.localPosition, new Vector3(leanPos, 0, 0), Time.deltaTime * LeanSpeed);
                break;
        }
    }

    void LeanUpdate()
    {
        RaycastHit raycastHit;

        if (Input.GetKey(LeanRight))
        {
            if (Physics.Raycast(Camera.main.transform.parent.position, Camera.main.transform.parent.TransformDirection(Vector3.right * 1f), out raycastHit, LeanRay, LeanMask))
            {
                float distance = Vector3.Distance(raycastHit.point, Camera.main.transform.parent.position);

                if (distance > LeanBackDistance)
                {
                    Lean(LeanDirections.Right);
                }
                else
                {
                    Lean(LeanDirections.Normal);
                }
            }
            else
            {
                Lean(LeanDirections.Right);
            }
        }
        else
        {
            Lean(LeanDirections.Normal);
        }

        if (Input.GetKey(LeanLeft))
        {
            if (Physics.Raycast(Camera.main.transform.parent.position, Camera.main.transform.parent.TransformDirection(Vector3.left * 1f), out raycastHit, LeanRay, LeanMask))
            {
                float distance = Vector3.Distance(raycastHit.point, Camera.main.transform.parent.position);

                Debug.Log(distance);

                if (distance > LeanBackDistance)
                {
                    Lean(LeanDirections.Left);
                }
                else
                {
                    Lean(LeanDirections.Normal);
                }
            }
            else
            {
                Lean(LeanDirections.Left);
            }
        }
        else
        {
            Lean(LeanDirections.Normal);
        }
    }
}