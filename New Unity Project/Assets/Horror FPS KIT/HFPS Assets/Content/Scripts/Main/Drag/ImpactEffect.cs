using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
public class ImpactEffect : MonoBehaviour
{
    public AudioClip[] impactSounds;
    public float impactDetectAt;

    [HideInInspector] public bool Impact;

    private bool canHit;

    void Start()
    {
        canHit = false;
        StartCoroutine(AntiSpam());
    }

    void OnCollisionEnter(Collision col)
    {
        if (canHit)
        {
            float volumeMultipler = Mathf.Clamp(col.relativeVelocity.magnitude * 0.1f, 0, 1);

            if (impactSounds.Length > 0)
            {
                int randomImpact = Random.Range(0, impactSounds.Length);
                GetComponent<AudioSource>().PlayOneShot(impactSounds[randomImpact], volumeMultipler);
            }

            canHit = false;
            Impact = volumeMultipler >= impactDetectAt;
            StartCoroutine(AntiSpam());
            StartCoroutine(ImpactDetect());
        }
    }

    IEnumerator AntiSpam()
    {
        yield return new WaitForSeconds(1);
        canHit = true;
    }

    IEnumerator ImpactDetect()
    {
        yield return new WaitForSeconds(3);
        Impact = false;
    }
}