using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooksShelfController : MonoBehaviour {
    //public GameObject AnimObject;
    public Animation anim;
    public string animationName;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayeBooksShelfAnim()
    {
        anim.Play(animationName);
        //anim.Play("");
    }
}
