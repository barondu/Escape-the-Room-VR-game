using UnityEngine;

public class DynamicKey : MonoBehaviour {
    public DynamicObject dynamicObject;

    public void UseObject()
    {
        dynamicObject.hasKey = true;
    }
}
