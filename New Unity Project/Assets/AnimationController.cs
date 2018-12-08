using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
    //public GameObject AnimObject;
    public Animation anim; 
    public string animationName;
    public string animationName1;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayeAnim()
    {
        anim.Play(animationName);
        //anim.Play("");
    }

    public void PlayeAnim1()
    {
        anim.Play(animationName);
        //anim.Play("");
    }
}
