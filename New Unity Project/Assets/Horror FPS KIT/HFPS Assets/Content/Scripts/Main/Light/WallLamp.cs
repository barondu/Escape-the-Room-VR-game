using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLamp : MonoBehaviour {

    public bool onlyMaterial;
	public bool isFilckering;
	public bool isOn;

	void Start()
	{
		if (!isFilckering) {
            if (GetComponent<Animation>())
            {
                GetComponent<Animation>().playAutomatically = false;
                GetComponent<Animation>().Stop();
                GetComponent<Animation>().enabled = false;
            }
		}

		if (isFilckering && isOn) {
			GetComponent<Light> ().GetComponent<Animation> ().wrapMode = WrapMode.Loop;
			GetComponent<Light> ().GetComponent<Animation> ().Play ();
		}
	}

	void Update () {
        if (!onlyMaterial)
        {
            if (GetComponent<Light>().enabled)
            {
                GetComponent<Light>().transform.parent.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
            }
            else
            {
                GetComponent<Light>().transform.parent.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0f, 0f, 0f));
            }
        }
        else
        {
            if (isOn)
            {
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
            }
            else
            {
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0f, 0f, 0f));
            }
        }
	}

	public void LightState(bool State)
	{
		switch (State) {
		case true:
                if (!onlyMaterial)
                {
                    GetComponent<Light>().enabled = true;
                    if (isFilckering)
                    {
                        GetComponent<Light>().GetComponent<Animation>().enabled = true;
                        GetComponent<Light>().GetComponent<Animation>().Play();
                    }
                }
                else
                {
                    isOn = true;
                }
			break;
		case false:
                if (!onlyMaterial)
                {
                    if (isFilckering)
                    {
                        GetComponent<Light>().GetComponent<Animation>().Stop();
                        GetComponent<Light>().GetComponent<Animation>().enabled = false;
                    }
                    GetComponent<Light>().enabled = false;
                }
                else
                {
                    isOn = false;
                }
			break;
		}
	}
}
