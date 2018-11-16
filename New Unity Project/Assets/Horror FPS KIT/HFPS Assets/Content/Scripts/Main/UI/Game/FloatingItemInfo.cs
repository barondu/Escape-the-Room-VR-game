using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingItemInfo : MonoBehaviour {

	public GameObject ObjectToFollow;

	private Image FloatingIcon;

	private Rect screenRect;
	private bool isObjectOverflowing;
	private bool isVisible;

	void Start () {
		screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
		FloatingIcon = GetComponent<Image> ();
	}
	

	void Update () {
        if (!ObjectToFollow)
        {
            Destroy(this.gameObject);
        }

		if (isVisible && !(ObjectToFollow == null)) {
			if (!screenRect.Contains (FloatingIcon.transform.position)) {
				FloatingIcon.enabled = false;
				isObjectOverflowing = true;
			} else {
				FloatingIcon.enabled = true;
				isObjectOverflowing = false;
			}

			if (ObjectToFollow.GetComponent<Renderer> ().isVisible) {
				isObjectOverflowing = false;
			} else {
				isObjectOverflowing = true;
			}

			if (!isObjectOverflowing) {
				Vector3 screenPos = Camera.main.WorldToScreenPoint (ObjectToFollow.transform.position);
				FloatingIcon.transform.position = screenPos;
			}
		} else {
			FloatingIcon.enabled = false;
		}
	}

	public void SetVisible(bool visible)
	{
		switch (visible) {
		case true:
			isVisible = true;
			break;
		case false:
			isVisible = false;
			break;
		}
	}
		
}