using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadButton : MonoBehaviour {

	public int number;
	private Keypad keypad;

	void Start()
	{
		keypad = transform.parent.GetComponent<Keypad> ();
	}

	public void UseObject () {
        if(!keypad.m_accessGranted)
		    keypad.InsertCode (number);
	}
}
