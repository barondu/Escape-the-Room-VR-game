using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public GameObject doorblocker;
    public GameObject doorlockedtext;
    public bool isHasKey;
    Animation anim;

    // Use this for initialization
    void Start()
    {
        //doorblocker = GameObject.Find("doorwall2(2)");
        anim = GetComponent<Animation>();
        //doorlockedtext = GameObject.Find("text_door_locked");
        isHasKey = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void unlock()
    {
        if (isHasKey)
        {
            doorblocker.SetActive(false);
            //anim.
            anim.Play("Door_Open");
        }
        else
        {
            doorlockedtext.GetComponent<TextMesh>().text = "DOOR LOCKED!";
        }
    }
    public void setIsHasKey(bool bool1)
    {
        isHasKey = bool1;
    }
    public void disattacheddoor()
    {
        doorlockedtext.GetComponent<TextMesh>().text = "";
    }
}
