using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SaveObject))]
public class SaveHelper : MonoBehaviour
{
    private Dictionary<string, object> m_dictionaryArray = new Dictionary<string, object>();

    public void SetArray(Dictionary<string, object> dictionaryArray)
    {
        m_dictionaryArray = dictionaryArray;
    }

    public Dictionary<string, object> GetArray()
    {
        return m_dictionaryArray;
    }

    public void CallScriptGetValues()
    {
        SendMessage("OnSave", SendMessageOptions.DontRequireReceiver);
    }

    public void LoadSavedValues(Newtonsoft.Json.Linq.JToken valuesToken)
    {
        SendMessage("OnLoad", valuesToken, SendMessageOptions.DontRequireReceiver);
    }

    public int ValuesCount()
    {
        return m_dictionaryArray.Count;
    }
}
