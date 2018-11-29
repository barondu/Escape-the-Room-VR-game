using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable : MonoBehaviour {

    private Light m_light;

    // Use this for initialization
    void Start()
    {
        m_light = GameObject.Find("Light_flashlight").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        m_light.enabled = true;
    }
    void OnTriggerExit(Collider other)
    {
       m_light.enabled = false;
    }
}

