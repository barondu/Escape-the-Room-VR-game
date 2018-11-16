using UnityEngine;

public class ExamineItem : MonoBehaviour {

    public enum type { Object, Paper }
    public type examineType = type.Object;

	public float examineDistance;
	public AudioClip examineSound;

    //Object
	public bool isUsable;
    public string examineObjectName;

    //Paper
	[Multiline]
	public string paperReadText;
    public int textSize;
	public Vector3 paperRotation;

    [HideInInspector] public bool isExamined;
}
