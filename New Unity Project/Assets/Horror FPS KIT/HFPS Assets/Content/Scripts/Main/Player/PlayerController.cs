using UnityEngine;
using System.Collections;
using ThunderWire.Helper.Input;

[RequireComponent(typeof(Footsteps))]
public class PlayerController : MonoBehaviour
{
	[Header("Main Setup")]
	public CharacterController controller;
	public ScriptManager scriptManager;

    private ItemSwitcher switcher;
	private InputController inputManager;
	private HealthManager hm;
	private Footsteps footsteps;
	
	[Header("Movement Speed")]
    public float walkSpeed = 4;
    public float runSpeed = 7;
    public float crouchSpeed = 2;
    public float proneSpeed = 1;
    public float inWaterSpeed = 2;
    public float inWaterJumpSpeed = 5;
    public float jumpSpeed = 7;
	public float climbSpeed = 1.5f;
	public float climbRate = 0.5f;
	public float startWalkSpeed = 2.0f;
	public float stopWalkSpeed = 2.2f;
	public int transitionSpeed = 3;

    [Header("Player")]
    public float baseGravity = 24;
    public float fallDamageMultiplier = 5.0f;
    public float standFallTreshold = 8;
    public float crouchFallTreshold = 4;
    public float slideLimit = 45.0f;
	public bool airControl = false;

    private float camNormalHeight = 0.9f;
    private float camCrouchHeight = 0.2f;
    private float camProneHeight = -0.4f;

    private float rayDistance;

    private float gravity = 24;
    private float fallingDamageThreshold;
    private float fallDistance;
    private float slideSpeed = 8.0f;
    private float antiBumpFactor = .75f;
    private float antiBunnyHopFactor = 1;

	private bool sliding = false;
    private bool falling = false;

    private bool isCrawling = false;

    [Header("Animations Camera")]
    public Animation cameraAnimations;
    [SerializeField] private string runAnimation = "CameraRun";
    [Range(0, 5)] public float runAnimSpeed;
    [SerializeField] private string walkAnimation = "CameraWalk";
    [Range(0, 5)] public float walkAnimSpeed;
    [SerializeField] private string idleAnimation = "CameraIdle";

    [Header("Animations Arms")]
    public Animation armsAnimations;
    [SerializeField] private string armsRunAnimation = "ArmsRun";
    [SerializeField] private string armsWalkAnimation = "ArmsWalk";
    [SerializeField] private string armsIdleAnimation = "ArmsIdle";
    public float adjustAnimSpeed = 7.0f;

    [Header("In Water")]
    public ParticleSystem waterFoam;
    [HideInInspector] public ParticleSystem emiter;

    [Header("Other")]
    public LayerMask checkDistanceMask;
	public Transform playerWeapGO;
	public Transform cameraGO;
    public Transform fallEffect;
    public Transform fallEffectWeap;
	public AudioSource aSource;

    [HideInInspector]
    public int state = 0;	
	
    [HideInInspector]
    public bool onLadder = false;
	
    [HideInInspector]
    Vector3 moveDirection = Vector3.zero;	
	
    [HideInInspector]
    public bool run;
	
    [HideInInspector]
    public bool canRun = true;
	
	[HideInInspector]
    public bool grounded = false;

	[HideInInspector]
	public bool controllable = true;
	
    [HideInInspector]
    public float speed;
	
    [HideInInspector]
    public float velMagnitude;
	
    private RaycastHit hit;

	private float playTime = 0.0f;
	private float climbDownThreshold = -0.4f;
	private bool useLadder = true;		
	private Vector3 climbDirection = Vector3.up;
	private Vector3 lateralMove = Vector3.zero;
	private Vector3 ladderMovement = Vector3.zero;
    private Vector3 contactPoint;
    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private float highestPoint;
	private int jumpTimer;
	
	private KeyCode ForwardKey;
	private KeyCode BackwardKey;
	private KeyCode LeftKey;
	private KeyCode RightKey;
	private KeyCode JumpKey;
	private KeyCode RunKey;
	private KeyCode CrouchKey;
	private KeyCode ZoomKey = KeyCode.Mouse1;

    /// <summary>
    /// Vertical Movement
    /// Up, Down
    /// </summary>
    private float inputY;

    /// <summary>
    /// Horizontal Movement
    /// Left, Right
    /// </summary>
	private float inputX;

