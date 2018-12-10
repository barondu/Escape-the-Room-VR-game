using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switcherAudio : MonoBehaviour {
    public AudioSource audioS;
    public GameObject linearmapping;
    private bool key;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (linearmapping.GetComponent<Valve.VR.InteractionSystem.LinearMapping>().value == 1.0f || linearmapping.GetComponent<Valve.VR.InteractionSystem.LinearMapping>().value == 0.0f)
        {
            if (key)
            { PlayAudio(); }
        }
        else { key = true; }


    }
    private void PlayAudio()
    {
        
        audioS.Play();
        key = false;
    }
}
