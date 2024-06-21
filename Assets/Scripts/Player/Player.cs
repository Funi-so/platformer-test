using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    

    void OnTriggerEnter(Collider collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
        if(itemWorld != null)
        {
            Debug.Log("Touching Item");
            GameManager.instance.inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }
    }
}
