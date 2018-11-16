using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    private ConfigHandler configHandler;
    private GameObject player;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public bool isLocalCamera;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -60F;
    public float maximumX = 60F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public float offsetY = 0F;
    public float offsetX = 0F;

    public float rotationX = 0F;
    GameObject cmra = null;

    public float rotationY = 0F;

    Quaternion originalRotation;
    Quaternion playerOriginalRotation;

    void Start()
    {
        configHandler = GameObject.Find("GAMEMANAGER").GetComponent<ConfigHandler>();
        player = transform.root.gameObject;

        if (!isLocalCamera)
        {
            cmra = GameObject.FindWithTag("MainCamera");
        }
        else
        {
            cmra = this.gameObject;
            Cursor.visible = (false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        originalRotation = transform.localRotation;
        playerOriginalRotation = player.transform.localRotation;
    }

    void Update()
    {
        if (configHandler && configHandler.ContainsSection("Game") && configHandler.ContainsSectionKey("Game", "Sensitivity"))
        {
            float sensitivity = float.Parse(configHandler.Deserialize("Game", "Sensitivity"));
            if (!(sensitivity == 0))
            {
                if (!(sensitivityX == 0))
                {
                    sensitivityX = sensitivity;
                }
                if (!(sensitivityY == 0))
                {
                    sensitivityY = sensitivity;
                }
            }
        }

        if (Cursor.lockState == CursorLockMode.None)
            return;

        if (axes == RotationAxes.MouseXAndY)
        {
            // Read the mouse input axis
            rotationX += (Input.GetAxis("Mouse X") * sensitivityX / 30 * cmra.GetComponent<Camera>().fieldOfView + offsetX);
            rotationY += (Input.GetAxis("Mouse Y") * sensitivityY / 30 * cmra.GetComponent<Camera>().fieldOfView + offsetY);

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

            player.transform.localRotation = playerOriginalRotation * xQuaternion;
            transform.localRotation = originalRotation * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += (Input.GetAxis("Mouse X") * sensitivityX / 60 * cmra.GetComponent<Camera>().fieldOfView + offsetX);
            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotationY += (Input.GetAxis("Mouse Y") * sensitivityY / 60 * cmra.GetComponent<Camera>().fieldOfView + offsetY);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            transform.localRotation = originalRotation * yQuaternion;
        }
        offsetY = 0F;
        offsetX = 0F;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public Vector2 GetRotation()
    {
        return new Vector2(rotationX, rotationY);
    }

    public void SetRotation(Vector2 rotation)
    {
        rotationX = rotation.x;
        rotationY = rotation.y;
    }
}