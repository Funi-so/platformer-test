using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    /*logica:
    checar se jogador possui item
    se possuir, troca o diálogo
    verifica o index, caso for um especifico executa código
    */
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
                if (items[i].itemType == Item.ItemType.Key)
                {
                    Debug.Log("Trocou");
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
                Destroy(gameObject);
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
