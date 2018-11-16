using UnityEngine;

public class Spinner : MonoBehaviour {

    public float RotateSpeed;
    [HideInInspector]
    public bool isSpinning;

    private RectTransform rectTransform;

    void Awake()
    {
        isSpinning = true;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update () {
        if (isSpinning)
        {
            rectTransform.Rotate(0, 0, -RotateSpeed * Time.deltaTime);
        }
    }
}
