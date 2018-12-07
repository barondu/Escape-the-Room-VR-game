using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public GameObject doorblocker;
    public GameObject doorlockedtext;
    public bool isHasKey;
    public bool useAnim;
    Animation anim,anim1;
    private bool isOpen;
    public bool checkPlank;
    public GameObject[] planks;
    public bool IsJumpcare;
    private GameObject jumpscare;
    public bool checkSwitch;
    public GameObject[] switchsLinearMapping;
    public int[] password;
    //public int size;

    // Use this for initialization
    void Start()
    {
        //doorblocker = GameObject.Find("doorwall2(2)");
        anim = GetComponent<Animation>();
        jumpscare = GameObject.Find("Example_Jumpscare");
        //doorlockedtext = GameObject.Find("text_door_locked");
        //isHasKey = false;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkPlank)
        {
            CheckPlank();
        }
        if(checkSwitch)
        {
            CheckSwitchs();
        }
    }
    public void unlock()
    {
        if (isHasKey && useAnim && !isOpen)
        {
            doorblocker.SetActive(false);
            anim.Play("Door_Open");
            if(IsJumpcare)
            {
                jumpscare.GetComponent<Jumpscare>().OnTriggerEnter();
            }
            isOpen = true;
        }
        else if (isHasKey && useAnim && isOpen)
        {
            doorblocker.SetActive(false);
            anim1.Play("Door_Close");
            isOpen = false;
        }
        else if (isHasKey && !useAnim)
        {
            doorblocker.SetActive(false);
        }
        else if (!isHasKey)
        {
            doorlockedtext.GetComponent<TextMesh>().text = "DOOR LOCKED!";
        }
        else { }
    }
    public void setIsHasKey(bool bool1)
    {
        isHasKey = bool1;
    }
    public void disattacheddoor()
    {
        doorlockedtext.GetComponent<TextMesh>().text = "";
    }

    private void CheckPlank()
    {
        int count = 0;
        foreach (GameObject i in planks)
        {
            if(i.GetComponent<Rigidbody>().useGravity==true)
            {
                count++;
            }
        }
        if (count == planks.Length)
        {
            setIsHasKey(true);
        }
    }

    private void CheckSwitchs()
    {
        //int[] password = new int[switchs.Length];
        
        int count = 0;
        for(int i = 0; i < switchsLinearMapping.Length;i++)
        {
            float linearValue = switchsLinearMapping[i].GetComponent< Valve.VR.InteractionSystem.LinearMapping>().value;
            print(linearValue);
            if (linearValue == 1.0f && password[i] == 1 )
            {
                count++;
            }
            if (linearValue == 0.0f && password[i] == 0)
            {
                count++;
            }
        }
       print(count);
       if (count == switchsLinearMapping.Length)
       {
            setIsHasKey(true);
            unlock();
       }
        
    }

    /*public void SetSwitchValue(int index, bool bool1)
    {
        if(index>switchs.Length-1|| index<0)
        {
            return;
        }
        switchs[index]
    }
    */
   
}
