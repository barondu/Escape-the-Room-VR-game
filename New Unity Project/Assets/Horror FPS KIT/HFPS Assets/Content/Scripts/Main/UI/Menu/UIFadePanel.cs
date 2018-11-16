using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFadePanel : MonoBehaviour {

    public Image FadeImage;
    public float FadeSpeed;
    public bool startFadeOut;

    void Awake()
    {
        FadeImage.gameObject.SetActive(true);
    }

    void Start () {
        UIFader.SetColor(FadeImage.color);

        if (startFadeOut)
        {
            StartCoroutine(UIFader.FadeOut(FadeSpeed));
        }
	}

    public void FadeOut()
    {
        UIFader.SetBlinkTime(0);
        StartCoroutine(UIFader.FadeOut(FadeSpeed));
    }

    public void FadeIn()
    {
        UIFader.SetBlinkTime(0);
        StartCoroutine(UIFader.FadeIn(FadeSpeed));
    }

    public void FadeBlink(float time)
    {
        UIFader.SetBlinkTime(time);
        StartCoroutine(UIFader.FadeIn(FadeSpeed));
    }

    IEnumerator Blink()
    {
        yield return new WaitUntil(() => UIFader.canFadeOut);
        StartCoroutine(UIFader.FadeOut(FadeSpeed));
    }

    void Update () {
        FadeImage.color = UIFader.GetUIColor();
        if (UIFader.isFaded)
        {
            FadeImage.raycastTarget = false;
        }
    }
}
