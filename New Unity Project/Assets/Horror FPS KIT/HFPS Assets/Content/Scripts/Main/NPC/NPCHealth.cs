using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : MonoBehaviour {

    [Header("Setup")]
    public Transform Hips;
    public string FleshTag = "Flesh";

    [Header("Character Health")]
    public int Health;
    public AudioClip HtAudio;

    private Rigidbody[] RigidbodyCache;
    public Collider[] ColliderCache;
    private ZombieBehaviour ai;

    [HideInInspector] public bool damageTaken;

    void Awake()
    {
        RigidbodyCache = Hips.GetComponentsInChildren<Rigidbody>();
        ColliderCache = Hips.GetComponentsInChildren<Collider>();
        ai = GetComponent<ZombieBehaviour>();
    }

    void Start()
    {
        Ragdoll(false);

        foreach(Collider c in ColliderCache)
        {
            NPCBodyPart bodyPart = c.gameObject.AddComponent<NPCBodyPart>();
            bodyPart.health = this;
            c.gameObject.tag = FleshTag;
        }
    }

    void Update () {
        if (Health <= 0 || Health <= 0.9)
        {
            Health = 0;
            Ragdoll(true);
        }
    }

    public void Damage(int damage)
    {
        if (Health <= 0) return;
        Health -= damage;

        damageTaken = true;

        if (HtAudio) { AudioSource.PlayClipAtPoint(HtAudio, transform.position, 1.0f); }
    }

    private void Ragdoll(bool enabled)
    {
        ai.StateMachine(!enabled);

        foreach (Rigidbody rb in RigidbodyCache)
        {
            Physics.IgnoreCollision(rb.GetComponent<Collider>(), Camera.main.transform.root.GetComponent<Collider>());

            if (enabled)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.GetComponent<Collider>().isTrigger = false;
            }
            else
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.GetComponent<Collider>().isTrigger = true;
            }
        }
    }
}
