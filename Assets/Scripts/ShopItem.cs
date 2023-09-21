using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ShopItem", menuName = "GUI/ShopItem", order =1)]
public class ShopItem : ScriptableObject
{
    public string itemName;
    [Multiline(3)]
    public string description;
    public float price;

    public Sprite icon;
    public Color color = Color.white;
}
