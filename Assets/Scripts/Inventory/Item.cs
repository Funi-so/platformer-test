using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        Sword,
        Key,
        Coin,
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.Sword: return ItemAssets.Instance.swordSprite;
            case ItemType.Key: return ItemAssets.Instance.keySprite;
            case ItemType.Coin: return ItemAssets.Instance.coinSprite;

        }
    }
}
