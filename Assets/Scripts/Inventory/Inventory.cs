using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 6;
    public List<IInventoryItem> mItems = new List<IInventoryItem>();
    private IList<InventorySlot> mSlots = new List<InventorySlot>();
    public event EventHandler<InventoryEventArgs> ItemAdded,ItemRemoved,ItemUsed,InventoryUpdate;
    public Action<ItemDragHandler> OnItemClicked, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;
    public Action<int, int> OnItemDroppedOn;
    public InventoryItemBase[] inventoryItems, inventory_9_Items;
    public int indexItemSlot_1, indexItemSlot_2;
    InventoryItemBase item;
    public Inventory()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            mSlots.Add(new InventorySlot());
            mSlots[i].Id = i;
        }
    }
    public void SwapItem()
    {
        int newIDSlot = -1;
        newIDSlot = mSlots[indexItemSlot_1].Id;
        mSlots[indexItemSlot_1].Id =mSlots[indexItemSlot_2].Id;
        mSlots[indexItemSlot_2].Id = newIDSlot;
        

        InventorySlot newSlot = new InventorySlot();
        newSlot=mSlots[indexItemSlot_1];
        mSlots[indexItemSlot_1] = mSlots[indexItemSlot_2];
        mSlots[indexItemSlot_2]=newSlot;
        
        OnItemDroppedOn?.Invoke(indexItemSlot_1, indexItemSlot_2);
    }
    public void AddItem(InventoryItemBase item, out int indexItem)
    {
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
        indexItem = freeSlot.Id;
    }

    public void RemoveItem(IInventoryItem item) //Quăng item ra đất
    {
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
#region old code
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
    #endregion
    
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
