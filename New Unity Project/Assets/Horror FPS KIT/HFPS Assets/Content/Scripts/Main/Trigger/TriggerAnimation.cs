using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour {

	public GameObject AnimationObject;
	public AudioClip AnimationSound;
	public float Volume = 0.5f;

    [SaveableField, HideInInspector]
	public bool isPlayed;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !isPlayed) {
			AnimationObject.GetComponent<Animation> ().Play ();
			if(AnimationSound){AudioSource.PlayClipAtPoint(AnimationSound, AnimationObject.transform.position, Volume);}
			isPlayed = true;
		}
	}
}
