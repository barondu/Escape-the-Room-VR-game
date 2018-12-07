using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankHamer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // this.GetComponent<Rigidbody>().Sleep();
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter(Collision collision)
    {  
        if (collision.collider.tag == "hammer")
        {
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            this.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
