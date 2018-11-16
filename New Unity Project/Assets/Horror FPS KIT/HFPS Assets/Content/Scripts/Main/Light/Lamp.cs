using UnityEngine;

public enum m_lampType { Static, Normal, Flickering }

public class Lamp : MonoBehaviour {

    public m_lampType LampType = m_lampType.Normal;
	public Light switchLight;
    public Animation animationObject;
    public GameObject EmissionObject;
	public bool isOn;

	[Header("Audio")]
	public AudioClip SwitchOn;
	public AudioClip SwitchOff;

	[HideInInspector]
	public bool canSwitchOn = true;

    void Awake()
    {
        if (!EmissionObject)
        {
            EmissionObject = gameObject;
        }
    }

    void Start()
    {
        if (LampType == m_lampType.Flickering)
        {
            if (isOn)
            {
                animationObject.wrapMode = WrapMode.Loop;
                animationObject.Play();
            }
        }

        switchLight.enabled = isOn;
    }

    void Update()
    {
        if (LampType == m_lampType.Static) return;

        if (switchLight.enabled)
        {
            EmissionObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
        }
        else
        {
            EmissionObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0f, 0f, 0f));
        }
    }

    public void UseObject()
	{
		if (!isOn) {
            SwitchLamp(true);
        } else {
            SwitchLamp(false);
        }
	}

    public void SwitchLamp(bool LampState)
    {
        if (LampState)
        {
            if (LampType == m_lampType.Flickering)
            {
                animationObject.wrapMode = WrapMode.Loop;
                animationObject.Play();
            }

            switchLight.enabled = true;
            isOn = true;

            if (SwitchOn) { AudioSource.PlayClipAtPoint(SwitchOn, transform.position, 0.75f); }
        }
        else
        {
            if (LampType == m_lampType.Flickering)
            {
                animationObject.Stop();
            }

            switchLight.enabled = false;
            isOn = false;

            if (SwitchOff) { AudioSource.PlayClipAtPoint(SwitchOff, transform.position, 0.75f); }
        }
    }

    public void OnLoad()
    {
        if (LampType == m_lampType.Flickering)
        {
            if (isOn)
            {
                animationObject.wrapMode = WrapMode.Loop;
                animationObject.Play();
            }
        }
    }
}