    void Awake()
    {
        inputManager = scriptManager.GetScript<InputController>();
        hm = GetComponent<HealthManager>();
        footsteps = GetComponent<Footsteps>();
    }

    void Start()
    {
        rayDistance = controller.height / 2 + 1.1f;
        slideLimit = controller.slopeLimit - .2f;

        cameraAnimations.wrapMode = WrapMode.Loop;
        armsAnimations.wrapMode = WrapMode.Loop;
        armsAnimations.Stop();

        cameraAnimations[runAnimation].speed = runAnimSpeed;
        cameraAnimations[walkAnimation].speed = walkAnimSpeed;

        switcher = scriptManager.GetScript<ItemSwitcher>();
    }

    void Update()
    {
		if(inputManager && inputManager.HasInputs())
		{
            ForwardKey = inputManager.GetInput("Forward");
            BackwardKey = inputManager.GetInput("Backward");
            LeftKey = inputManager.GetInput("Left");
            RightKey = inputManager.GetInput("Right");
            JumpKey = inputManager.GetInput("Jump");
            RunKey = inputManager.GetInput("Run");
            CrouchKey = inputManager.GetInput("Crouch");
        }

        velMagnitude = controller.velocity.magnitude;
        //float inputX = Input.GetAxis("Horizontal");
        //float inputY = Input.GetAxis("Vertical");

        if (controllable) {
            if (Input.GetKey(ForwardKey)) {
                inputY = InputHelper.GetKeyAxis(Axis.Forward, inputY, true, startWalkSpeed);
            } else if(inputY > 0) {
                inputY = InputHelper.GetKeyAxis(Axis.Forward, inputY, false, stopWalkSpeed);
            }

			if (Input.GetKey (BackwardKey)) {
                inputY = InputHelper.GetKeyAxis(Axis.Backward, inputY, true, startWalkSpeed);
            } else if(inputY < 0) {
                inputY = InputHelper.GetKeyAxis(Axis.Backward, inputY, false, stopWalkSpeed);
            }
		
			if (Input.GetKey (LeftKey)) {
                inputX = InputHelper.GetKeyAxis(Axis.Left, inputX, true, startWalkSpeed);
            } else if(inputX < 0) {
                inputX = InputHelper.GetKeyAxis(Axis.Left, inputX, false, stopWalkSpeed);
            }

            if (Input.GetKey (RightKey)) {
                inputX = InputHelper.GetKeyAxis(Axis.Right, inputX, true, startWalkSpeed);
            } else if(inputX > 0) {
                inputX = InputHelper.GetKeyAxis(Axis.Right, inputX, false, stopWalkSpeed);
            }
        } else {
            inputX = 0f;
            inputY = 0f;
            moveDirection = Vector3.zero;
        }

        //Debug.Log("X " + inputX + " Y: " + inputY);
		
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f) ? .7071f : 1.0f;

        if (onLadder)
        {
            LadderUpdate();
            highestPoint = transform.position.y;
            run = false;
            fallDistance = 0.0f;
            grounded = false;
            armsAnimations.CrossFade(armsIdleAnimation);
            cameraAnimations.CrossFade(idleAnimation);
            return;
        }
			
