using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using ThunderWire.Helper.Parser;

public class SaveObject : MonoBehaviour {

    public enum SaveType { Transform, TransformRigidbody, Position, Rotation, RendererActive, ObjectActive, Door, Drawer, Lever, Valve, MovableInteract, Keypad, Light, NPC, SaveHelper, AttributeSave }
    public SaveType saveType = SaveType.Transform;

    [Tooltip("Unique name under which data will be saved. Name cannot be duplicate of other!!")]
    public string uniqueName;

    public bool includeAttribute;

    public bool disableLoad;
    public bool hasDoor;
    public DynamicObject dynamicObject;

    List<BasePair> saveablePairs = new List<BasePair>();

    void Awake()
    {
        if (saveType == SaveType.AttributeSave || includeAttribute)
        {
            var Pairs = from Com in GetComponents<MonoBehaviour>()
                        where Com.GetType().GetFields().Where(field => field.GetCustomAttributes(typeof(SaveableField), false).Count() > 0) != null
                        select new BasePair(Com, Com.GetType().GetFields().Where(field => field.GetCustomAttributes(typeof(SaveableField), false).Count(atr => atr.GetType() == typeof(SaveableField)) > 0 && !field.IsLiteral && field.IsPublic).ToArray());

            saveablePairs = Pairs.ToList();
        }
    }

