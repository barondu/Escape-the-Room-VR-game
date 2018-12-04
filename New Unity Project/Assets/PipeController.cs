using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//namespace Valve.VR.InteractionSystem
//{

    public class PipeController : MonoBehaviour
    {
        public GameObject hintLight;
        public GameObject linearmapping;
        
        // Use tpublic GameObjecthis for initialization
        void Start()
        {
            hintLight = GameObject.Find("Cagelight_pipe");
            linearmapping = GameObject.Find("LinearMapping_pipe");
    }

        // Update is called once per frame
        void Update()
        {
            double value = linearmapping.GetComponent<Valve.VR.InteractionSystem.LinearMapping>().value;
            if (value >=0 && value <0.2 )
            {
            //red
            hintLight.GetComponent<WallLamp>().color =  new Color(1f,0f,0f);
            
            }
            //green
            if (value >= 0.2 && value < 0.4)
            {
            hintLight.GetComponent<WallLamp>().color = new Color(0f, 1f, 0f);
            }
            //blue
            if (value >= 0.4 && value < 0.6)
            {
            hintLight.GetComponent<WallLamp>().color = new Color(0f, 0f, 1f);
            }
            //yellow
            if (value >= 0.6 && value < 0.8)
            {
            hintLight.GetComponent<WallLamp>().color = new Color(1f, 1f, 0f);
            }
            //pink
            if (value >= 0.8 && value < 1)
            {
            hintLight.GetComponent<WallLamp>().color = new Color(1f, 0f, 1f);
            }
    }
    }
//}