using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Keypad : MonoBehaviour {

    [Header("Materials")]
    public MeshRenderer LedRed;
    public Material LedRedOn;
    public MeshRenderer LedGreen;
    public Material LedGreenOn;
    public Material LedOff;

    [Header("Setup")]
	public int AccessCode;
	public TextMesh AccessCodeText;
	private MeshRenderer textRenderer;

	[Header("Sounds")]
	public AudioClip enterCode;
	public AudioClip accessGranted;
	public AudioClip accessDenied;

	[Space(15)]
	public UnityEvent OnAccessGranted;
	[Space(7)]
	public UnityEvent OnAccessDenied;

	private string numberInsert = "";
	private bool confirmCode;
	private bool enableInsert = true;

    [HideInInspector]
    public bool m_accessGranted = false;

	void Start()
	{
		textRenderer = AccessCodeText.gameObject.GetComponent<MeshRenderer> ();
	}

	public void InsertCode(int number)
	{
		if (!(numberInsert.Length >= AccessCode.ToString ().Length) && enableInsert && number != 10 && number != 11) {
			numberInsert = numberInsert + number;
			if(enterCode){AudioSource.PlayClipAtPoint(enterCode, Camera.main.transform.position);}
		}
        if (!string.IsNullOrEmpty(numberInsert))
        {
            switch (number)
            {
                case 10:
                    // Back
                    if (numberInsert.Length > 0)
                    {
                        numberInsert = numberInsert.Remove(numberInsert.Length - 1);
                        if (enterCode) { AudioSource.PlayClipAtPoint(enterCode, Camera.main.transform.position); }
                    }
                    break;
                case 11:
                    // Confirm Code
                    confirmCode = true;
                    break;
            }
        }
	}

	void Update () {
		if (enableInsert) {
			textRenderer.material.SetColor ("_Color", Color.white);
			AccessCodeText.text = numberInsert;
		}

		if (numberInsert == AccessCode.ToString () && confirmCode) {
			OnAccessGranted.Invoke ();
            LedRed.material = LedOff;
            LedGreen.material = LedGreenOn;
            confirmCode = false;
			enableInsert = false;
			numberInsert = "";
            m_accessGranted = true;
            StartCoroutine (WaitGranted ());
		} else if(confirmCode) {
			OnAccessDenied.Invoke ();
            //LedGreen.material = LedOff;
            //LedRed.material = LedRedOn;
            confirmCode = false;
			enableInsert = false;
            m_accessGranted = false;
            numberInsert = "";
			StartCoroutine (WaitDenied ());
		}
	}

    public void SetAccessGranted()
    {
        LedRed.material = LedOff;
        LedGreen.material = LedGreenOn;
        confirmCode = false;
        enableInsert = false;
        numberInsert = "";
        m_accessGranted = true;
    }

	IEnumerator WaitGranted()
	{
		if(accessGranted){AudioSource.PlayClipAtPoint(accessGranted, Camera.main.transform.position);}
		textRenderer.material.SetColor ("_Color", Color.green);
		AccessCodeText.text = "GRANTED";
		yield return new WaitForSeconds (1);
		enableInsert = true;
	}

	IEnumerator WaitDenied()
	{
		if(accessDenied){AudioSource.PlayClipAtPoint(accessDenied, Camera.main.transform.position);}
		textRenderer.material.SetColor ("_Color", Color.red);
		AccessCodeText.text = "DENIED";
		yield return new WaitForSeconds (1);
		enableInsert = true;
	}
}
