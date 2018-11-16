using UnityEngine;

public class UIObjectInfo : MonoBehaviour {

    public string objectTitle;

    [Header("Settings")]
    public bool autoObjectTitle;
    public bool isUppercased;

    [System.Serializable]
    public class Titles
    {
        public string DoorOpen = "Open Door";
        public string DoorClose = "Close Door";

        public string DrawerOpen = "Open Drawer";
        public string DrawerClose = "Close Drawer";

        public string SwitchOn = "Switch On";
        public string SwitchOff = "Switch Off";
    }

    [Space(7)]
    public Titles titles = new Titles();

    private DynamicObject dynamicObject;
    private Lamp lamp;
    private LightSwitch lightSwitch;

    void Start()
    {
        if (autoObjectTitle)
        {
            if (GetComponent<DynamicObject>())
            {
                dynamicObject = GetComponent<DynamicObject>();

                if (dynamicObject.interactType != Type_Interact.Animation)
                {
                    Debug.LogError("Object Info is only available for Animation Interact Type");
                }
            }
            else if (GetComponent<Lamp>())
            {
                lamp = GetComponent<Lamp>();
            }
            else if (GetComponent<LightSwitch>())
            {
                lightSwitch = GetComponent<LightSwitch>();
            }
        }
    }

    void Update()
    {
        if (autoObjectTitle)
        {
            if (dynamicObject)
            {
                if (dynamicObject.interactType == Type_Interact.Animation)
                {
                    if (dynamicObject.dynamicType == Type_Dynamic.Door)
                    {
                        if (dynamicObject.isOpened)
                        {
                            objectTitle = titles.DoorClose;
                        }
                        else
                        {
                            objectTitle = titles.DoorOpen;
                        }
                    }
                    else if (dynamicObject.dynamicType == Type_Dynamic.Drawer)
                    {
                        if (dynamicObject.isOpened)
                        {
                            objectTitle = titles.DrawerClose;
                        }
                        else
                        {
                            objectTitle = titles.DrawerOpen;
                        }
                    }
                    else if(dynamicObject.dynamicType == Type_Dynamic.Lever)
                    {
                        if (dynamicObject.isUp)
                        {
                            objectTitle = titles.SwitchOff;
                        }
                        else
                        {
                            objectTitle = titles.SwitchOn;
                        }
                    }
                }
            }
            else if (lamp)
            {
                if (lamp.isOn)
                {
                    objectTitle = titles.SwitchOff;
                }
                else
                {
                    objectTitle = titles.SwitchOn;
                }
            }
            else if (lightSwitch)
            {
                if (lightSwitch.isOn)
                {
                    objectTitle = titles.SwitchOff;
                }
                else
                {
                    objectTitle = titles.SwitchOn;
                }
            }
        }

        if (isUppercased)
        {
            objectTitle = objectTitle.ToUpper();
        }
    }
}
