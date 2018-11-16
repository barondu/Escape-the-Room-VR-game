using UnityEngine;
using System.Collections;

public class Heal : MonoBehaviour {

	public float HealAmout;
	public AudioClip HealSound;
	
	private HealthManager hm;

	void Start()
	{
		hm = Camera.main.transform.root.gameObject.GetComponent<HealthManager>();
	}
	
	public void UseObject () {
		hm.ApplyHeal (HealAmout);
		if (!hm.isMaximum) {
			if (HealSound) {
				AudioSource.PlayClipAtPoint (HealSound, transform.position, 1.0f);
			}
			Destroy (gameObject);
		}
	}
}
