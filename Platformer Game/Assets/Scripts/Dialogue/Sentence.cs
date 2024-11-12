using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Speaker{NPC,Player }
[System.Serializable]
public class Sentence
{
    public Speaker speaker;
    public string message;
    public string[] answers = new string[3];
}
