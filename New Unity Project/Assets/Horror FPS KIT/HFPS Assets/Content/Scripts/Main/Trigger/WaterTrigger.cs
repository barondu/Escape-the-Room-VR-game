using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterBuoyancy;

[System.Serializable]
public class WaterFoamClass
{
    public GameObject FoamObject;
    public ParticleSystem Foam;
}

public class WaterTrigger : MonoBehaviour {

    private Footsteps footsteps;

    public ParticleSystem WaterFoam;
    public GameObject WaterSplash;
    public float foamShowSpeed = 0.5f;
    public float foamHeight;
    public bool enableWaterFoam;
    public bool isParent;

    private List<WaterFoamClass> WaterFoams = new List<WaterFoamClass>();

    private float localTop;

    void Start()
    {
        footsteps = Camera.main.transform.root.GetComponent<Footsteps>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() && other.GetComponent<Rigidbody>().useGravity)
        {
            Instantiate(WaterSplash, other.gameObject.transform.position, other.gameObject.transform.rotation);

            if (other.GetComponent<FloatingObject>() && other.GetComponent<FloatingObject>().enableFoam && enableWaterFoam)
            {
                WaterFoams.Add(new WaterFoamClass
                {
                    FoamObject = other.gameObject,
                    Foam = Instantiate(WaterFoam, other.gameObject.transform.position, transform.rotation) as ParticleSystem
                });
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (footsteps.gameObject.GetComponent<PlayerController>().onLadder)
            {
                footsteps.inWater = false;
            }
            else
            {
                footsteps.inWater = true;
                
                if (isParent)
                {
                    footsteps.gameObject.GetComponent<PlayerController>().PlayerInWater(transform.parent.position.y + foamHeight);
                }
                else
                {
                    footsteps.gameObject.GetComponent<PlayerController>().PlayerInWater(transform.position.y + foamHeight);
                }

                footsteps.gameObject.GetComponent<PlayerController>().state = 0;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        for(int i = 0; i < WaterFoams.Count; i++)
        {
            if(WaterFoams[i].FoamObject == other.gameObject && WaterFoams.Count > 0)
            {
                Destroy(WaterFoams[i].Foam.gameObject);
                WaterFoams.RemoveAt(i);
            }
        }

        if (other.gameObject == Camera.main.transform.root.gameObject)
        {
            footsteps.inWater = false;
        }
    }

    public void DestroyEvent()
    {
        GetComponent<Collider>().enabled = false;
        footsteps.inWater = false;
        if (isParent)
        {
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (WaterFoams.Count > 0)
        {
            foreach (var variable in WaterFoams)
            {
                variable.Foam.gameObject.name = "Foam_" + variable.FoamObject.name;

                Vector3 foamPos = variable.FoamObject.transform.position;
                if (isParent)
                {
                    foamPos.y = transform.parent.position.y + foamHeight;
                }
                else
                {
                    foamPos.y = transform.position.y + foamHeight;
                }
                variable.Foam.transform.position = foamPos;

                float speed = variable.FoamObject.GetComponent<Rigidbody>().velocity.magnitude;

                if (speed > foamShowSpeed)
                {
                    if(variable.Foam.isStopped)
                        variable.Foam.Play(true);
                }
                else
                {
                    if (variable.Foam.isPlaying)
                        variable.Foam.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }
    }
}
