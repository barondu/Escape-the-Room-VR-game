using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioZoneTrigger : MonoBehaviour {

	public float TransitionSpeed;
	public AudioSource AmbienceSource;
	public AudioClip TriggerAmbience;
	public float TriggerAmbienceVolume;

	private AudioClip DefaultClip;

	private float fadeIn;
	private float fadeOut;

	private float Volume;
	private float ambienceVolume;
	private float DefaultVolume;

	private bool FadedOut;
	private bool FadedIn;

	private bool StartFadeIn;
	private bool StartFadeOut;

	private bool EnteredTrigger;
	private bool Changed = false;
	private bool completed = true;

	void Start()
	{
        AmbienceSource = Camera.main.transform.root.GetChild(1).GetChild(0).GetComponent<AudioSource>();

        if (!AmbienceSource) return;

		FadedIn = true;
		ambienceVolume = AmbienceSource.volume;
		Volume = ambienceVolume;
		DefaultVolume = ambienceVolume;
		DefaultClip = AmbienceSource.clip;
	}

	void Update()
	{
		if (StartFadeOut && !FadedOut) {
			FadeOut ();
		}
		if (StartFadeIn && !FadedIn) {
			FadeIn ();
		}
		if (!completed && FadedIn) {
			StartCoroutine (ChangeAmbience ());
			completed = true;
		}

        if(AmbienceSource)
		    AmbienceSource.volume = Volume;
	}

	void ChangeClip()
	{
		if (!Changed) {
			AmbienceSource.clip = TriggerAmbience;
			ambienceVolume = TriggerAmbienceVolume;
			Changed = true;
		} else {
			AmbienceSource.clip = DefaultClip;
			ambienceVolume = DefaultVolume;
			Changed = false;
		}
	}

	void FadeOut()
	{
		if (fadeOut > 0) {
			fadeOut -= Time.deltaTime * TransitionSpeed;
			Volume = fadeOut;
		} else if (Volume <= 0) {
			FadedOut = true;
			FadedIn = false;
			StartFadeOut = false;
		}
	}

	void FadeIn()
	{
		if (fadeIn >= 0) {
			fadeIn += Time.deltaTime * TransitionSpeed;
			Volume = fadeIn;
		}
		if (Volume >= ambienceVolume) {
			FadedIn = true;
			FadedOut = false;
			StartFadeIn = false;
		}
	}

	IEnumerator ChangeAmbience()
	{
		fadeOut = ambienceVolume;
		StartFadeOut = true;

		yield return new WaitUntil (() => FadedOut == true);

		ChangeClip ();

		if (!(AmbienceSource.clip == DefaultClip)) {
			yield return new WaitUntil (() => Changed == true);
		} else {
			yield return new WaitUntil (() => Changed == false);
		}

		AmbienceSource.Play ();
		fadeIn = 0;
		StartFadeIn = true;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && AmbienceSource) {
			EnteredTrigger = true;
			if (TriggerAmbience) {
				if (FadedIn) {
					StartCoroutine (ChangeAmbience ());
					completed = true;
				} else {
					completed = false;
				}
			} else {
				FadeOut ();
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Player" && EnteredTrigger && AmbienceSource) {
			if (TriggerAmbience ) {
				if (FadedIn) {
					StartCoroutine (ChangeAmbience ());
					EnteredTrigger = false;
					completed = true;
				} else {
					completed = false;
				}
			} else {
				FadeIn ();
			}
		}
	}
}
