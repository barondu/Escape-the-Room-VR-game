using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour {

	private HFPS_GameManager gameManager;

	public GameObject LampIndicator;

	[HideInInspector]
	public bool isPoweredOn;

	void Start()
	{
        gameManager = GetComponent<HFPS_GameManager> ();
	}

	public void ElectricityOff(string Hint)
	{
        gameManager.ShowHint (Hint);
	}

	public void SwitcherUp()
	{
		isPoweredOn = true;
		if (LampIndicator) {
			LampIndicator.GetComponent<MeshRenderer> ().material.SetColor ("_EmissionColor", new Color (1f, 1f, 1f));
			LampIndicator.transform.GetChild (0).gameObject.GetComponent<Light> ().enabled = true;
		}
	}

	public void SwitcherDown()
	{
		isPoweredOn = false;
		if (LampIndicator) {
			LampIndicator.GetComponent<MeshRenderer> ().material.SetColor ("_EmissionColor", new Color (0f, 0f, 0f));
			LampIndicator.transform.GetChild (0).gameObject.GetComponent<Light> ().enabled = false;
		}
	}
}