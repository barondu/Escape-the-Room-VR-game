using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzle3Box : MonoBehaviour {
    public GameObject[] allPeople;
    //private int NumLiar;
    //public GameObject murder;
    public GameObject teleportpoint;
    //private string[] state;
    private string currentState;
	// Use this for initialization
	void Start () {
        //state = new string[3]{ "initial","liar","murder"};
        currentState = "initial";
    }
	
	// Update is called once per frame
	void Update () {
        

      //  print(count + "         fffffffffffffffffffffffffffffffff");
      switch (currentState){
            case "initial":
                int count = 0;
                foreach (GameObject i in allPeople)
                {
                    if (i.GetComponent<puzzle3>().IsChooseRight())
                    { count++; }
                }

                if (count == allPeople.Length)
                {
                    foreach (GameObject i in allPeople)
                    {
                        if (i.GetComponent<puzzle3>().liar)
                        {
                            i.GetComponent<puzzle3>().setLightColor(new Color(0.0f, 1.0f, 0.0f));
                        }
                    }
                    currentState = "liar";

                }
               
                break;
            case "liar":
                bool FindMurderer = false;
                bool IfSelectedMurderer = false;
                foreach (GameObject i in allPeople)
                {
                    if(i.GetComponent<puzzle3>().CheckMurderer())
                    {
                        i.GetComponent<puzzle3>().setLightColor(new Color(1.0f, 0.0f, 0.0f));
                        FindMurderer = true;
                        currentState = "murder";
                        this.GetComponent<AudioSource>().Play();
                    }
                    if (i.GetComponent<puzzle3>().GetisSelected_Murder())
                    {
                        IfSelectedMurderer = true;
                    }
                }
                if(!FindMurderer && IfSelectedMurderer)
                {

                    foreach (GameObject i in allPeople)
                    {
                        i.GetComponent<puzzle3>().Resetall();
                    }
                    currentState = "initial";
                }

                break;

            case "murder":
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                this.GetComponent<Rigidbody>().useGravity = true;
                teleportpoint.SetActive(true);
                break;

        }
        	
	}
    public string GetState()
    {
        return currentState;
    }
}
