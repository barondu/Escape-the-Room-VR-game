using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using rnd = ThunderWire.Helper.Random;

public enum Type_Dynamic
{
    Door,
    Drawer,
    Lever,
    Valve,
    MovableInteract
}

public enum Type_Use
{
    Normal,
    Locked,
    Jammed,
}

public enum Type_Interact
{
    Mouse,
    Animation
}

public enum Type_Key
{
    Script,
    Inventory
}

public class DynamicObject : MonoBehaviour {

    private rnd.Random rand = new rnd.Random();
    private Inventory inventory;
    private HingeJoint Joint;

    #region Enums
    public Type_Dynamic dynamicType = Type_Dynamic.Door;
    public Type_Use useType = Type_Use.Normal;
    public Type_Interact interactType = Type_Interact.Animation;
    public Type_Key keyType = Type_Key.Script;
    #endregion

    #region GenericSettings
    public Animation m_Animation;
    public List<Collider> IgnoreColliders = new List<Collider>();
    public UnityEvent InteractEvent;
    public UnityEvent DisabledEvent;
    public string customText;
    public int keyID;
    public bool loadDisabled;
    public string useAnim;
    public string backUseAnim;
    #endregion

    #region DoorSettings
    public bool useJammedPlanks;
    public List<Rigidbody> Planks = new List<Rigidbody>();

    [HideInInspector] public bool isOpened;
    private float defaultAngle;
    private float minCloseAngle;
    private float maxCloseAngle;
    #endregion

    #region DrawerSettings
    [Tooltip("If true default move vector will be X, if false default vector is Z")]
    public bool moveWithX;
    public Vector2 minMaxMove;
    public bool reverseMove;
    public float InteractPos;
    #endregion

    #region LeverSettings
    public float stopAngle;
    public bool lockOnUp;

    [HideInInspector] public bool isUp;
    #endregion

    #region ValveSettings
    public AudioClip[] valveTurnSounds;
    public float valveSoundAfter;
    public float valveTurnSpeed;
    public float valveTurnTime;

    private bool turnSound;
    private bool valveInvoked;
    #endregion

    #region Sounds
    [Range(0, 1)]
    public float m_Volume = 1;
    public AudioClip Open;
    public AudioClip Close;
    public AudioClip LockedTry;
    public AudioClip UnlockSound;
    public AudioClip LeverUpSound;
    #endregion

    /* Debug */
    public bool DebugAngle;

    [HideInInspector] public float rotateValue;
    [HideInInspector] public bool isHolding;
    [HideInInspector] public bool Hold;
    [HideInInspector] public bool hasKey;
    [HideInInspector] public bool isLocked;
    [HideInInspector] public bool isInvoked;
    [HideInInspector] public float Angle;

    private bool isPlayed;
    private bool onceUnlock;

    private Transform collisionObject;
    private Transform oldCollisionObjectParent;

    public void ParseUseType(int value)
    {
        useType = (Type_Use)value;
    }

    private void OnEnable()
    {
        defaultAngle = transform.eulerAngles.y;
        minCloseAngle = defaultAngle - 10f;
        maxCloseAngle = defaultAngle + 10f;
    }

