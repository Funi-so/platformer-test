using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public Item item;

    public static ItemWorld SpawnItemWorld(Vector3 position, Item item){
        Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);

        return itemWorld;
    }


    public void SetItem(Item item)
    {
        this.item = item;
    }

    public Item GetItem() { return item; }

    public void DestroySelf() { Destroy(gameObject); }
}
