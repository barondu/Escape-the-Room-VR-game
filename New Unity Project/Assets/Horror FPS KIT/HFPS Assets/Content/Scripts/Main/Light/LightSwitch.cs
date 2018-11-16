using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour {

	public Electricity electricity;
	public string ElectricityOffHint = "It seems that the electricity is turned off, go outside and turn it on.";
	public Light[] switchLight;

	[Header("Animation")]
	public GameObject SwitchAnim;
	public string AnimSwitchOn;
	public string AnimSwitchOff;

	[Header("Audio")]
	public AudioClip SwitchOn;
	public AudioClip SwitchOff;

	public bool isOn;

	[HideInInspector]
	public bool canSwitchOn;

	void Update()
	{
		if (electricity) {
			canSwitchOn = electricity.isPoweredOn;
		} else {
			canSwitchOn = true;
		}
	}

	public void UseObject()
	{
		if (canSwitchOn) {
			if (!isOn) {
				for (int i = 0; i < switchLight.Length; i++) {
					if (switchLight [i].gameObject.GetComponent<WallLamp> ()) {
						switchLight [i].gameObject.GetComponent<WallLamp> ().LightState (true);		
					}
					switchLight [i].transform.parent.gameObject.GetComponent<MeshRenderer> ().material.SetColor ("_EmissionColor", new Color (1f, 1f, 1f));
					switchLight [i].enabled = true;
				}
				AudioSource.PlayClipAtPoint (SwitchOn, this.transform.position, 0.6f);
				if (SwitchAnim) {
					SwitchAnim.GetComponent<Animation> ().Play (AnimSwitchOn);
				}
				isOn = true;
			} else {
				for (int i = 0; i < switchLight.Length; i++) {
					if (switchLight [i].gameObject.GetComponent<WallLamp> ()) {
						switchLight [i].gameObject.GetComponent<WallLamp> ().LightState (false);
					}
					switchLight [i].transform.parent.gameObject.GetComponent<MeshRenderer> ().material.SetColor ("_EmissionColor", new Color (0f, 0f, 0f));
					switchLight [i].enabled = false;
				}
				AudioSource.PlayClipAtPoint (SwitchOff, this.transform.position, 0.6f);
				if (SwitchAnim) {
					SwitchAnim.GetComponent<Animation> ().Play (AnimSwitchOff);
				}
				isOn = false;
			}
		} else {
			electricity.ElectricityOff (ElectricityOffHint);
		}
	}

    public void OnSave()
    {
        if (GetComponent<SaveHelper>())
        {
            GetComponent<SaveHelper>().SetArray(new Dictionary<string, object>
            {
                { "isOn", isOn }
            });
        }
    }

    public void OnLoad(Newtonsoft.Json.Linq.JObject token)
    {
        isOn = (bool)token["isOn"];

        for (int i = 0; i < switchLight.Length; i++)
        {
            if (switchLight[i].gameObject.GetComponent<WallLamp>())
            {
                switchLight[i].gameObject.GetComponent<WallLamp>().LightState(isOn);
            }

            switchLight[i].transform.parent.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
            switchLight[i].enabled = isOn;
        }
    }
}
