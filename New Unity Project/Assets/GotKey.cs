using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotKey : MonoBehaviour {

    public GameObject text;
    public GameObject targetDoor;

    // Use this for initialization
    void Start () {
        //text = GameObject.Find("text_got_key");
    }
	
	// Update is called once per frame
	void Update () {
        
    }
    public void GotTheKey()
    {
        text.GetComponent<TextMesh>().text = "YOU GOT THE KEY!";
        targetDoor.GetComponent<UnlockDoor>().setIsHasKey(true);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

}
