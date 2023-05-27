using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(3, 10)] public string[] speakers;
    [TextArea(3, 10)] public string[] sentences;
}
