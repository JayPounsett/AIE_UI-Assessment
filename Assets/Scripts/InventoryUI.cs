using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    
    public ShopItemUI itemPrefab;
    public Slot slotPrefab;
    public float goldTotal;
    
    [SerializeField] private TextMeshProUGUI goldAmountText;

    Slot[] slots;
   
    // Start is called before the first frame update
    void Start()
    {
        // Setup gold amount
        goldAmountText.text = goldTotal.ToString();
        
        // create a slot for each item
        slots = new Slot[inventory.items.Length];
        for (int i = 0; i < inventory.items.Length; i++)
        {
            slots[i] = Instantiate(slotPrefab, transform);
            
            // create a shop item UI and feed the item into it
            slots[i].itemUI = Instantiate(itemPrefab, slots[i].transform);
            slots[i].itemUI.SetItem(inventory.items[i]);
            
            // create a slot
            slots[i].Init(this, i, slots[i].itemUI);
        }
    }

    void Update()
    {
        // Update gold amount
        goldAmountText.text = goldTotal.ToString();
    }
    
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
