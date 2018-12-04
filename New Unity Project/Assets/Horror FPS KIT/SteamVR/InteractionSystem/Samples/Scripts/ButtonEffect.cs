//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


namespace Valve.VR.InteractionSystem.Sample
{
    public class ButtonEffect : MonoBehaviour
    {
        public int num = 0;
        public void OnButtonDown(Hand fromHand)
        {
            string buttonname = gameObject.name;
            switch(buttonname)
            {
                case "Button1":
                    GameObject text1 = GameObject.Find("button_text1");
                    if (num == 9)
                    {
                        num = 0;
                        text1.GetComponent<TextMesh>().text = (num).ToString();
                    }
                    else
                    {
                        num++;
                        text1.GetComponent<TextMesh>().text = (num).ToString();
                    }
                    break;
                case "Button2":
                    GameObject text2 = GameObject.Find("button_text2");
                    if (num == 9)
                    {
                        num = 0;
                        text2.GetComponent<TextMesh>().text = (num).ToString();
                    }
                    else
                    {
                        num++;
                        text2.GetComponent<TextMesh>().text = (num).ToString();
                    }
                    break;
                case "Button3":
                    GameObject text3 = GameObject.Find("button_text3");
                    if (num == 9)
                    {
                        num = 0;
                        text3.GetComponent<TextMesh>().text = (num).ToString();
                    }
                    else
                    {
                        num++;
                        text3.GetComponent<TextMesh>().text = (num).ToString();
                    }
                    break;
                case "Button4":
                    GameObject text4 = GameObject.Find("button_text4");
                    if (num == 9)
                    {
                        num = 0;
                        text4.GetComponent<TextMesh>().text = (num).ToString();
                    }
                    else
                    {
                        num++;
                        text4.GetComponent<TextMesh>().text = (num).ToString();
                    }
                    break;
            }
            fromHand.TriggerHapticPulse(1000);
        }

        public void OnButtonUp(Hand fromHand)
        {
            ColorSelf(Color.white);
        }

        private void ColorSelf(Color newColor)
        {
            Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                renderers[rendererIndex].material.color = newColor;
            }
        }
    }
}