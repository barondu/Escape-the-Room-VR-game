using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixpassword : MonoBehaviour
{

    private Vector3 ori_position;
    public GameObject ori;

    // Use this for initialization
    void Start()
    {
        ori_position = ori.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ori.transform.position = ori_position;
    }
}
