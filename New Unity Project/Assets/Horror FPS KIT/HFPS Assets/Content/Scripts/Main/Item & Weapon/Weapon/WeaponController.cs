/*
 * WeaponController.cs - script is written by ThunderWire Games
 * This script controls weapon
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum fireMode { None, Semi, Auto, Burst, Shotgun }
public enum Aim { Simple, Sniper }

public class WeaponController : MonoBehaviour
{
    [HideInInspector]
    public fireMode currentMode = fireMode.Semi;
    public fireMode firstMode = fireMode.Semi;
    public fireMode secondMode = fireMode.Burst;
    public Aim aimMode = Aim.Simple;

    [Header("Setup")]
	public ScriptManager scriptManager;
	private InputController inputManager;
    private HFPS_GameManager gameManager;
    private PlayerController playerController;

    [HideInInspector]
	public Inventory inventory;

	[Header("Weapon Inventory Settings")]
	public int weaponID;
	public int bulletsID;

    [Header("Weapon Bullets")]
    [Tooltip("Bullets in Magazine")]
    public int bulletsRemaining = 0;

    [Tooltip("Bullets per Magazine")]
    public int bulletsPerMag = 20;

    [Tooltip("Current Weapon Bullets")]
    public int bullets = 0;

    [Header("Weapon Configuration")]
    public LayerMask layerMask;
    public int damage = 20;
    private float fireRate = 0.1f;
    public float fireRateFirstMode = 0.1f;
    public float fireRateSecondMode = 0.1f;
    public float range = 250.0f;
    public float force = 200.0f;
	public float hitforce = 20.0f;

    [Header("Accuracy Settings")]
    public float baseInaccuracyAIM = 0.005f;
    public float baseInaccuracyHIP = 1.5f;
    public float inaccuracyIncreaseOverTime = 0.2f;
    public float inaccuracyDecreaseOverTime = 0.5f;
    private float maximumInaccuracy;
    public float maxInaccuracyHIP = 5.0f;
    public float maxInaccuracyAIM = 1.0f;
    private float triggerTime = 0.05f;
    private float baseInaccuracy;

    [Header("Aiming")]
    public Vector3 aimPosition;
    private bool aiming;
    private Vector3 curVect;
    private Vector3 hipPosition = Vector3.zero;
    public float aimSpeed = 0.25f;
    public float zoomSpeed = 0.5f;
    public int FOV = 40;

    private float scopeTime;
    private bool inScope = false;
    public Texture scopeTexture;

    [Header("Burst Settings")]
    public int shotsPerBurst = 3;
    public float burstTime = 0.07f;

    [Header("Shotgun Settings")]
    public int pelletsPerShot = 10;

    [Header("Kickback")]
    public Transform kickGO;
    public float kickUp = 0.5f;
    public float kickSideways = 0.5f;

	[System.Serializable]
	public class m_bulletmarks 
	{
		public string Tag;
		public GameObject BulletHole;
		public bool isFlesh;
	}

    [Space(3)]
	[Tooltip("Element ’0’ is always Default bullet hole.")]
	public m_bulletmarks[] Bulletmarks;
	private List<string> tags = new List<string>();

    [Header("Audio")]
    public AudioSource aSource;
    public AudioClip soundDraw;
    public AudioClip soundFire;
    public AudioClip soundEmpty;
    public AudioClip switchModeSound;

    private Animation weaponAnim;

    [Header("Animations")]
    public string fireAnim = "Fire";
    [Range(0.0f, 5.0f)]
    public float fireAnimSpeed = 1.0f;

    [Space(5f)]

    public string drawAnim = "Draw";
    [Range(0.0f, 5.0f)]
    public float drawAnimSpeed = 1.0f;

    [Space(5f)]

    public string hideAnim = "Hide";
	[Range(0.0f, 5.0f)]
	public float hideAnimSpeed = 1.0f;

    [Space(5f)]

    public string reloadAnim = "Reload";
    [Range(0.0f, 5.0f)]
    public float reloadAnimSpeed = 1.0f;

    public string switchAnim = "SwitchAnim";
    public string idleAnim;
	
    [Header("MuzzleFlash/Light")]
    public Renderer muzzleFlash;
    public Light muzzleLight;

    private Camera mainCamera;
    private Camera wepCamera;

    [HideInInspector]
    public bool reloading = false;

    [HideInInspector]
    public bool selected = false;

    private bool showUI = false;
    private bool canSwicthMode = true;
    private bool draw;
    private bool playing = false;
    private bool isFiring = false;
    private bool bursting = false;
    private bool muzzleShow = false;
    private float nextFireTime = 0.0f;
    private RaycastHit hit;
    private float camFOV = 60.0f;
    private bool wallHit;

	private KeyCode ZoomKey = KeyCode.Mouse1;
	private KeyCode SwitchFireModeKey = KeyCode.M;
	private KeyCode FireKey;
	private KeyCode ReloadKey;

    void Awake()
    {
        playerController = transform.root.GetComponent<PlayerController>();
        inputManager = scriptManager.GetScript<InputController>();
        gameManager = scriptManager.GetScript<HFPS_GameManager>();
        weaponAnim = transform.GetChild(0).GetComponent<Animation>();
        mainCamera = Camera.main;
        wepCamera = Camera.main.transform.parent.GetChild(1).GetComponent<Camera>();
    }

    void Start()
    {
        muzzleFlash.enabled = false;
        muzzleLight.enabled = false;
        hipPosition = transform.GetChild(0).localPosition;
        camFOV = mainCamera.fieldOfView;
        currentMode = firstMode;
        fireRate = fireRateFirstMode;
        aiming = false;
		
		for (int i= 0; i < Bulletmarks.Length; i++)
		{
			if(!(tags.Count == Bulletmarks.Length))
			{
				tags.Add(Bulletmarks[i].Tag);
			}
		}
    }

    void Update()
    {
        inventory = scriptManager.GetScript<Inventory>();

        if (inventory && weaponID != -1 && bulletsID != -1)
        {
            if (inventory.CheckItemIDInventory(weaponID))
            {
                inventory.SetItemAmount(weaponID, bulletsRemaining);
            }

            if (inventory.CheckItemIDInventory(bulletsID))
            {
                bullets = inventory.GetItemAmount(bulletsID);               
            }
            else
            {
                bullets = 0;
            }
        }

        if (gameManager)
        {
            if (showUI)
            {
                gameManager.AmmoUI.SetActive(true);
                gameManager.BulletsText.text = bulletsRemaining.ToString();
                gameManager.MagazinesText.text = bullets.ToString();
            }
            else
            {
                gameManager.AmmoUI.SetActive(false);
            }
        }

        if (inputManager && inputManager.HasInputs())
        {
            FireKey = inputManager.GetInput("Fire");
            ReloadKey = inputManager.GetInput("Reload");
        }

        if (!transform.GetChild(0).gameObject.activeSelf) return; 
        if (!scriptManager.SetScriptEnabledGlobal) return;
        if (wallHit) return;

        if (selected)
        {
            if (Cursor.lockState == CursorLockMode.None)
                return;

            if (Input.GetKeyDown(FireKey))
            {
                if (currentMode == fireMode.Semi)
                {
                    FireSemi();
                }
                else if (currentMode == fireMode.Burst)
                {
                    StartCoroutine(FireBurst());
                }
                else if (currentMode == fireMode.Shotgun)
                {
                    FireShotgun();
                }

                if (bulletsRemaining > 0)
                    isFiring = true;
            }

            if (Input.GetKey(FireKey))
            {
                if (currentMode == fireMode.Auto)
                {
                    FireSemi();
                    if (bulletsRemaining > 0)
                        isFiring = true;
                }
            }

            if (Input.GetKeyDown(ReloadKey))
            {
                StartCoroutine(Reload());
            }
        }

        if (Input.GetKey(ZoomKey) && !reloading && selected)
        {
            if (!aiming)
            {
                aiming = true;
                curVect = aimPosition - transform.GetChild(0).transform.localPosition;
                scopeTime = Time.time + aimSpeed;
            }
            if (transform.GetChild(0).localPosition != aimPosition && aiming)
            {
                if (Mathf.Abs(Vector3.Distance(transform.GetChild(0).localPosition, aimPosition)) < curVect.magnitude / aimSpeed * Time.deltaTime)
                {
                    transform.GetChild(0).localPosition = aimPosition;
                }
                else
                {
                    transform.GetChild(0).localPosition += curVect / aimSpeed * Time.deltaTime;
                }
            }

            if (aimMode == Aim.Sniper)
            {
                if (Time.time >= scopeTime && !inScope)
                {
                    inScope = true;
                    Component[] gos = GetComponentsInChildren<Renderer>();
                    foreach (var go in gos)
                    {
                        Renderer a = go as Renderer;
                        a.enabled = false;
                    }
                }
            }
        }
        else
        {
            if (aiming)
            {
                aiming = false;
                inScope = false;
                curVect = hipPosition - transform.GetChild(0).localPosition;
                if (aimMode == Aim.Sniper)
                {
                    Component[] go = GetComponentsInChildren<Renderer>();
                    foreach (var g in go)
                    {
                        Renderer b = g as Renderer;
                        if (b.name != "muzzle_flash")
                            b.enabled = true;
                    }
                }
            }

            if (Mathf.Abs(Vector3.Distance(transform.GetChild(0).localPosition, hipPosition)) < curVect.magnitude / aimSpeed * Time.deltaTime)
            {
                transform.GetChild(0).localPosition = hipPosition;
            }
            else
            {
                transform.GetChild(0).localPosition += curVect / aimSpeed * Time.deltaTime;
            }
        }

        if (weaponAnim.gameObject.activeSelf)
        {
            if (aiming)
            {
                maximumInaccuracy = maxInaccuracyAIM;
                baseInaccuracy = baseInaccuracyAIM;
                scriptManager.GetScript<HFPS_GameManager>().Crosshair.enabled = false;

                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, FOV, zoomSpeed * Time.deltaTime);
                if (wepCamera)
                {
                    wepCamera.fieldOfView = Mathf.Lerp(wepCamera.fieldOfView, FOV, zoomSpeed * Time.deltaTime);
                }
            }
            else
            {
                maximumInaccuracy = maxInaccuracyHIP;
                baseInaccuracy = baseInaccuracyHIP;
                scriptManager.GetScript<HFPS_GameManager>().Crosshair.enabled = true;

                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camFOV, Time.deltaTime * 3);
                if (wepCamera)
                {
                    wepCamera.fieldOfView = Mathf.Lerp(wepCamera.fieldOfView, camFOV, Time.deltaTime * 3);
                }
            }
        }

        if (playerController.velMagnitude > 3.0f)
        {
            triggerTime += inaccuracyDecreaseOverTime;
        }

        if (isFiring)
        {
            triggerTime += inaccuracyIncreaseOverTime;
        }
        else
        {
            if (playerController.velMagnitude < 3.0f)
                triggerTime -= inaccuracyDecreaseOverTime;
        }

        if (triggerTime >= maximumInaccuracy)
        {
            triggerTime = maximumInaccuracy;
        }

        if (triggerTime <= baseInaccuracy)
        {
            triggerTime = baseInaccuracy;
        }

        if (nextFireTime > Time.time)
        {
            isFiring = false;
        }

        if (Input.GetKeyDown(SwitchFireModeKey) && secondMode != fireMode.None && canSwicthMode)
        {
            if (currentMode != firstMode)
            {
                StartCoroutine(FirstFireMode());
            }
            else
            {
                StartCoroutine(SecondFireMode());
            }
        }
    }

    void FixedUpdate ()
    {
		if (!transform.GetChild(0).gameObject.activeSelf) return;
		if(inScope) return;

        if (muzzleShow)
        {
            //muzzleFlash.transform.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
            muzzleFlash.enabled = true;
            muzzleLight.enabled = true;
            muzzleShow = false;
        }
        else
        {
            muzzleFlash.enabled = false;
            muzzleLight.enabled = false;
        }
    }

    void OnGUI()
    {
        if (selected)
        {
            if (scopeTexture != null && inScope)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), scopeTexture, ScaleMode.StretchToFill);
            }
        }
    }

    IEnumerator FirstFireMode()
    {
        canSwicthMode = false;
        selected = false;

        aSource.clip = switchModeSound;
        aSource.Play();

        PlayAnimationName(switchAnim, 1f);

        yield return new WaitForSeconds(0.6f);

        currentMode = firstMode;
        fireRate = fireRateFirstMode;
        selected = true;
        canSwicthMode = true;
    }

    IEnumerator SecondFireMode()
    {
        canSwicthMode = false;
        selected = false;

        aSource.clip = switchModeSound;
        aSource.Play();

        //PlayAnimationName(switchAnim, 1f);

        yield return new WaitForSeconds(0.6f);

        currentMode = secondMode;
        fireRate = fireRateSecondMode;
        selected = true;
        canSwicthMode = true;
    }

    void FireSemi()
    {
        if (reloading || bulletsRemaining <= 0)
        {
            if (bulletsRemaining == 0)
            {
               StartCoroutine(OutOfAmmo());
            }
            return;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time)
        {
            FireOneBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator FireBurst()
    {
        int shotCounter = 0;

        if (reloading || bursting || bulletsRemaining <= 0)
        {
            if (bulletsRemaining <= 0)
            {
               StartCoroutine(OutOfAmmo());
            }
            yield break;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            while (shotCounter < shotsPerBurst)
            {
                bursting = true;
                shotCounter++;
                if (bulletsRemaining > 0)
                {
                    FireOneBullet();
                }
                yield return new WaitForSeconds(burstTime);
            }
            nextFireTime = Time.time + fireRate;
        }
        bursting = false;
    }

    void FireShotgun()
    {
        if (reloading || bulletsRemaining <= 0 || draw)
        {
            if (bulletsRemaining == 0)
            {
               StartCoroutine(OutOfAmmo());
            }
            return;
        }

        int pellets = 0;

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            while (pellets < pelletsPerShot)
            {
                FireOnePellet();
                pellets++;
            }
            bulletsRemaining--;
            nextFireTime = Time.time + fireRate;
        }

        weaponAnim.Rewind(fireAnim);
        weaponAnim.Play(fireAnim);

        aSource.PlayOneShot(soundFire, 1.0f);

        muzzleShow = true;
        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUp, Random.Range(-kickSideways, kickSideways), 0));
    }

    void FireOneBullet()
    {
        if (nextFireTime > Time.time || draw)
        {
            if (bulletsRemaining <= 0)
            {
               StartCoroutine(OutOfAmmo());
            }
            return;
        }

        //Vector3 dir = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * triggerTime, Random.Range(-0.01f, 0.01f) * triggerTime, 1));
		Ray aim = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 pos = mainCamera.transform.position;
		
        if (Physics.Raycast(aim, out hit, range, layerMask))
        {
			//Debug.Log("Hit: " + hit.collider.gameObject.name);
			
            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (hit.rigidbody){
                hit.rigidbody.AddForceAtPosition(aim.direction * hitforce, hit.point);
			}
		
			BulletMark(hit, contact, rot);
        }
		
        aSource.PlayOneShot(soundFire);

        PlayAnimationName(fireAnim, fireAnimSpeed);

        muzzleShow = true;
        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUp, Random.Range(-kickSideways, kickSideways), 0));

        bulletsRemaining--;
    }

    void FireOnePellet()
    {
        //Vector3 dir = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * triggerTime, Random.Range(-0.01f, 0.01f) * triggerTime, 1));
		Ray aim = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 pos = mainCamera.transform.position;

        if (Physics.Raycast(aim, out hit, range, layerMask))
        {
            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (hit.rigidbody){
                hit.rigidbody.AddForceAtPosition(aim.direction * hitforce, hit.point);
			}
			
			BulletMark(hit, contact, rot);
        }
    }

    void BulletMark(RaycastHit hit, Vector3 contact, Quaternion rot)
    {
        float rScale = Random.Range(0.5f, 1.0f);
        string m_tag = "";

        if (hit.collider.gameObject.GetComponent<CustomTag>())
        {
            m_tag = hit.collider.gameObject.GetComponent<CustomTag>().Tag;
        }
        else
        {
            m_tag = hit.collider.tag;
        }

        if (Bulletmarks.Length > 0)
        {
            for (int i = 0; i < Bulletmarks.Length; i++)
            {
                if (m_tag == Bulletmarks[i].Tag)
                {
                    if (!Bulletmarks[i].isFlesh)
                    {
                        GameObject bulletMark = Instantiate(Bulletmarks[i].BulletHole, contact, rot) as GameObject;
                        bulletMark.transform.localPosition += .02f * hit.normal;
                        bulletMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                        bulletMark.transform.parent = hit.transform;
                    }
                    else
                    {
                        Instantiate(Bulletmarks[i].BulletHole, contact, rot);
                        hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
                    }
                }
                if (!tags.Contains(m_tag))
                {
                    GameObject bulletMark = Instantiate(Bulletmarks[0].BulletHole, contact, rot) as GameObject;
                    bulletMark.transform.localPosition += .02f * hit.normal;
                    bulletMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                    bulletMark.transform.parent = hit.transform;
                }
            }
        }
    }

    private float GetAnimationLenght(string AnimationName)
    {
        return weaponAnim[AnimationName].length;
    }

    private void PlayAnimationName(string AnimationName, float AnimationSpeed)
    {
        weaponAnim[AnimationName].speed = AnimationSpeed;
        weaponAnim.Rewind(AnimationName);
        weaponAnim.Play(AnimationName, PlayMode.StopAll);
    }


    IEnumerator OutOfAmmo()
    {
        if (reloading || playing) yield break;

        if (soundEmpty) { AudioSource.PlayClipAtPoint(soundEmpty, mainCamera.transform.position, 0.3f); }
    }

    IEnumerator Reload()
	{
		if (reloading)
			yield break;

		int bulletsToFullMag = bulletsPerMag - bulletsRemaining;

		reloading = true;
		canSwicthMode = false;

		if (bullets > 0 && bulletsRemaining != bulletsPerMag) {
            PlayAnimationName(reloadAnim, reloadAnimSpeed);
            yield return new WaitForSeconds(GetAnimationLenght(reloadAnim) / reloadAnimSpeed);

            inventory.RemoveItemAmount (bulletsID, bulletsToFullMag);

			if (bullets >= bulletsToFullMag) {
                bulletsRemaining = bulletsPerMag;
			} else {
                bulletsRemaining = bulletsRemaining + bullets;
			}

			bullets = inventory.GetItemAmount (bulletsID);
		}

		reloading = false;
		canSwicthMode = true;
		isFiring = false;
	}

    public void Select()
    {
        StartCoroutine(SelectEvent());
        showUI = true;
    }

    public void Deselect()
    {
        StartCoroutine(DeselectEvent());
    }

    public void WallHit(bool hit)
    {
        wallHit = hit;

        if (hit)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camFOV, Time.deltaTime * 3);
            if (wepCamera)
            {
                wepCamera.fieldOfView = Mathf.Lerp(wepCamera.fieldOfView, camFOV, Time.deltaTime * 3);
            }
        }
    }

    IEnumerator SelectEvent()
    {
        transform.GetChild(0).gameObject.SetActive (true);
        scriptManager.GetScript<PlayerFunctions>().zoomEnabled = false;
        //wepCamera.fieldOfView = FOV;
        draw = true;
        canSwicthMode = false;

        aSource.clip = soundDraw;
        aSource.Play();

        weaponAnim[drawAnim].speed = 1;
        PlayAnimationName(drawAnim, drawAnimSpeed);
        yield return new WaitUntil(() => !weaponAnim.isPlaying);

        draw = false;
        reloading = false;
        canSwicthMode = true;
        selected = true;
    }

	IEnumerator DeselectEvent()
    {
		if (!selected) yield break;
        showUI = false;
        scriptManager.GetScript<PlayerFunctions>().zoomEnabled = true;
        selected = false;
        mainCamera.fieldOfView = camFOV;
        transform.GetChild(0).localPosition = hipPosition;

        if (weaponAnim.gameObject.activeSelf)
        {
            PlayAnimationName(hideAnim, hideAnimSpeed);
        }

        yield return new WaitUntil(() => !weaponAnim.isPlaying);

        weaponAnim.gameObject.SetActive (false);
    }

    public void Disable()
    {
        weaponAnim.Play(drawAnim);
        weaponAnim[drawAnim].speed = 0;
        scriptManager.GetScript<PlayerFunctions>().zoomEnabled = true;
        selected = false;
        showUI = false;
        mainCamera.fieldOfView = camFOV;
        transform.GetChild(0).localPosition = hipPosition;
        StartCoroutine(DoDisable());
    }

    IEnumerator DoDisable()
    {
        yield return new WaitUntil(() => weaponAnim[drawAnim].speed == 0);
        weaponAnim.gameObject.SetActive(false);
    }

    public void LoaderSetItemEnabled()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        scriptManager.GetScript<PlayerFunctions>().zoomEnabled = false;
        weaponAnim.Play(idleAnim);
        draw = false;
        reloading = false;
        canSwicthMode = true;
        selected = true;
        showUI = true;
    }
}