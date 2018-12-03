using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordChecked : MonoBehaviour {
    GameObject text1;
    GameObject text2;
    GameObject text3;
    GameObject text4;
    // Use this for initialization
    void Start () {
        text1 = GameObject.Find("button_text1");
        text2 = GameObject.Find("button_text2");
        text3 = GameObject.Find("button_text3");
        text4 = GameObject.Find("button_text4");
    }
	
	// Update is called once per frame
	void Update () {
		if(text1.GetComponent<TextMesh>().text == 4.ToString() && text2.GetComponent<TextMesh>().text == 3.ToString() && text3.GetComponent<TextMesh>().text == 9.ToString() && text4.GetComponent<TextMesh>().text == 6.ToString())
        {
            print("gfaegegewrghrheshethjhrh");
            GameObject box = GameObject.Find("tresure_box");
            box.transform.Translate(2.5f, 1.989f, 0.153f);
        }
	}
}
