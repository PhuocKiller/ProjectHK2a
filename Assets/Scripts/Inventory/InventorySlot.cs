using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySlot
{
    public List<IInventoryItem> mItemStack = new List<IInventoryItem>();

    public int Id;
    public void AddItem(IInventoryItem item)
    {
        item.Slot = this;
        Debug.Log("item.SlotID " + item.Slot.Id);
        
        mItemStack.Add(item);
    }
    public IInventoryItem FirstItem
    {
        get
        {
            if (IsEmpty) { return null; }
            return mItemStack[0];
        }
    }
    public bool IsStackable(IInventoryItem item)
    {
        if (IsEmpty || Count>=item.maxStack) return false;
        IInventoryItem first= mItemStack[0];
        if (first.Name== item.Name)
        {
            return true;
        }
        return false;
    }
    public bool IsEmpty
    {
        get { return Count == 0; }
    }
    public int Count
    {
        get { return mItemStack.Count; }
    }
    public bool Remove(IInventoryItem item,int indexSlot)
    {
        if (IsEmpty) return false ;
        IInventoryItem first=FirstItem;
        Debug.Log("first.Slot.Id " + first.Slot.Id);
        Debug.Log("mItemStack.count " + mItemStack.Count);
        if (first.Name == item.Name && Id== first.Slot.Id)
        {
            mItemStack.Remove(item);
            return true;
        }
        return false;
    }


}
