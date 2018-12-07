using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzle2 : MonoBehaviour {
    public GameObject teleportpoint;
    public GameObject linearmapping;
    public GameObject text;
    public GameObject environmentLamp;
    public GameObject environmentLight;
    private double[] keyColor = new double[3];
    private int count;
    // Use this for initialization
    void Start () {
        count = 0;
        //GameObject box1 = GameObject.Find("Box02 (3)");
        //GameObject box2 = GameObject.Find("Box02 (4)");
        //teleportpoint1 = box1.transform.Find("TeleportPoint1").gameObject;
        //teleportpoint = box2.transform.Find("TeleportPoint").gameObject;
        //linearmapping = GameObject.Find("LinearMapping_pipe");
        //text = GameObject.Find("text_retry");
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void RecordColor()
    {
        double value = linearmapping.GetComponent<Valve.VR.InteractionSystem.LinearMapping>().value;
        //print(value);
        //print(count);
        
        if (count == 0)
        {
            keyColor[0] = value;
            count++;
            text.GetComponent<TextMesh>().text = "";
        }
        else if (count == 1)
        {
            keyColor[1] = value;
            count++;
        }
        else if (count == 2)
        {
            keyColor[2] = value;
            CheckColorPassword(); 
            count = 0;
        }
        //print(keyColor[0] + "........" + keyColor[1] + "........" + keyColor[2] + "........");
    }

    private void CheckColorPassword()
    {
        if (keyColor[0] > 0.6 && keyColor[0] <= 0.8 && keyColor[1] > 0 && keyColor[1] <= 0.2 && keyColor[2] > 0.2 && keyColor[2] <= 0.4)
        {
            teleportpoint.SetActive(true);
            //environmentLight.SetActive(true);
            environmentLamp.GetComponent<MeshRenderer>().materials[0] = Resources.Load("Mat_Lamp") as Material;
            environmentLight.SetActive(true);

        }
        else
        {
            text.GetComponent<TextMesh>().text = "WRONG!\n TRY AGAIN";
            keyColor[0] = 0;
            keyColor[1] = 0;
            keyColor[2] = 0;
        }
    }
}