    public Dictionary<string, object> GetObjectData()
    {
        Dictionary<string, object> objectData = new Dictionary<string, object>();
        Dictionary<string, object> data = new Dictionary<string, object>();

        objectData.Clear();
        data.Clear();

        if (saveType == SaveType.Transform)
        {
            data.Add("objectEnabled", gameObject.GetComponent<MeshRenderer>().enabled);
            data.Add("position", transform.position);
            data.Add("eulerAngles", transform.eulerAngles);
        }
        else if(saveType == SaveType.TransformRigidbody)
        {
            data.Add("objectEnabled", gameObject.GetComponent<MeshRenderer>().enabled);
            data.Add("position", transform.position);
            data.Add("eulerAngles", transform.eulerAngles);
            data.Add("rb_kinematic", GetComponent<Rigidbody>().isKinematic);
            data.Add("rb_useGravity", GetComponent<Rigidbody>().useGravity);
            data.Add("rb_mass", GetComponent<Rigidbody>().mass);
            data.Add("rb_drag", GetComponent<Rigidbody>().drag);
            data.Add("rb_angDrag", GetComponent<Rigidbody>().angularDrag);
            data.Add("rb_freezeRot", GetComponent<Rigidbody>().freezeRotation);
            data.Add("rb_velocity", GetComponent<Rigidbody>().velocity);
        }
        else if(saveType == SaveType.Position)
        {
            data.Add("position", transform.position);
        }
        else if(saveType == SaveType.Rotation)
        {
            data.Add("eulerAngles", transform.eulerAngles);
        }
        else if(saveType == SaveType.RendererActive)
        {
            data.Add("objectEnabled", gameObject.GetComponent<MeshRenderer>().enabled);
        }
        else if (saveType == SaveType.ObjectActive)
        {
            data.Add("objectEnabled", gameObject.activeSelf);
        }
        else if(saveType == SaveType.Door)
        {
            DynamicObject door = GetComponent<DynamicObject>();
            data.Add("useType", door.useType);
            data.Add("angle", door.Angle);
            data.Add("isOpen", door.isOpened);
            data.Add("isLocked", door.isLocked);
        }
        else if (saveType == SaveType.Drawer)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<DynamicObject>())
                {
                    Dictionary<string, object> drawerData = new Dictionary<string, object>();
                    drawerData.Clear();

                    DynamicObject drawer = transform.GetChild(i).GetComponent<DynamicObject>();
                    drawerData.Add("position", new Vector2(drawer.transform.position.x, drawer.transform.position.z));
                    drawerData.Add("useType", drawer.useType);
                    drawerData.Add("isOpen", drawer.isOpened);
                    drawerData.Add("isLocked", drawer.isLocked);
                    data.Add("drawer_" + i, drawerData);
                }
            }
        }
        else if (saveType == SaveType.Lever)
        {
            DynamicObject lever = GetComponent<DynamicObject>();

            if (!hasDoor)
            {
                data.Add("useType", lever.useType);
                data.Add("leverAngle", lever.Angle);
                data.Add("isUp", lever.isUp);
                data.Add("isHolding", lever.isHolding);
                data.Add("hold", lever.Hold);
            }
            else
            {
                Dictionary<string, object> leverData = new Dictionary<string, object>();
                Dictionary<string, object> doorData = new Dictionary<string, object>();
                leverData.Clear();
                doorData.Clear();

                if (!dynamicObject)
                {
                    Debug.LogError("Please assign DynamicObject with Door type in " + gameObject.name);
                }

                //Lever Data
                leverData.Add("useType", lever.useType);
                leverData.Add("leverAngle", lever.Angle);
                leverData.Add("isUp", lever.isUp);
                leverData.Add("isHolding", lever.isHolding);
                leverData.Add("hold", lever.Hold);

                //Door Data
                doorData.Add("useType", dynamicObject.useType);
                doorData.Add("angle", dynamicObject.Angle);
                doorData.Add("isOpen", dynamicObject.isOpened);
                doorData.Add("isLocked", dynamicObject.isLocked);

                //Add data to return array
                data.Add("leverData", leverData);
                data.Add("doorData", doorData);
            }
        }
        else if(saveType == SaveType.Valve)
        {
            DynamicObject valve = GetComponent<DynamicObject>();
            data.Add("rotateValue", valve.rotateValue);
        }
        else if (saveType == SaveType.MovableInteract)
        {
            data.Add("isInvoked", GetComponent<DynamicObject>().isInvoked);
        }
        else if (saveType == SaveType.Keypad)
        {
            data.Add("accessGranted", GetComponent<Keypad>().m_accessGranted);
        }
        else if (saveType == SaveType.Light)
        {
            data.Add("isOn", GetComponent<Lamp>().isOn);
        }
        else if (saveType == SaveType.NPC)
        {
            data.Add("NPCData", GetComponent<ZombieBehaviour>().GetNPCData());
        }
        else if (saveType == SaveType.SaveHelper)
        {
            if (GetComponent<SaveHelper>())
            {
                GetComponent<SaveHelper>().CallScriptGetValues();
                data = GetComponent<SaveHelper>().GetArray();
            }
            else
            {
                data = null;
            }
        }

        if (saveType == SaveType.AttributeSave || includeAttribute)
        {
            foreach (var Pairs in saveablePairs)
            {
                foreach (var field in Pairs.FieldData)
                {
                    if (field.FieldType == typeof(Color) || field.FieldType == typeof(KeyCode))
                    {
                        if (field.FieldType == typeof(Color))
                        {
                            data.Add(field.Name, string.Format("#{0}", ColorUtility.ToHtmlStringRGBA((Color)Pairs.Instance.GetType().InvokeMember(field.Name, BindingFlags.GetField, null, Pairs.Instance, null))));
                        }
                        else
                        {
                            data.Add(field.Name, Pairs.Instance.GetType().InvokeMember(field.Name, BindingFlags.GetField, null, Pairs.Instance, null).ToString());
                        }
                    }
                    else
                    {
                        data.Add(field.Name, Pairs.Instance.GetType().InvokeMember(field.Name, BindingFlags.GetField, null, Pairs.Instance, null));
                    }
                }
            }
        }

        if (GetComponent<ExamineItem>())
        {
            data.Add("isExamined", GetComponent<ExamineItem>().isExamined);
        }

        if (data != null)
        {
            objectData.Add(uniqueName, data);
        }

        return objectData;
    }

    public void SetObjectData (Newtonsoft.Json.Linq.JToken objectToken)
    {
        if (objectToken.HasValues)
        {
            if (saveType == SaveType.Transform)
            {
                DisableObject(gameObject, (bool)objectToken["objectEnabled"]);

                transform.position = objectToken["position"].ToObject<Vector3>();

                transform.eulerAngles = objectToken["eulerAngles"].ToObject<Vector3>();
            }
            else if(saveType == SaveType.TransformRigidbody)
            {
                DisableObject(gameObject, (bool)objectToken["objectEnabled"]);
                transform.position = objectToken["position"].ToObject<Vector3>();
                transform.eulerAngles = objectToken["eulerAngles"].ToObject<Vector3>();
                GetComponent<Rigidbody>().isKinematic = objectToken["rb_kinematic"].ToObject<bool>();
                GetComponent<Rigidbody>().useGravity = objectToken["rb_useGravity"].ToObject<bool>();
                GetComponent<Rigidbody>().mass = objectToken["rb_mass"].ToObject<float>();
                GetComponent<Rigidbody>().drag = objectToken["rb_drag"].ToObject<float>();
                GetComponent<Rigidbody>().angularDrag = objectToken["rb_angDrag"].ToObject<float>();
                GetComponent<Rigidbody>().freezeRotation = objectToken["rb_freezeRot"].ToObject<bool>();
                GetComponent<Rigidbody>().velocity = objectToken["rb_velocity"].ToObject<Vector3>();
            }
            else if (saveType == SaveType.Position)
            {
                transform.position = objectToken["position"].ToObject<Vector3>();
            }
            else if (saveType == SaveType.Rotation)
            {
                transform.eulerAngles = objectToken["eulerAngles"].ToObject<Vector3>();
            }
            else if (saveType == SaveType.RendererActive)
            {
                DisableObject(gameObject, (bool)objectToken["objectEnabled"]);
            }
            else if (saveType == SaveType.ObjectActive)
            {
                gameObject.SetActive((bool)objectToken["objectEnabled"]);
            }
            else if (saveType == SaveType.Door)
            {
                Vector3 rot = new Vector3(transform.eulerAngles.x, (float)objectToken["angle"], transform.eulerAngles.z);
                transform.eulerAngles = rot;

                GetComponent<DynamicObject>().ParseUseType((int)objectToken["useType"]);
                GetComponent<DynamicObject>().Angle = (float)objectToken["angle"];
                GetComponent<DynamicObject>().isOpened = (bool)objectToken["isOpen"];
                GetComponent<DynamicObject>().isLocked = (bool)objectToken["isLocked"];
            }
            else if (saveType == SaveType.Drawer)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<DynamicObject>())
                    {
                        DynamicObject drawer = transform.GetChild(i).GetComponent<DynamicObject>();
                        var drawerToken = objectToken["drawer_" + i];
                        var posToken = drawerToken["position"];

                        Vector3 pos = new Vector3((float)posToken["x"], drawer.transform.position.y, (float)posToken["y"]);

                        drawer.transform.position = pos;
                        drawer.ParseUseType((int)drawerToken["useType"]);
                        drawer.isOpened = (bool)drawerToken["isOpen"];
                        drawer.isLocked = (bool)drawerToken["isLocked"];
                    }
                }
            }
            else if (saveType == SaveType.Lever)
            {
                bool reverse = GetComponent<DynamicObject>().reverseMove;
                DynamicObject lever = GetComponent<DynamicObject>();

                if (!hasDoor)
                {
                    lever.ParseUseType((int)objectToken["useType"]);

                    if (reverse)
                    {
                        Vector3 rot = new Vector3(transform.localEulerAngles.y, (float)objectToken["leverAngle"], transform.localEulerAngles.z);
                        transform.localEulerAngles = rot;
                    }
                    else
                    {
                        Vector3 rot1 = new Vector3(transform.localEulerAngles.x, (float)objectToken["leverAngle"], transform.localEulerAngles.z);
                        transform.localEulerAngles = rot1;
                    }

                    lever.isUp = (bool)objectToken["isUp"];
                    lever.isHolding = (bool)objectToken["isHolding"];
                    lever.Hold = (bool)objectToken["hold"];
                    lever.loadDisabled = disableLoad;
                }
                else
                {
                    var doorData = objectToken["doorData"];
                    var leverData = objectToken["leverData"];

                    //Door data
                    Vector3 doorRot = new Vector3(dynamicObject.transform.eulerAngles.x, (float)doorData["angle"], dynamicObject.transform.eulerAngles.z);
                    dynamicObject.transform.eulerAngles = doorRot;
                    dynamicObject.ParseUseType((int)doorData["useType"]);
                    dynamicObject.Angle = (float)doorData["angle"];
                    dynamicObject.isOpened = (bool)doorData["isOpen"];
                    dynamicObject.isLocked = (bool)doorData["isLocked"];

                    //Lever data
                    lever.ParseUseType((int)leverData["useType"]);
                    if (reverse)
                    {
                        Vector3 rot = new Vector3(transform.localEulerAngles.y, (float)leverData["leverAngle"], transform.localEulerAngles.z);
                        transform.localEulerAngles = rot;
                    }
                    else
                    {
                        Vector3 rot1 = new Vector3(transform.localEulerAngles.x, (float)leverData["leverAngle"], transform.localEulerAngles.z);
                        transform.localEulerAngles = rot1;
                    }
                    lever.isUp = (bool)leverData["isUp"];
                    lever.isHolding = (bool)leverData["isHolding"];
                    lever.Hold = (bool)leverData["hold"];
                    lever.loadDisabled = disableLoad;
                }
            }
            else if (saveType == SaveType.Valve)
            {
                GetComponent<DynamicObject>().rotateValue = (float)objectToken["rotateValue"];
                GetComponent<DynamicObject>().loadDisabled = disableLoad;
            }
            else if (saveType == SaveType.MovableInteract)
            {
                GetComponent<DynamicObject>().isInvoked = (bool)objectToken["isInvoked"];
            }
            else if (saveType == SaveType.Keypad)
            {
                bool granted = (bool)objectToken["accessGranted"];
                if (granted)
                {
                    GetComponent<Keypad>().SetAccessGranted();
                }
            }
            else if (saveType == SaveType.Light)
            {
                GetComponent<Lamp>().isOn = (bool)objectToken["isOn"];
                GetComponent<Lamp>().OnLoad();
            }
            else if (saveType == SaveType.NPC)
            {
                GetComponent<ZombieBehaviour>().SetNPCData(objectToken["NPCData"]);
            }
            else if (saveType == SaveType.SaveHelper)
            {
                if (GetComponent<SaveHelper>())
                {
                    GetComponent<SaveHelper>().LoadSavedValues(objectToken);
                }
            }

            if (saveType == SaveType.AttributeSave || includeAttribute)
            {
                foreach (var Pair in saveablePairs)
                {
                    foreach (var Field in Pair.FieldData)
                    {
                        SetValue(Pair.Instance, Field, objectToken[Field.Name]);
                    }
                }
            }

            if (GetComponent<ExamineItem>())
            {
                GetComponent<ExamineItem>().isExamined = (bool)objectToken["isExamined"];
            }
        }
    }

    string GetObjectEnabled(GameObject obj)
    {
        return obj.GetComponent<MeshRenderer>().enabled.ToString();
    }

    void DisableObject(GameObject obj, bool active)
    {
        if (active == false)
        {
            if (obj.GetComponent<InventoryItem>())
            {
                obj.GetComponent<InventoryItem>().DisableObject(active);
            }
            else
            {
                obj.GetComponent<MeshRenderer>().enabled = false;
                obj.GetComponent<Collider>().enabled = false;
            }
        }
    }

    private static void SetValue(object instance, FieldInfo fInfo, Newtonsoft.Json.Linq.JToken jvalue)
    {
        System.Type type = fInfo.FieldType;
        string value = jvalue.ToString();
        if (type == typeof(string)) fInfo.SetValue(instance, value);
        if (type == typeof(int)) fInfo.SetValue(instance, int.Parse(value));
        if (type == typeof(uint)) fInfo.SetValue(instance, uint.Parse(value));
        if (type == typeof(long)) fInfo.SetValue(instance, long.Parse(value));
        if (type == typeof(ulong)) fInfo.SetValue(instance, ulong.Parse(value));
        if (type == typeof(float)) fInfo.SetValue(instance, float.Parse(value));
        if (type == typeof(double)) fInfo.SetValue(instance, double.Parse(value));
        if (type == typeof(bool)) fInfo.SetValue(instance, bool.Parse(value));
        if (type == typeof(char)) fInfo.SetValue(instance, char.Parse(value));
        if (type == typeof(short)) fInfo.SetValue(instance, short.Parse(value));
        if (type == typeof(byte)) fInfo.SetValue(instance, byte.Parse(value));
        if (type == typeof(Vector2)) fInfo.SetValue(instance, jvalue.ToObject(type));
        if (type == typeof(Vector3)) fInfo.SetValue(instance, jvalue.ToObject(type));
        if (type == typeof(Vector4)) fInfo.SetValue(instance, jvalue.ToObject(type));
        if (type == typeof(Quaternion)) fInfo.SetValue(instance, jvalue.ToObject(type));
        if (type == typeof(KeyCode)) fInfo.SetValue(instance, Parser.Convert<KeyCode>(value));
        if (type == typeof(Color)) fInfo.SetValue(instance, Parser.Convert<Color>(value));
    }
}

public class BasePair
{
    public Component Instance;
    public FieldInfo[] FieldData;

    public BasePair(Component key, FieldInfo[] value)
    {
        Instance = key;
        FieldData = value;
    }
}