using UnityEngine;
using ThunderWire.Helper.Parser;

public class ExampleInput : MonoBehaviour
{
    public ConfigHandler configHandler;

    private KeyCode useKey;
    private bool isSet = false;
    private bool isPressed = false;

    void Update()
    {
        if (configHandler.GetKeysCount() > 0 && !isSet)
        {
            useKey = Parser.Convert<KeyCode>(configHandler.Deserialize("Input", "Use"));
            isSet = true;
        }

        if (Input.GetKeyDown(useKey) && !isPressed)
        {
            Debug.Log("Use Key Pressed!");
            isPressed = true;
        }
        else if (isPressed)
        {
            isPressed = false;
        }
    }
}