using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAnimation : MonoBehaviour {

    public GameObject AnimationObject;
    public string Animation;
    public float animationSpeed = 1.0f;
    public AudioClip AnimationAudio;

    private bool isInteracted;

    public void Interact()
    {
        if (!isInteracted)
        {
            AnimationObject.GetComponent<Animation>()[Animation].speed = animationSpeed;
            AnimationObject.GetComponent<Animation>().Play(Animation);
            if (AnimationAudio) { AudioSource.PlayClipAtPoint(AnimationAudio, transform.position, 0.75f); }
            isInteracted = true;
        }
    }
}
