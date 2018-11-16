using UnityEngine;

public class ExampleInputManager : MonoBehaviour {

    public InputController inputManager;

    public KeyCode useKey;
    private bool isPressed = false;

    void Update()
    {
        if (inputManager.HasInputs())
        {
            useKey = inputManager.GetInput("Use");
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
