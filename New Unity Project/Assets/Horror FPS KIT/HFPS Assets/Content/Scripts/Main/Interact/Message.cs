﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : MonoBehaviour {
    public enum MessageType { Hint, Message, ItemName }
    public MessageType messageType = MessageType.Message;
    public string message;
}
