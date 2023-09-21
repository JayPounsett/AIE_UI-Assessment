using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public ShopItemUI itemUI;

    public InventoryUI parentInventory;
    public int index;
    public ShopItemUI item;

    public void Init(InventoryUI par, int ind, ShopItemUI it)
    {
        index = ind;
        parentInventory = par;
        item = it;
        item.slot = this;
    }

    public void UpdateItem(ShopItem item)
    {
        // update the backend array
        parentInventory.inventory.items[index] = item;

        // update the front end array
        itemUI.SetItem(item);
    }
}
