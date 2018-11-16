using System;
using System.Collections.Generic;
using UnityEngine;
using SubjectNerd.Utilities;

public class InputScriptable : ScriptableObject {

    [Serializable]
    public class InputMaper
    {
        public string InputName;
        public KeyCode DefaultKey;
    }

    public bool RewriteConfig;

    [Reorderable]
    public List<InputMaper> inputMap = new List<InputMaper>();
}
