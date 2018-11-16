using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    public List<TabButton> tabButtons = new List<TabButton>();

    [Space(5)]
    public bool holdColor;

    [Header("Button Colors")]
    public Color NormalColor = Color.white;
    public Color HoverColor = Color.white;
    public Color PressedColor = Color.white;
    public Color HoldColor = Color.white;

    [Header("Text Colors")]
    public Color TextNormalColor = Color.white;
    public Color TextHoverColor = Color.white;
    public Color TextPressedColor = Color.white;
    public Color TextHoldColor = Color.white;

    private Image btnImg;
    private Text btnTxt;

    void Awake()
    {
        if (GetComponent<Image>())
        {
            btnImg = GetComponent<Image>();
            btnImg.color = NormalColor;
        }

        if (transform.childCount > 0 && transform.GetChild(0).GetComponent<Text>())
        {
            btnTxt = transform.GetChild(0).GetComponent<Text>();
            btnTxt.color = TextNormalColor;
        }

        if (holdColor)
        {
            btnImg.color = HoldColor;
            btnTxt.color = TextHoldColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (holdColor)
        {
            return;
        }

        btnImg.color = PressedColor;
        btnTxt.color = TextPressedColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        holdColor = true;

        foreach (TabButton bt in tabButtons)
        {
            if (bt != this)
            {
                bt.Unhold();
            }
        }

        btnImg.color = HoldColor;
        btnTxt.color = TextHoldColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (holdColor)
        {
            return;
        }

        btnImg.color = HoverColor;
        btnTxt.color = TextHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (holdColor)
        {
            return;
        }

        btnImg.color = NormalColor;
        btnTxt.color = TextNormalColor;
    }

    public void Unhold()
    {
        holdColor = false;
        btnImg.color = NormalColor;
        btnTxt.color = TextNormalColor;
    }
}
