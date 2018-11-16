using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupNotification : MonoBehaviour {

	public Text ItemText;
	public Image ItemImage;
	private bool isFaded = false;

	void Update()
	{
		if (isFaded) {
			Destroy (this.gameObject);
		}
	}

	public void SetPickupNotification(string notification)
	{
		ItemText.text = "PICKED UP " + notification.ToUpper();
		StartCoroutine (WaitFade ());
	}

	public void SetNotification(string notification)
	{
		ItemText.text = notification.ToUpper();
		StartCoroutine (WaitFade ());
	}

	public void SetNotificationIcon(string notification, Sprite icon)
	{
		ItemImage.sprite = icon;
		ItemText.text = notification.ToUpper();
		StartCoroutine (WaitFade ());
	}

	IEnumerator WaitFade()
	{
		yield return new WaitForSeconds (1.5f);
		StartCoroutine (FadeOut ());
	}

	IEnumerator FadeOut()
	{
		ItemText.CrossFadeAlpha (0.1f, 0.5f, false);
		ItemImage.CrossFadeAlpha (0.1f, 0.5f, false);
		yield return new WaitForSeconds (0.5f);
		isFaded = true;
	}
}
