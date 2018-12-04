using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headCollision : MonoBehaviour
{

    private Vector3 lastPosition;
    private Vector3 recentPosition;
    private Vector3 lockedLastPosition;
    public GameObject Player;
    public GameObject VRCamera;
    private bool coll = false;
    private bool start_game = false;

    // Use this for initialization
    void Start()
    {
        recentPosition = VRCamera.transform.position;
        lastPosition = VRCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!start_game)
        {
            start_game = true;
        }
        if (Math.Abs(recentPosition.x - lastPosition.x) > 0.05 || Math.Abs(recentPosition.z - lastPosition.z) > 0.05)
        {
            if (!coll) lastPosition = VRCamera.transform.position;
            else
            {
                if (Math.Abs(recentPosition.x - lockedLastPosition.x) > 0.1 || Math.Abs(recentPosition.z - lockedLastPosition.z) > 0.1) //update the position
                {
                    lastPosition = VRCamera.transform.position;
                    coll = false;
                }
            }
        }

        recentPosition = VRCamera.transform.position;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
           // Debug.Log("Collid with wall:" + this.transform.position);
            Player.transform.position -= 2 * new Vector3(recentPosition.x - lastPosition.x, 0, recentPosition.z - lastPosition.z);
            coll = true;
            lockedLastPosition = lastPosition;//if head iron move too fast, the player will still go through the wall, so it needs to freeze the lastposition's update when head move through the wall
        }
    }
}
