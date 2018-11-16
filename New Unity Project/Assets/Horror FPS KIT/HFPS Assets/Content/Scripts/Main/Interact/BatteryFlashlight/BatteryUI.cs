/*
BatteryNGUI.cs by ThunderWire Games
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatteryUI : MonoBehaviour {

	public ScriptManager scriptManager;
	private HFPS_GameManager gameManager;
	private FlashlightScript Flashlight;

	[Header("Setup")]
	//private GameObject BatteryLabel;
	public AudioClip ReloadBatteriesSound;
	public KeyCode BatteryReloadKey = KeyCode.B;

	[Header("Batteries Quantity")]
	public int MinBatteries = 0;
	public int MaxBatteries = 5;

	[Header("Messages")]
	public string MaxBatteryText = "You have Max Batteries";
	public string PickupText = "Battery +1";

	private int Batteries;

	[HideInInspector]
	public bool canPickup;

	void Start()
	{
		Flashlight = GetComponent<FlashlightScript>();
        gameManager = scriptManager.GetScript<HFPS_GameManager>();
	}

	public void AddBattery(int quantity)
	{
		if (canPickup) {
			Batteries += quantity;
            gameManager.AddPickupMessage (PickupText);
		} else {
            gameManager.WarningMessage (MaxBatteryText);
		}
	}
	
	void Update () {
		if (Flashlight.FlashlightGO.activeSelf) {
			if (Input.GetKeyDown (BatteryReloadKey) && Batteries > 0 && Batteries <= 5) {
				if (Flashlight.batteryPercentage < 90.0f) {
					Flashlight.batteryPercentage = 100;
					Batteries --;
					if (ReloadBatteriesSound) {
						AudioSource.PlayClipAtPoint (ReloadBatteriesSound, transform.position, 0.75f);
					}
				}
			}
		}

	 	//Text Battery = BatteryLabel.GetComponent<Text>();

		Batteries = Mathf.Clamp(Batteries, 0, MaxBatteries);
		
	    if (Batteries <= MinBatteries)
		{
			Batteries = MinBatteries;
			//Battery.text = "0 / 5";
			canPickup = true;
		}
		
	    else if (Batteries <= 1 && Batteries > 0)
		{
			//Battery.text = "1 / 5";
			canPickup = true;
		}				
		
	    else if (Batteries <= 2 && Batteries > 1)
		{
			//Battery.text = "2 / 5";
			canPickup = true;
		}				
		
	    else if (Batteries <= 3 && Batteries > 2)
		{
			//Battery.text = "3 / 5";
			canPickup = true;
		}				
		
	    else if (Batteries <= 4 && Batteries > 3)
		{
			//Battery.text = "4 / 5";
			canPickup = true;
		}				
		
	    else if (Batteries <= 5 && Batteries > 4)
		{
			//Battery.text = "5 / 5";
			canPickup = false;
		}
		
		//Setting for a max batteries
	    else if(Batteries > 5)
		{
            Batteries = MaxBatteries;
			canPickup = false;
        }
	}
}