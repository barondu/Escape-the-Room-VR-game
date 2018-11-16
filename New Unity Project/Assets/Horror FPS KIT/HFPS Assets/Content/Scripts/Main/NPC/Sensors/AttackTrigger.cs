using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {

    [HideInInspector]
    public bool PlayerInTrigger;

    [HideInInspector]
    public bool Enabled = true;

    private void Update()
    {
        if (!Enabled)
        {
            PlayerInTrigger = false;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player" && Enabled)
        {
            PlayerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && Enabled)
        {
            PlayerInTrigger = false;
        }
    }
}
