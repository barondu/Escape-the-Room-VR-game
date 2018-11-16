using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour {

    public enum modes { Once, MoreTimes }

    public modes Mode = modes.Once;
    [Space(5)]
    public UnityEvent triggerEvent;

    [SaveableField, HideInInspector]
    public bool isPlayed;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isPlayed)
        {
            triggerEvent.Invoke();
            isPlayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isPlayed && Mode == modes.MoreTimes)
        {
            isPlayed = false;
        }
    }
}
