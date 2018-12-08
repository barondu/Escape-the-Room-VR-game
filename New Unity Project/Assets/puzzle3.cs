using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzle3 : MonoBehaviour {
    public bool liar;
    public bool murder;
    public GameObject Box;
    private bool isSelected, isSelected_Murder;
    public GameObject HintLight;

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        this.GetComponent<AudioSource>().Play();
        string currentstate = Box.GetComponent<puzzle3Box>().GetState();
        print(currentstate + "          caonimamaamamamaam");
        if (collision.collider.tag == "apple")
        {

            switch (currentstate)
            {
                case "initial":

                    if (!isSelected)
                    {
                        isSelected = true;
                        TurnOnTheHintLight();
                    }
                    else
                    {
                        isSelected = false;
                        TurnOffTheHintLight();
                    }

                    break;
                case "liar":
                    if (!isSelected_Murder)
                    {
                        isSelected_Murder = true;
                    }
                    else
                    {
                        isSelected_Murder = false;
                    }
                    break;
            }
        }


    }
    public void setLightColor(Color color)
    {
        HintLight.GetComponent<Light>().color = color;
    }

    public void TurnOnTheHintLight()
    {
        HintLight.SetActive(true);
        HintLight.GetComponent<AudioSource>().Play();
    }

    public void TurnOffTheHintLight()
    {
        HintLight.SetActive(false);
        HintLight.GetComponent<AudioSource>().Play();
    }

    public bool IsChooseRight()
    {
        if ((isSelected && liar)||(!isSelected && !liar))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CheckMurderer()
    {
        if (murder && isSelected_Murder)
        {
            return true;
        }
        else { return false; }
    }
    public void Resetall()
    {

        isSelected = false;
        isSelected_Murder = false;
        TurnOffTheHintLight();
        setLightColor(new Color(1.0f, 1.0f, 1.0f));
    }
    public bool GetisSelected_Murder()
    {
        return isSelected_Murder;
    }
}
