using UnityEngine;
using System.Collections;
using rnd = ThunderWire.Helper.Random;

public class Footsteps : MonoBehaviour
{
	[Header("Main")]
    public AudioSource soundsGO;

    private CharacterController controller;
    private PlayerController playerController;
    private rnd.Random rand = new rnd.Random();

    [System.Serializable]
	public class footsteps
	{
		public string groundTag;
		public AudioClip[] footstep;
	}

	[Header("Player Footsteps")]
	[Tooltip("Element 0 is always Untagged/Concrete and 1 is ladder")]
	public footsteps[] m_Footsteps;

    [Space(5)]
    [Header("Audio Volume")]
	public float audioVolumeCrouch = 0.1f;
	public float audioVolumeWalk = 0.2f;
	public float audioVolumeRun = 0.3f;
    public float audioVolumeWater = 0.3f;

    [Header("Audio Step Lenght")]
    //Walk Step Settings
    public float stepLengthWalk = 0.75f;
    public bool realStepWalk;
    public float extraWalkLenght;

    //Run Step Settings
    public float stepLengthRun = 0.3f;
    public bool realStepRun;
    public float extraRunLenght;

    //Crouch Step Settings
    public float stepLengthCrouch = 0.75f;
    public bool realStepCrouch;
    public float extraCrouchLenght;

    [Header("Water Settings")]
    public string WaterFootstepTag = "Water";

    private bool step = true;
    private string curMat;
    private float speed;

    [HideInInspector]
    public bool inWater;
    private bool isRemovingFoam;

    void Awake()
    {
        step = true;
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        speed = controller.velocity.magnitude;
        float walkSpeed = playerController.walkSpeed - 0.5f;
        float runSpeed = playerController.runSpeed - 0.5f;
        float crouchSpeed = playerController.crouchSpeed - 0.5f;
        float walkWaterSpeed = playerController.inWaterSpeed - 0.5f;

        curMat = hit.gameObject.tag;

        if (playerController.state != 2 && step)
        {
            if (controller.isGrounded && hit.normal.y > 0.3f)
            {
                for (int i = 0; i < m_Footsteps.Length; i++)
                {
                    if (m_Footsteps[i].footstep.Length > 0 && !inWater)
                    {
                        if (curMat == m_Footsteps[i].groundTag)
                        {
                            if (playerController.state == 0)
                            {
                                if (!playerController.run && speed >= walkSpeed)
                                {
                                    StartCoroutine(WalkOnGround());
                                }
                                if (playerController.run && speed >= runSpeed && speed > walkSpeed)
                                {
                                    StartCoroutine(RunOnGround());
                                }
                            }
                            else
                            {
                                if (speed >= crouchSpeed && speed < walkSpeed)
                                {
                                    StartCoroutine(CrouchOnGrouund());
                                }
                            }
                        }
                    }
                }
            }
        }
        if (playerController.emiter)
        {
            if (inWater && !playerController.onLadder)
            {
                if (playerController.state == 0 && step)
                {
                    if (speed >= walkWaterSpeed)
                    {
                        StartCoroutine(WalkOnWater());
                    }
                }

                if (GetComponent<CharacterController>().velocity.magnitude > 1f)
                {
                    if (playerController.emiter.isStopped)
                        playerController.emiter.Play(true);
                }
                else
                {
                    if (playerController.emiter.isPlaying)
                        playerController.emiter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
            else if (!isRemovingFoam)
            {
                playerController.emiter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                StartCoroutine(RemoveFoam());
                isRemovingFoam = true;
            }
        }
    }

    IEnumerator RemoveFoam()
    {
        yield return new WaitForSeconds(2);
        Destroy(playerController.emiter.gameObject);
        isRemovingFoam = false;
    }
	
	//Ladder footsteps
	public void PlayLadderSound()
	{
		soundsGO.PlayOneShot(m_Footsteps[1].footstep[rand.Range(0, m_Footsteps[1].footstep.Length)], audioVolumeWalk);
	}

    public IEnumerator JumpLand()
    {
        if (!soundsGO.enabled) yield break;

		for (int i = 0; i < m_Footsteps.Length; i++)
		{
			if (curMat == m_Footsteps[i].groundTag) 
			{
				soundsGO.PlayOneShot(m_Footsteps[i].footstep[rand.Range(0, m_Footsteps[i].footstep.Length)], 0.5f);
				yield return new WaitForSeconds(0.12f);
				soundsGO.PlayOneShot(m_Footsteps[i].footstep[rand.Range(0, m_Footsteps[i].footstep.Length)], 0.4f);
			}
		}
    }

	IEnumerator WalkOnGround()
	{
		for (int i = 0; i < m_Footsteps.Length; i++)
		{
			if (curMat == m_Footsteps[i].groundTag)
			{
				step = false;
				soundsGO.PlayOneShot(m_Footsteps[i].footstep[rand.Range(0, m_Footsteps[i].footstep.Length)], audioVolumeWalk);
                if (!realStepWalk)
                {
                    yield return new WaitForSeconds(stepLengthWalk);
                }
                else
                {
                    yield return new WaitUntil(() => !soundsGO.isPlaying);
                    if (extraWalkLenght > 0)
                    {
                        yield return new WaitForSeconds(extraWalkLenght);
                    }
                }
				step = true;
			}
		}
	}

    IEnumerator RunOnGround()
	{
		for (int i = 0; i < m_Footsteps.Length; i++)
		{
			if (curMat == m_Footsteps[i].groundTag)
			{
				step = false;
				soundsGO.PlayOneShot(m_Footsteps[i].footstep[rand.Range(0, m_Footsteps[i].footstep.Length)], audioVolumeRun);
                if (!realStepRun)
                {
                    yield return new WaitForSeconds(stepLengthRun);
                }
                else
                {
                    yield return new WaitUntil(() => !soundsGO.isPlaying);
                    if (extraRunLenght > 0)
                    {
                        yield return new WaitForSeconds(extraRunLenght);
                    }
                }
                step = true;
			}
		}
	}

    IEnumerator CrouchOnGrouund()
    {
        for (int i = 0; i < m_Footsteps.Length; i++)
        {
            if (curMat == m_Footsteps[i].groundTag)
            {
                step = false;
                soundsGO.PlayOneShot(m_Footsteps[i].footstep[rand.Range(0, m_Footsteps[i].footstep.Length)], audioVolumeCrouch);
                if (!realStepCrouch)
                {
                    yield return new WaitForSeconds(stepLengthCrouch);
                }
                else
                {
                    yield return new WaitUntil(() => !soundsGO.isPlaying);
                    if (extraCrouchLenght > 0)
                    {
                        yield return new WaitForSeconds(extraCrouchLenght);
                    }
                }
                step = true;
            }
        }
    }

    IEnumerator WalkOnWater()
    {
        for (int i = 0; i < m_Footsteps.Length; i++)
        {
            if (m_Footsteps[i].groundTag == WaterFootstepTag)
            {
                step = false;
                soundsGO.PlayOneShot(m_Footsteps[i].footstep[rand.Range(0, m_Footsteps[i].footstep.Length)], audioVolumeWater);
                yield return new WaitUntil(() => !soundsGO.isPlaying);
                step = true;
            }
        }
    }
}