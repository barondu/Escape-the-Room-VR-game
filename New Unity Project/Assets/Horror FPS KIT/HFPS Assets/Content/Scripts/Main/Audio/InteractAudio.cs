using System;
using UnityEngine;

[Serializable]
public enum AudioType { Once, Switch }

[RequireComponent(typeof(AudioSource))]
public class InteractAudio : MonoBehaviour {

    private AudioSource audioS;

    public AudioType audioType = AudioType.Once;
    public AudioClip switchSound;
    [Range(0, 1)] public float switchVolume;

    private bool isPlaying;

	void Start () {
        audioS = gameObject.GetComponent<AudioSource>();
    }
	
	public void UseObject() {
        switch (audioType)
        {
            case AudioType.Once:
                if (!audioS.isPlaying)
                {
                    audioS.Play();
                }
                break;

            case AudioType.Switch:
                if (isPlaying)
                {
                    if (switchSound) { AudioSource.PlayClipAtPoint(switchSound, transform.position, switchVolume); }
                    audioS.Play();
                    isPlaying = true;
                }
                else
                {
                    if (switchSound) { AudioSource.PlayClipAtPoint(switchSound, transform.position, switchVolume); }
                    audioS.Stop();
                    isPlaying = false;
                }
                break;
        }
	}
}
