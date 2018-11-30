using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaa : MonoBehaviour {
    GameObject text_num;
    public int num = 0;
	// Use this for initialization
	void Start () {
        text_num = GameObject.Find("button_text");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateNumber()
    {
        if(num == 9)
        {
            num = 0;
            text_num.GetComponent<TextMesh>().text = (num).ToString();
        }
        else
        {
            num++;
            text_num.GetComponent<TextMesh>().text = (num).ToString();
            
        }

    }
}
