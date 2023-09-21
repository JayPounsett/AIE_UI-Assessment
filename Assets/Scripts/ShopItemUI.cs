using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    ShopItem item;
    
    [SerializeField] private TextMeshProUGUI playerGold;
    [SerializeField] private TextMeshProUGUI storeGold;

    public bool dragging;
    public Transform originalParent;
    public Slot originalSlotOnParent;
    public Canvas canvas;
    public Slot slot;

    private List<RaycastResult> hits = new List<RaycastResult>();
    private List<RaycastResult> startingSlot = new List<RaycastResult>();
    
    public void SetItem(ShopItem i)
    {
        item = i;
        if (image)
        {
            if (item)
            {
                image.sprite = item.icon;
                image.color = item.color;
            }
            gameObject.SetActive(item != null);
        }
    }

    private void Swap(Slot newParent, Slot oldParent)
    {
        ShopItemUI oldInventory = oldParent.item as ShopItemUI;
        ShopItemUI newInventory = newParent.item as ShopItemUI;
        
        if (newInventory)
        {
            ShopItem startDragItem = oldInventory.item;
            ShopItem endDragItem = newInventory.item;
            
            oldInventory.slot.UpdateItem(endDragItem);
            newInventory.slot.UpdateItem(startDragItem);

            StoreTransaction(oldParent, newParent, startDragItem, endDragItem, oldInventory, newInventory);
        }
    }

    private static void StoreTransaction(Slot oldParent, Slot newParent, ShopItem startDragItem, ShopItem endDragItem, 
        ShopItemUI oldInventory, ShopItemUI newInventory)
    {
        // If buying/selling item for gold and not trading for an item
        if (endDragItem == null)
        {
            float transaction = startDragItem.price - 0;

            //If not enough gold to give change back, stop transaction
            if (newParent.parentInventory.goldTotal - transaction < 0)
            {
                //Cannot do transaction, not enough money to give back, return items
                Debug.Log("Transaction: Buying [" + startDragItem.name + "]. The " + newParent.parentInventory.inventory.name + " cannot afford item. " 
                          + (transaction - newParent.parentInventory.goldTotal) + " more gold pieces are needed.");
                oldInventory.slot.UpdateItem(startDragItem);
                newInventory.slot.UpdateItem(endDragItem);
            }

            if (newParent.parentInventory.goldTotal - transaction >= 0)
            {
                Debug.Log("Transaction: Selling [" + startDragItem.name + "] for gold. The " 
                          + newParent.parentInventory.inventory.name + " hands over " + transaction + " gold pieces.");
                oldParent.parentInventory.goldTotal += transaction;
                newParent.parentInventory.goldTotal -= transaction;
                
                Debug.Log("[" + oldParent.parentInventory.inventory.name + "'s Gold Total] " + oldParent.parentInventory.goldTotal + " gp.");
                Debug.Log("[" + newParent.parentInventory.inventory.name + "'s Gold Total] " + newParent.parentInventory.goldTotal + " gp.");
            }
        }

        else
        {
            // Trading item for item and gold
            float transaction = startDragItem.price - endDragItem.price;

            // If the sold item is worth less than the bought item, oldParent needs to pay gold
            if (transaction < 0)
            {
                //If not enough gold to give change back, stop transaction
                if (oldParent.parentInventory.goldTotal + transaction < 0)
                {
                    //Cannot do transaction, not enough money to give back, return items
                    Debug.Log("Transaction: Buying [" + startDragItem.name + "]. " + oldParent.parentInventory.inventory.name +  " cannot afford item. " 
                              + (transaction - oldParent.parentInventory.goldTotal) + " more gold pieces are needed.");
                    oldInventory.slot.UpdateItem(startDragItem);
                    newInventory.slot.UpdateItem(endDragItem);
                }

                //If enough to give change back, proceed
                if (oldParent.parentInventory.goldTotal + transaction >= 0)
                {
                    Debug.Log("Transaction: Trading [" + startDragItem.name + "] for [" + endDragItem.name + "]. The " 
                              + oldParent.parentInventory.inventory.name + " hands over " + -transaction + " gold pieces.");
                    newParent.parentInventory.goldTotal -= transaction;
                    oldParent.parentInventory.goldTotal += transaction;
                    
                    Debug.Log("[" + oldParent.parentInventory.inventory.name + "'s Gold Total] " + oldParent.parentInventory.goldTotal + " gp.");
                    Debug.Log("[" + newParent.parentInventory.inventory.name + "'s Gold Total] " + newParent.parentInventory.goldTotal + " gp.");
                }
            }

            // else if the item is worth more than the bought item
            if (transaction >= 0)
            {
                if (newParent.parentInventory.goldTotal - transaction < 0)
                {
                    //Cannot do transaction, not enough money to give back, return items
                    Debug.Log("Transaction: Buying [" + startDragItem.name + "]. Cannot afford item. " + (transaction - newParent.parentInventory.goldTotal) + " more gold needed.");
                    oldInventory.slot.UpdateItem(startDragItem);
                    newInventory.slot.UpdateItem(endDragItem);
                }

                if (newParent.parentInventory.goldTotal - transaction >= 0)
                {
                    Debug.Log("Transaction: Trading [" + startDragItem.name + "] for [" + endDragItem.name + "]. " + transaction + " gold handed over.");
                    newParent.parentInventory.goldTotal -= transaction;
                    oldParent.parentInventory.goldTotal += transaction;
                    
                    Debug.Log("[" + oldParent.parentInventory.inventory.name + "'s Gold Total] " + oldParent.parentInventory.goldTotal + " gp.");
                    Debug.Log("[" + newParent.parentInventory.inventory.name + "'s Gold Total] " + newParent.parentInventory.goldTotal + " gp.");
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (originalParent == null) originalParent = transform.parent;
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
        
        //Store starting slot for use later
        EventSystem.current.RaycastAll(eventData, startingSlot);
        foreach (RaycastResult hit in startingSlot)
        {
            Slot s = hit.gameObject.GetComponent<Slot>();
            
            if (s)
            {
                SetStartingSlot(s);
            }
        }
        
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragging)
            transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // See if there is a Slot under the mouse using EventSystem.RaycastAll
        Slot slotFound = null;
        
        EventSystem.current.RaycastAll(eventData, hits);
        foreach (RaycastResult hit in hits)
        {
            Slot s = hit.gameObject.GetComponent<Slot>();

            if (s)
            {
                slotFound = s;
                Swap(slotFound, originalSlotOnParent);
            }
            else
            {
                Transform myTransform;
                
                (myTransform = transform).SetParent(originalParent);
                myTransform.localPosition = Vector3.zero;
            }
        }
    }

    private void SetStartingSlot(Slot slot)
    {
        originalSlotOnParent = slot;
    }
}
