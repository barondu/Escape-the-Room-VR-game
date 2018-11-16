using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimEvents
{
    public string EventCallName;
    public GameObject EventObject;
    public string EventToSend;
}

public class AnimationEvent : MonoBehaviour {

    public AnimEvents[] AnimationEvents;

	public void SendEvent (string CallName) {
        foreach(var ent in AnimationEvents)
        {
            if(ent.EventCallName == CallName)
            {
                ent.EventObject.SendMessage(ent.EventToSend, SendMessageOptions.DontRequireReceiver);
            }
        }
	}
}
