using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public void AddIndexCount(int value)
    {
        DialogController.controller.dialogIndex += value;
        DialogController.controller.Dialog();
    }
}
