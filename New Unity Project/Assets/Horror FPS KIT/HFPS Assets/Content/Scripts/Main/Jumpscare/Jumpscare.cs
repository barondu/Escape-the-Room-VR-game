/* Jumpscare.cs - Jumpscare Manager */

using UnityEngine;

public class Jumpscare : MonoBehaviour {

	private JumpscareEffects effects;

	[Header("Jumpscare Setup")]
	public Animation AnimationObject;
	public AudioClip AnimationSound;
	public float SoundVolume = 0.5f;

	[Tooltip("Value sets how long will be player scared.")]
	public float ScareLevelSec = 33f;

    [SaveableField, HideInInspector]
	public bool isPlayed;

	void Start()
	{
		effects = Camera.main.transform.parent.transform.parent.gameObject.GetComponent<JumpscareEffects> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !isPlayed) {
			AnimationObject.Play ();
			if(AnimationSound){AudioSource.PlayClipAtPoint(AnimationSound, Camera.main.transform.position, SoundVolume);}
			effects.Scare (ScareLevelSec);
			isPlayed = true;
		}
	}
}
