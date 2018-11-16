using System;
using System.Collections;
using UnityEngine;

public static class UIFader {

    private static Color fadeColor;
    private static float blinkTime = 0;

    public static bool canFadeOut;
    public static bool isFaded;

    public static void SetColor(Color color)
    {
        fadeColor = color;
    }

    public static void SetBlinkTime(float time)
    {
        blinkTime = time;
    }

    public static Color GetUIColor()
    {
        return fadeColor;
    }

    public static IEnumerator FadeIn(float fadeSpeed)
    {
        isFaded = false;

        Color color = fadeColor;
        color.a = 0;

        fadeColor = color;

        while (fadeColor.a <= 0.95f)
        {
            color.a += Time.fixedDeltaTime * fadeSpeed;
            fadeColor = color;
            yield return null;
        }

        color.a = 1;
        fadeColor = color;

        isFaded = true;

        if (blinkTime > 0)
        {
            yield return new WaitForSecondsRealtime(blinkTime);
            canFadeOut = true;
        }
    }

    public static IEnumerator FadeOut(float fadeSpeed)
    {
        isFaded = false;
        Color color = fadeColor;

        while (fadeColor.a >= 0.1)
        {
            color.a -= Time.fixedDeltaTime * fadeSpeed;
            fadeColor = color;
            yield return null;
        }

        color.a = 0;
        fadeColor = color;

        canFadeOut = false;
        isFaded = true;
    }
}
