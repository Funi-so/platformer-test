using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create NPCSO", fileName = "New NPC")]
public class NPCSO : ScriptableObject
{
    public string myName;
    //public Sprite NPCSprite;
    public Sentence[] dialog;
}
