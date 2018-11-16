using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class JumpscareEffects : MonoBehaviour {

	private PostProcessingBehaviour processingBehaviour;
    private PostProcessingProfile processingProfile;

    private ChromaticAberrationModel.Settings chromatic;
    private VignetteModel.Settings vignette;

	private GameObject CameraShake;

    [Header("Scare Effects Settings")]
    public string scareAnimation;
	public float scareChromaticAberration;
	public float scareVignette;

    [Header("Lerps")]
    public float scareLerp;
    public float chromaticLerp;
    public float vignetteLerp;

	private float LerpSpeed = 1f;
	private float ScareWaitSec;
	private float defaultVolume;

	private AudioSource PlayerBreath;
	private bool isFeelingBetter;
	private bool Effects;


	void Start () {
        processingBehaviour = Camera.main.gameObject.GetComponent<PostProcessingBehaviour>();
        processingProfile = processingBehaviour.profile;

        chromatic = processingProfile.chromaticAberration.settings;
        vignette = processingProfile.vignette.settings;

        CameraShake = Camera.main.transform.root.GetChild (0).gameObject;
		PlayerBreath = transform.root.GetChild (1).transform.GetChild (1).gameObject.GetComponent<AudioSource> ();
		defaultVolume = PlayerBreath.volume;
	}

	void Update()
	{
        if (isFeelingBetter) {
			if (PlayerBreath.volume > 0.01f) {
				PlayerBreath.volume = Mathf.Lerp (PlayerBreath.volume, 0f, LerpSpeed * Time.deltaTime);
			}
			if(PlayerBreath.volume <= 0.01f){
				PlayerBreath.Stop ();
				StopCoroutine (ScareBreath ());
				StopCoroutine (WaitEffects ());
				isFeelingBetter = false;
			}
		}

        if (Effects) {
			if (chromatic.intensity <= scareChromaticAberration) {
                chromatic.intensity = Mathf.Lerp (chromatic.intensity, scareChromaticAberration, scareLerp * Time.deltaTime);
			}
			if (vignette.intensity <= scareVignette) {
				vignette.intensity = Mathf.Lerp (vignette.intensity, scareVignette, scareLerp * Time.deltaTime);
			}
		} else {
			if (chromatic.intensity >= 0.015f) {
                chromatic.intensity = Mathf.Lerp (chromatic.intensity, 0f, chromaticLerp * Time.deltaTime);
			} else {
                chromatic.intensity = 0;
			}
			if (vignette.intensity >= 0.015f) {
				vignette.intensity = Mathf.Lerp (vignette.intensity, 0f, vignetteLerp * Time.deltaTime);
			} else {
				vignette.intensity = 0;
			}
		}

        processingProfile.chromaticAberration.settings = chromatic;
        processingProfile.vignette.settings = vignette;
	}

	public void Scare(float sec)
	{
		CameraShake.GetComponent<Animation> ().Play (scareAnimation);
		ScareWaitSec = sec;
		Effects = true;
        StartCoroutine (ScareBreath ());
		StartCoroutine (WaitEffects ());
	}

    IEnumerator WaitEffects()
	{
		yield return new WaitForSeconds (5f);
		Effects = false;
	}

	IEnumerator ScareBreath()
	{
		PlayerBreath.volume = defaultVolume;
		PlayerBreath.Play();
		yield return new WaitForSeconds (ScareWaitSec);
		isFeelingBetter = true;
	}
}
