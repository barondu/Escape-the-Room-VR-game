using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class DynamicObjectPlank : MonoBehaviour {

    public float strenght;
    public AudioClip woodCrack;

    private Rigidbody plankRB;
    private GameObject player;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        plankRB = GetComponent<Rigidbody>();
        plankRB.isKinematic = true;
        plankRB.useGravity = false;
    }

    void Start()
    {
        player = Camera.main.transform.root.gameObject;
    }

    public void UseObject()
    {
        if (!plankRB) return;

        plankRB.isKinematic = false;
        plankRB.useGravity = true;

        audioSource.PlayOneShot(woodCrack);

        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>());

        plankRB.AddForce(-Camera.main.transform.forward * strenght * 10, ForceMode.Force);
        gameObject.tag = "Untagged";
        gameObject.layer = 0;

        Destroy(this);
    }
}