    void Awake()
    {
        if(dynamicType == Type_Dynamic.Door)
        {
            if(interactType == Type_Interact.Mouse)
            {
                if (GetComponent<HingeJoint>())
                {
                    Joint = GetComponent<HingeJoint>();
                }
                else
                {                   
                    Debug.LogError(transform.parent.gameObject.name + " requires Hinge Joint Component!");
                    return;
                }
            }

            if (GetLockStatus())
            {
                if (interactType == Type_Interact.Mouse)
                {
                    GetComponent<Rigidbody>().freezeRotation = true;
                    Joint.useLimits = false;
                }
                isLocked = true;
            }
            else if(useType == Type_Use.Normal)
            {
                if (interactType == Type_Interact.Mouse)
                {
                    GetComponent<Rigidbody>().freezeRotation = false;
                    Joint.useLimits = true;
                }
                isLocked = false;
            }
        }
        else if(dynamicType == Type_Dynamic.Drawer)
        {
            IgnoreColliders.Add(Camera.main.transform.root.GetComponent<Collider>());
            isLocked = useType != Type_Use.Normal;
        }
        else if(dynamicType == Type_Dynamic.Lever)
        {
            isLocked = false;
        }

        if (IgnoreColliders.Count > 0)
        {
            for (int i = 0; i < IgnoreColliders.Count; i++)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), IgnoreColliders[i]);
            }
        }
    }

    void Start()
    {
        inventory = GameObject.Find("GAMEMANAGER").GetComponent<HFPS_GameManager>().inventoryScript;
    }

    void Update()
    {
        if (CanLockType())
        {
            if (!isLocked)
            {
                useType = Type_Use.Normal;
                if (interactType == Type_Interact.Mouse && Joint)
                {
                    Joint.useLimits = true;
                    GetComponent<Rigidbody>().freezeRotation = false;
                }
            }
        }

        if (dynamicType == Type_Dynamic.Door)
        {
            Angle = transform.eulerAngles.y;

            if (GetLockStatus())
            {
                if (useJammedPlanks)
                {
                    CheckJammed();

                    if (Planks.Count < 1)
                    {
                        isLocked = false;
                    }
                }
            }

            if (interactType == Type_Interact.Mouse)
            {
                Joint.useMotor = isHolding;

                if (DebugAngle)
                {
                    if (Joint.limits.min < -1)
                    {
                        Debug.Log("Angle: " + Angle + " , Door Close: " + (defaultAngle - 0.5f));
                    }
                    else
                    {
                        Debug.Log("Angle: " + Angle + " , Door Close: " + (defaultAngle + 0.5));
                    }
                }

                if (Angle > 1f)
                {
                    if (Joint.limits.min < -1)
                    {
                        if (Angle <= (defaultAngle - 2f) && !isOpened)
                        {
                            if (Open)
                            {
                                AudioSource.PlayClipAtPoint(Open, transform.position);
                            }
                            isOpened = true;
                        }

                        if (Angle > minCloseAngle && Angle < maxCloseAngle && Angle >= (defaultAngle - 0.5f) && isOpened)
                        {
                            if (Close)
                            {
                                AudioSource.PlayClipAtPoint(Close, transform.position);
                            }
                            isOpened = false;
                        }
                    }
                    else
                    {
                        if (Angle >= (defaultAngle + 0.5f) && !isOpened)
                        {
                            if (Open)
                            {
                                AudioSource.PlayClipAtPoint(Open, transform.position);
                            }
                            isOpened = true;
                        }

                        if (Angle <= (defaultAngle + 0.2f) && isOpened)
                        {
                            if (Close)
                            {
                                AudioSource.PlayClipAtPoint(Close, transform.position);
                            }
                            isOpened = false;
                        }
                    }
                }
            }
        }
        else if (dynamicType == Type_Dynamic.Lever)
        {
            if (reverseMove)
            {
                Angle = transform.localEulerAngles.x;
            }
            else
            {
                Angle = transform.localEulerAngles.y;
            }

            float minAngle = Angle - 2f;
            float maxAngle = Angle + 2f;

            if (interactType == Type_Interact.Mouse)
            {
                if (DebugAngle)
                {
                    Debug.Log("Angle: " + Mathf.Round(Angle) + " Min: " + minAngle + " Max: " + maxAngle);
                }

                if (lockOnUp)
                {
                    if (Hold)
                    {
                        GetComponent<Rigidbody>().isKinematic = true;
                        GetComponent<Rigidbody>().useGravity = false;
                    }
                    else
                    {
                        GetComponent<Rigidbody>().isKinematic = false;
                        GetComponent<Rigidbody>().useGravity = true;
                    }
                }
                else
                {
                    if (isHolding)
                    {
                        GetComponent<Rigidbody>().isKinematic = false;
                        GetComponent<Rigidbody>().useGravity = false;
                    }

                    if (!isHolding && Hold)
                    {
                        GetComponent<Rigidbody>().isKinematic = true;
                        GetComponent<Rigidbody>().useGravity = false;
                    }
                    else if (!Hold)
                    {
                        GetComponent<Rigidbody>().isKinematic = false;
                        GetComponent<Rigidbody>().useGravity = true;
                    }
                }

                if (!DebugAngle)
                {
                    if (Angle > minAngle && Angle < maxAngle && Angle >= stopAngle)
                    {
                        if (!loadDisabled)
                        {
                            InteractEvent.Invoke();
                            if (!isPlayed && LeverUpSound)
                            {
                                AudioSource.PlayClipAtPoint(LeverUpSound, transform.position, 1f);
                                isPlayed = true;
                            }
                        }
                        Hold = true;
                    }
                    else
                    {
                        DisabledEvent.Invoke();
                        loadDisabled = false;
                        isPlayed = false;
                        Hold = false;
                    }
                }
            }
            else
            {
                if (isUp && !isInvoked && !isOpened)
                {
                    if (!loadDisabled)
                    {
                        StartCoroutine(WaitEventInvoke());
                    }
                    isOpened = true;
                }
            }
        }
        else if (dynamicType == Type_Dynamic.Valve)
        {
            if (rotateValue >= 1f && !valveInvoked)
            {
                if (!loadDisabled)
                {
                    InteractEvent.Invoke();
                }
                valveInvoked = true;
            }
            else if (turnSound)
            {
                StartCoroutine(ValveSounds());
                turnSound = false;
            }
        }
        else if (dynamicType == Type_Dynamic.MovableInteract)
        {
            if (!isInvoked)
            {
                if (moveWithX)
                {
                    if (minMaxMove.x < minMaxMove.y)
                    {
                        if (transform.localPosition.x <= InteractPos)
                        {
                            InteractEvent.Invoke();
                            isInvoked = true;
                        }
                    }
                    else if(minMaxMove.y > minMaxMove.x)
                    {
                        if (transform.localPosition.x >= InteractPos)
                        {
                            InteractEvent.Invoke();
                            isInvoked = true;
                        }
                    }
                }
                else
                {
                    if (minMaxMove.x < minMaxMove.y)
                    {
                        if (transform.localPosition.z >= InteractPos)
                        {
                            InteractEvent.Invoke();
                            isInvoked = true;
                        }
                    }
                    else if (minMaxMove.y > minMaxMove.x)
                    {
                        if (transform.localPosition.z <= InteractPos)
                        {
                            InteractEvent.Invoke();
                            isInvoked = true;
                        }
                    }
                }
            }
        }

        if (!isHolding)
        {
            turnSound = true;
        }
    }

    public void UseObject()
    {
        if (CanLockType())
        {
            if (!onceUnlock && isLocked)
            {
                if (LockedTry && !CheckHasKey())
                {
                    AudioSource.PlayClipAtPoint(LockedTry, transform.position, m_Volume);
                }

                TryUnlock();
            }
        }

        if (interactType == Type_Interact.Animation && !isLocked)
        {
            if (dynamicType == Type_Dynamic.Door || dynamicType == Type_Dynamic.Drawer || dynamicType == Type_Dynamic.Lever)
            {
                if (!m_Animation.isPlaying && !Hold)
                {
                    if (!isOpened)
                    {
                        m_Animation.Play(useAnim);
                        if (Open) { AudioSource.PlayClipAtPoint(Open, transform.position, m_Volume); }
                        if (dynamicType == Type_Dynamic.Lever) { StartCoroutine(LeverSound()); }
                        isOpened = true;
                    }
                    else
                    {
                        m_Animation.Play(backUseAnim);
                        if (Close && dynamicType == Type_Dynamic.Drawer) { AudioSource.PlayClipAtPoint(Close, transform.position, m_Volume); }
                        if (dynamicType == Type_Dynamic.Lever) { StartCoroutine(LeverSound()); }
                        isOpened = false;
                    }
                }

                if (Hold) return;

                if (dynamicType == Type_Dynamic.Lever)
                {
                    StartCoroutine(WaitEventInvoke());
                    Hold = lockOnUp;
                }
            }
        }
    }

    private void TryUnlock()
    {
        if (keyType == Type_Key.Inventory)
        {
            if (inventory && keyID != -1)
            {
                if (inventory.CheckItemIDInventory(keyID))
                {
                    hasKey = true;
                    if (UnlockSound) { AudioSource.PlayClipAtPoint(UnlockSound, transform.position, m_Volume); }
                    StartCoroutine(WaitUnlock());
                    inventory.RemoveItem(keyID);
                    onceUnlock = true;
                }
            }
            else if (!inventory)
            {
                Debug.LogError("Inventory script is not set!");
            }
        }
        else
        {
            if (hasKey)
            {
                if (UnlockSound) { AudioSource.PlayClipAtPoint(UnlockSound, transform.position, m_Volume); }
                StartCoroutine(WaitUnlock());
                onceUnlock = true;
            }
        }
    }

    IEnumerator WaitEventInvoke()
    {
        yield return new WaitUntil(() => !m_Animation.isPlaying);

        if (!isInvoked)
        {
            InteractEvent.Invoke();
            isInvoked = true;
            isUp = true;
            yield return null;
        }
        else
        {
            DisabledEvent.Invoke();
            loadDisabled = false;
            isInvoked = false;
            isUp = false;
            yield return null;
        }
    }

    IEnumerator ValveSounds()
    {
        while (isHolding && !valveInvoked)
        {
            int soundID = rand.Range(0, valveTurnSounds.Length);
            AudioSource.PlayClipAtPoint(valveTurnSounds[soundID], transform.position, m_Volume);
            yield return new WaitForSeconds(valveSoundAfter);
        }

        yield return null;
    }

    IEnumerator LeverSound()
    {
        yield return new WaitUntil(() => !m_Animation.isPlaying);
        if (LeverUpSound) { AudioSource.PlayClipAtPoint(LeverUpSound, transform.position, m_Volume); }
    }

    IEnumerator WaitUnlock()
    {
        if (UnlockSound)
        {
            yield return new WaitForSeconds(UnlockSound.length);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        isLocked = false;
    }

    public bool GetLockStatus()
    {
        return useType == Type_Use.Jammed || useType == Type_Use.Locked;
    }

    public bool CheckHasKey()
    {
        if (keyType == Type_Key.Inventory)
        {
            if (inventory)
            {
                return inventory.CheckItemIDInventory(keyID);
            }
        }
        else if (hasKey)
        {
            return true;
        }

        return false;
    }

    private void CheckJammed()
    {
        for (int i = 0; i < Planks.Count; i++)
        {
            if (!Planks[i].isKinematic)
            {
                Planks.RemoveAt(i);
            }
        }
    }

    private bool GetJointType()
    {
        return dynamicType == Type_Dynamic.Door || dynamicType == Type_Dynamic.Lever;
    }

    private bool CanLockType()
    {
        return dynamicType == Type_Dynamic.Door || dynamicType == Type_Dynamic.Drawer;
    }

    public void UnlockDoor()
    {
        if (isLocked)
        {
            if (UnlockSound) { AudioSource.PlayClipAtPoint(UnlockSound, transform.position, m_Volume); }
            isLocked = false;
        }
    }

    public void LoadUnlock()
    {
        isLocked = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() && dynamicType == Type_Dynamic.Drawer)
        {
            collisionObject = collision.transform;
            oldCollisionObjectParent = collisionObject.transform.parent;
            collisionObject.transform.SetParent(this.transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() && dynamicType == Type_Dynamic.Drawer)
        {
            collisionObject.transform.SetParent(oldCollisionObjectParent);
            collisionObject = null;
        }
    }

    public void DoorCloseEvent()
    {
        if (Close) { AudioSource.PlayClipAtPoint(Close, transform.position, m_Volume); }
    }
}
