using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordChecked : MonoBehaviour {
    public GameObject text1;
    public GameObject text2;
    public GameObject text3;
    public GameObject text4;
    public GameObject card;
    public GameObject key;
    private Animator anim;
    private bool islocked;
    // Use this for initialization
    void Start () {
        islocked = true;
        anim= gameObject.GetComponent<Animator>();
        anim.SetBool("hasKey", false);
    }
	
	// Update is called once per frame
	void Update () {
		if(islocked && text1.GetComponent<TextMesh>().text == 4.ToString() && text2.GetComponent<TextMesh>().text == 3.ToString() && text3.GetComponent<TextMesh>().text == 9.ToString() && text4.GetComponent<TextMesh>().text == 6.ToString())
        {
            anim.SetBool("hasKey", true);
            key.SetActive(true);
            card.SetActive(true);
            print("open");
            this.GetComponent<AudioSource>().Play();
            islocked = false;
            //GameObject box = GameObject.Find("tresure_box");
            //box.transform.Translate(2.5f, 1.989f, 0.153f);
        }
	}
}
