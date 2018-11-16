using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveNotification : MonoBehaviour {

    public GameObject saveNotification;

    private bool isFaded;
    private Image s_image;
    private Text s_text;

	void Start () {
        saveNotification.SetActive(false);
        isFaded = false;
    }
	
    public void ShowSaveNotification(float time)
    {
        if(!isFaded)
        saveNotification.SetActive(true);
        FadeIn(time);
    }

    IEnumerator FadeIn(float t)
    {
        s_image.CrossFadeAlpha(1f, 0.5f, false);
        s_text.CrossFadeAlpha(1f, 0.5f, false);
        yield return new WaitForSeconds(t);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        s_image.CrossFadeAlpha(0.1f, 0.5f, false);
        s_text.CrossFadeAlpha(0.1f, 0.5f, false);
        saveNotification.SetActive(false);
        isFaded = true;
        yield return null;
    }
}
