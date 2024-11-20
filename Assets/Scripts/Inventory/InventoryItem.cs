using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes
{
    HealPotion,
    ManaPotion,
    LargeHealPotion,
    LargeManaPotion,
    Key
}
public class InventoryItem : MonoBehaviour
{

}
public interface IInventoryItem
{
    ItemTypes itemTypes { get; set; }
    string Name { get; }
    Sprite Image { get; }
    void OnPickUp();
    void OnDrop();
    void OnUse();
    InventorySlot Slot { get; set; }
}
public class InventoryEventArgs : EventArgs

{
    public IInventoryItem Item;
    public InventoryEventArgs(IInventoryItem item)
    {
        Item = item;
    }
}
