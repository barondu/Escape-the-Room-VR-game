using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour {

    public float radius;
    public string PlayerTag;
    public string ZombieTag;

    [HideInInspector] public bool InTrigger;
    [HideInInspector] public bool zombieInTrigger;


    void Update()
    {
        RaycastHit[] hit = Physics.SphereCastAll(transform.position, radius, transform.forward, radius);

        foreach (var i in hit)
        {
            if (i.collider.gameObject.tag == PlayerTag)
            {
                InTrigger = true;
            }
            else if(i.collider.gameObject.tag == ZombieTag)
            {
                zombieInTrigger = true;
            }
        }
    }

    void LateUpdate()
    {
        InTrigger = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
