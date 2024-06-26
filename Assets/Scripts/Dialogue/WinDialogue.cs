using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinDialogue : MonoBehaviour
{
    public NPC npc;
    public NPCSO first;
    public NPCSO second;

    Inventory inventory;

    private void Start()
    {
        npc.myData = first;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inventory = GameManager.instance.inventory;
            List<Item> items = inventory.GetItemList();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemType == Item.ItemType.Sword)
                {
                    npc.myData = second;
                    
                    break; 
                }
            }
        }
    }

    bool hasTalked = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(DialogController.controller.isTalking && DialogController.controller.currentNPC == second)
            {
                hasTalked = true;
            }
            if(hasTalked && DialogController.controller.dialogIndex == 1) 
            {
                DialogController.controller.FinishDialog();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("VictoryScene");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            hasTalked = false;
        }
    }
}
