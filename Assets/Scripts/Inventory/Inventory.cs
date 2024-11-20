using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 6;
    public List<IInventoryItem> mItems = new List<IInventoryItem>();
    private IList<InventorySlot> mSlots = new List<InventorySlot>();
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public event EventHandler<InventoryEventArgs> ItemUsed;
    public event EventHandler<InventoryEventArgs> InventoryUpdate;
    public InventoryItemBase[] inventoryItems, inventory_9_Items;
    InventoryItemBase item;
    private void Awake()
    {
        
    }
    private void Start()
    {
    }

    public void AddItem(InventoryItemBase item)
    {
       /* if (mItems.Count < SLOTS)
        {
            Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            if (collider.enabled)
            {

                collider.enabled = false;
                mItems.Add(item);
                item.OnPickUp();
                if (ItemAdded != null)
                {
                    ItemAdded(this, new InventoryEventArgs(item));
                }
                if (InventoryUpdate != null)
                {
                    InventoryUpdate(this, new InventoryEventArgs(item));
                }
            }
        }*/


        InventorySlot freeSlot = FindStackAble(item);
        if (freeSlot == null)
        {
            freeSlot = FindNextEmptySlot();
        }
        if (freeSlot != null)
        {
            

            freeSlot.AddItem(item);
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }
    public void RemoveItem(IInventoryItem item) //Quăng item ra đất
    {

        /*if (mItems.Contains(item))
        {
            mItems.Remove(item);
            item.OnDrop();

            if (ItemRemoved != null)
            {
                ItemRemoved(this, new InventoryEventArgs(item));
            }
            if (InventoryUpdate != null)
            {
                InventoryUpdate(this, new InventoryEventArgs(item));
            }
        }*/
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.Remove(item))
            {
                if (ItemRemoved != null)
                {
                    ItemRemoved(this,new InventoryEventArgs(item));
                }
                break;
            }
        }
    }

    internal void UseItemClickInventory(IInventoryItem item) //Use item khi click trực tiếp trong inventory
    {
        if (ItemUsed != null)
        {
            ItemUsed(this, new InventoryEventArgs(item));
        }

        item.OnUse();
        

        if (InventoryUpdate != null)
            {
                InventoryUpdate(this, new InventoryEventArgs(item));
            }
        
    }
    
    internal void UseItemClickButton(InventoryItemBase item) //Use item khi click button
    {
        if (mItems.Contains(item))
        {
            mItems.Remove(item);
            item.OnUse();
            if (ItemRemoved != null)
            {
                ItemRemoved(this, new InventoryEventArgs(item));
            }
            if (InventoryUpdate != null)
            {
                InventoryUpdate(this, new InventoryEventArgs(item));
            }
        }
    }
   
    /*void BuyItemSuccess(int itemCost)
    {
        PlayerController.instance.coins -= itemCost;
        UIManager.instance.coinValues.text = PlayerController.instance.coins.ToString();
        AudioManager.instance.PlaySound(AudioManager.instance.buyItem);
    }*/
    public void CreateNewItem(Vector3 pos, ItemTypes itemTypes) //tạo ra item khi quăng ra đất
    {
        int i;
        for (i = 0; i <= inventoryItems.Length; i++)
        {
            if (inventoryItems[i].itemTypes == itemTypes)
            {
                break;
            }
        }
        Instantiate(inventoryItems[i], pos, Quaternion.identity);
    }
    public Inventory()
    {
        for (int i = 0;i<SLOTS;i++)
        {
            mSlots.Add(new InventorySlot(i));
        }
    }
    private InventorySlot FindStackAble(InventoryItemBase item)
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsStackable(item))
            {
                return slot;
            }
            
        }
        return null;
    }
    private InventorySlot FindNextEmptySlot()
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsEmpty) return slot;
        }
        return null;
    }
    
    
}