        if (grounded)
        {
            gravity = baseGravity;

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance))
            {
                float hitangle = Vector3.Angle(hit.normal, Vector3.up);
                if (hitangle > slideLimit)
                {
                    sliding = true;
                }
                else
                {
                    sliding = false;
                }
            }

            if (canRun && state == 0)
            {
                if (Input.GetKey(RunKey) && Input.GetKey(ForwardKey) && !Input.GetKey(ZoomKey) && !footsteps.inWater && controllable)
                {
                    run = true;
                }
                else
                {
                    run = false;
                }
            }

            if (falling)
            {
                if (state == 2)
                    fallingDamageThreshold = crouchFallTreshold;
                else
                    fallingDamageThreshold = standFallTreshold;

                falling = false;
                fallDistance = highestPoint - currentPosition.y;
                if (fallDistance > fallingDamageThreshold)
                {
                    ApplyFallingDamage(fallDistance);
                }

                if (fallDistance < fallingDamageThreshold && fallDistance > 0.1f)
                {
                    if (state < 2) footsteps.JumpLand();
                    StartCoroutine(FallCamera(new Vector3(7, Random.Range(-1.0f, 1.0f), 0), new Vector3(3, Random.Range(-0.5f, 0.5f), 0), 0.15f));
                }
            }

            if (sliding)
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize( ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                sliding = false;
            }
            else
            {
				if (state == 0) {
                    if (run)
                    {
                        speed = runSpeed;
                    }
                    else if (Input.GetKey(ZoomKey))
                    {
                        speed = crouchSpeed;
                    }
                    else if(!footsteps.inWater)
                    {
                        speed = walkSpeed;
                    }
                    else
                    {
                        speed = inWaterSpeed;
                    }
				} else if (state == 1) {
					speed = crouchSpeed;
					run = false;
				}
                else if (state == 2)
                {
                    speed = proneSpeed;
                    run = false;
                }
                
                /*
                if (Cursor.lockState == CursorLockMode.Locked)
                    moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                else
                    moveDirection = new Vector3(0, -antiBumpFactor, 0);    
                */

                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;

                if (controllable)
                {
                    if (!Input.GetKey(JumpKey))
                    {
                        jumpTimer++;
                    }
                    else if (jumpTimer >= antiBunnyHopFactor)
                    {
                        jumpTimer = 0;
                        if (state == 0)
                        {
                            if (!footsteps.inWater)
                            {
                                moveDirection.y = jumpSpeed;
                            }
                            else
                            {
                                moveDirection.y = inWaterJumpSpeed;
                            }
                        }

                        if (state > 0)
                        {
                            if (CheckDistance() > 1.6f)
                            {
                                state = 0;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            currentPosition = transform.position;
            if (currentPosition.y > lastPosition.y)
            {
                highestPoint = transform.position.y;
                falling = true;
            }

            if (!falling)
            {
                highestPoint = transform.position.y;
                falling = true;
            }

            if (airControl)
            {
                moveDirection.x = inputX * speed;
                moveDirection.z = inputY * speed;
                moveDirection = transform.TransformDirection(moveDirection);
            }
        }

        if (grounded)
        {
            if (!run && velMagnitude > crouchSpeed)
            {
                armsAnimations[armsWalkAnimation].speed = velMagnitude / adjustAnimSpeed;
                armsAnimations.CrossFade(armsWalkAnimation);
                cameraAnimations.CrossFade(walkAnimation);
            }
            else if(run && velMagnitude > walkSpeed)
            {
                armsAnimations.CrossFade(armsRunAnimation);
                cameraAnimations.CrossFade(runAnimation);
            }
            else if(velMagnitude < crouchSpeed)
            {
                armsAnimations.CrossFade(armsIdleAnimation);
                cameraAnimations.CrossFade(idleAnimation);
            }
        }
        else
        {
            armsAnimations.CrossFade(armsIdleAnimation);
            cameraAnimations.CrossFade(idleAnimation);
            run = false;
        }

		if (Input.GetKeyDown(CrouchKey) && !footsteps.inWater && controllable)
        {
            if (state == 0)
            {
                state = 1;
            }
            else if (state > 0)
            {
                if (CheckDistance() > 1.6f)
                {
                    state = 0;
                }
            }
        }

		if (state == 0) { //Stand Position
			controller.height = 2.0f;
			controller.center = new Vector3 (0, 0, 0);

			if (cameraGO.localPosition.y > camNormalHeight) {
				cameraGO.localPosition = new Vector3 (cameraGO.localPosition.x, camNormalHeight, cameraGO.localPosition.z);
			} else if (cameraGO.localPosition.y < camNormalHeight) {
				cameraGO.localPosition = new Vector3 (cameraGO.localPosition.x, cameraGO.localPosition.y + Time.deltaTime * transitionSpeed, cameraGO.localPosition.z);
			}

		} else if (state == 1) { //Crouch Position
			controller.height = 1.4f;
			controller.center = new Vector3 (0, -0.3f, 0);

			if (cameraGO.localPosition.y != camCrouchHeight) {
				if (cameraGO.localPosition.y > camCrouchHeight) {
					cameraGO.localPosition = new Vector3 (cameraGO.localPosition.x, cameraGO.localPosition.y - Time.deltaTime * transitionSpeed, cameraGO.localPosition.z);
				}
				if (cameraGO.localPosition.y < camCrouchHeight) {
					cameraGO.localPosition = new Vector3 (cameraGO.localPosition.x, cameraGO.localPosition.y + Time.deltaTime * transitionSpeed, cameraGO.localPosition.z);
				}

			}

        } else if (state == 2) { //Prone Position
            controller.height = 0.6f;
            controller.center = new Vector3(0, -0.7f, 0);

            if (cameraGO.localPosition.y < camProneHeight)
            {
                cameraGO.localPosition = new Vector3(cameraGO.localPosition.x, camProneHeight, cameraGO.localPosition.z);
            }
            else if (cameraGO.localPosition.y > camProneHeight)
            {
                cameraGO.localPosition = new Vector3(cameraGO.localPosition.x, cameraGO.localPosition.y - Time.deltaTime * (transitionSpeed + 1), cameraGO.localPosition.z);
            }
        }


        moveDirection.y -= gravity * Time.deltaTime;
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    float CheckDistance()
    {
        Vector3 pos = transform.position + controller.center - new Vector3(0, controller.height / 2, 0);
        RaycastHit hit;

        if (Physics.SphereCast(pos, controller.radius, transform.up, out hit, 10, checkDistanceMask))
        {
            Debug.DrawLine(pos, hit.point, Color.yellow, 2.0f);
            return hit.distance;
        }
        else
        {
            Debug.DrawLine(pos, hit.point, Color.yellow, 2.0f);
            return 3;
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        //dont move the rigidbody if the character is on top of it
        if (controller.collisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }

		body.AddForceAtPosition (controller.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

    void LateUpdate()
    {
        lastPosition = currentPosition;
    }

    IEnumerator FallCamera(Vector3 d, Vector3 dw, float ta)
    {
        Quaternion s = fallEffect.localRotation;
        Quaternion sw = fallEffectWeap.localRotation;
        Quaternion e = fallEffect.localRotation * Quaternion.Euler(d);
       // Quaternion ew = fallEffectWeap.localRotation * Quaternion.Euler(dw);
        float r = 1.0f / ta;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * r;
            fallEffect.localRotation = Quaternion.Slerp(s, e, t);
            fallEffectWeap.localRotation = Quaternion.Slerp(sw, e, t);
            yield return null;
        }
    }

    void ApplyFallingDamage(float fallDistance)
    {
		hm.ApplyDamage(fallDistance * fallDamageMultiplier);
        if (state < 2) StartCoroutine(footsteps.JumpLand());
        StartCoroutine( FallCamera(new Vector3(12, Random.Range(-2.0f, 2.0f), 0), new Vector3(4, Random.Range(-1.0f, 1.0f), 0), 0.1f));
    }

    public void PlayerInWater(float top)
    {
        Vector3 foamPos = transform.position;
        foamPos.y = top;

        if (emiter == null)
        {
            emiter = Instantiate(waterFoam, foamPos, transform.rotation) as ParticleSystem;
        }

        emiter.transform.position = foamPos;
    }

    void OnTriggerStay(Collider collider){
		if (collider.tag == "Ladder" && useLadder) {
            switcher.Ladder(true);
            onLadder = true;
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "StartCrawl" && !isCrawling)
        {
            state = 2;
            isCrawling = true;
        }
    }

    void OnTriggerExit(Collider collider){
		if (collider.tag == "Ladder") {
            switcher.Ladder(false);
            onLadder = false;
			useLadder = true;
        }
        else if (collider.tag == "StartCrawl" && isCrawling)
        {
            isCrawling = false;
        }
    }
		
	//Ladder Movement
	private void LadderUpdate () {
		float cameraRotation = Camera.main.gameObject.transform.forward.y;

        if (onLadder)
		{
			Vector3 verticalMove;
			verticalMove = climbDirection.normalized;
			verticalMove *= inputY;
			verticalMove *= (cameraRotation > climbDownThreshold) ? 1 : -1;
			lateralMove = new Vector3(inputX, 0, inputY);
			lateralMove = transform.TransformDirection(lateralMove);
            ladderMovement = verticalMove + lateralMove;
			controller.Move(ladderMovement * climbSpeed * Time.deltaTime);
		 
			if(inputY == 1 && !(aSource.isPlaying) && Time.time >= playTime)
			{
				PlayLadderSound();
			}
		 
			if(Input.GetKey(JumpKey)) {
                useLadder = false;
				onLadder = false;
			}
		}
	}
		
	//Ladder Footsteps
	void PlayLadderSound()
	{
		footsteps.PlayLadderSound();
		playTime = Time.time + climbRate;
	}
}