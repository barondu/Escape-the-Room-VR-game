using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeverAngle : MonoBehaviour {

	public List<GameObject> IgnoreColliders = new List<GameObject>();
	public bool DebugEulerAngle;
	public float angleStop;
	public GameObject SwitcherInteract;
	
	[HideInInspector]
	public bool hold;

	void Start() {
		for (int i = 0; i < IgnoreColliders.Count; i++) {
			Physics.IgnoreCollision(GetComponent<Collider>(), IgnoreColliders[i].GetComponent<Collider>());
		}
	}

	void Update () {
		if (DebugEulerAngle) 
		{
			Debug.Log(this.gameObject.transform.eulerAngles.x);
		}

		if(hold)
		{
			gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		}else{
			gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		}
	}		
}
