using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    //public GameObject myExclamation;
    public NPCSO myData;
    public bool canTalk;
    private void Start()
    {
        DialogController.controller.myExclamation.SetActive(false);
        canTalk = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTalk)
        {
            if (!DialogController.controller.isTalking)
            {
                DialogController.controller.myExclamation.SetActive(false);
                DialogController.controller.StartDialog(myData);
            }
            else
            {
                DialogController.controller.SkipDialog();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogController.controller.myExclamation.SetActive(true);
            canTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogController.controller.myExclamation.SetActive(false);
            canTalk = false;
        }
    }
}
