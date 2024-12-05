using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySlot
{
    private Stack<IInventoryItem> mItemStack = new Stack<IInventoryItem>();

    public int Id;
    public void AddItem(IInventoryItem item)
    {
        item.Slot = this;
        
        mItemStack.Push(item);
    }
    public IInventoryItem FirstItem
    {
        get
        {
            if (IsEmpty) { return null; }
            return mItemStack.Peek();
        }
    }
    public bool IsStackable(IInventoryItem item)
    {
        if (IsEmpty || Count>=item.maxStack) return false;
        IInventoryItem first= mItemStack.Peek();
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
        IInventoryItem first=mItemStack.Peek();
        Debug.Log("first.Slot.Id " + first.Slot.Id);
        if (first.Name == item.Name && Id== first.Slot.Id)
        {
            mItemStack.Pop();
            return true;
        }
        return false;
    }


}
