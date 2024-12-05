using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 6;
    public IList<InventorySlot> mSlots = new List<InventorySlot>();
    public event EventHandler<InventoryEventArgs> ItemAdded,ItemRemoved,ItemUsed,InventoryUpdate;
    public Action<ItemDragHandler> OnItemClicked, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;
    public Action<int, int> OnItemDroppedOn;
    public InventoryItemBase[] inventoryItems, inventory_9_Items;
    public int indexItemSlot_1, indexItemSlot_2;
    InventoryItemBase item;
    public GameObject buyItemPanel;
    [SerializeField] Transform inventoryPanel;


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
        /*SkillButton newBtn = new SkillButton();
        newBtn.Initialize(inventoryPanel.GetChild(mSlots[indexItemSlot_1].Id).GetComponent<SkillButton>().m_skillName);
        inventoryPanel.GetChild(mSlots[indexItemSlot_1].Id).GetComponent<SkillButton>().Initialize(
          inventoryPanel.GetChild(mSlots[indexItemSlot_2].Id).GetComponent<SkillButton>().m_skillName);
        inventoryPanel.GetChild(mSlots[indexItemSlot_2].Id).GetComponent<SkillButton>().Initialize(newBtn.m_skillName);*/
        int newIDSlot = -1;
        newIDSlot = mSlots[indexItemSlot_1].Id;
        mSlots[indexItemSlot_1].Id = mSlots[indexItemSlot_2].Id;
        mSlots[indexItemSlot_2].Id = newIDSlot;


        InventorySlot newSlot = new InventorySlot();
        newSlot = mSlots[indexItemSlot_1];
        mSlots[indexItemSlot_1] = mSlots[indexItemSlot_2];
        mSlots[indexItemSlot_2] = newSlot;
        OnItemDroppedOn?.Invoke(indexItemSlot_1, indexItemSlot_2);
    }
    public void AddItem(InventoryItemBase item, out int indexItem)
    {
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        InventoryItemBase newItem = item.Clone();
        InventorySlot freeSlot = FindStackAble(newItem);
        if (freeSlot == null)
        {
            freeSlot = FindNextEmptySlot();
        }
        if (freeSlot != null)
        {
            
            freeSlot.AddItem(newItem);
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(newItem));
            }
        }
        networkManager.SpawnObjWhenAddItem(networkManager.IndexItemBaseOnName(newItem.Name), freeSlot.Id);
        indexItem = freeSlot.Id;
        SkillButton btn = inventoryPanel.GetChild(freeSlot.Id).GetComponent<SkillButton>();
        btn.Initialize(newItem.skillName);
    }

    public void RemoveItem(InventoryItemBase item,int indexSlot)
    {
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        foreach (InventorySlot slot in mSlots)
        {
            foreach (var itemSlot in slot.mItemStack)
            {
               // Debug.Log("itemSlot.Slot.ID " + itemSlot.Slot.Id);
            }
            if (slot.Remove(item, indexSlot))
            {
                if (ItemRemoved != null)
                {
                    ItemRemoved(this,new InventoryEventArgs(item));
                    networkManager.DestroyObjWhenRemoveItem(item.Name);
                }
                if(slot.Count==0)
                {
                    SkillButton btn = inventoryPanel.GetChild(slot.Id).GetComponent<SkillButton>();
                    btn.Initialize(SkillName.None);
                }
                break;
            }
        }
    }
    internal void UseItemClickInventory(InventoryItemBase item,int indexSlot, out bool canActive) //Use item khi click trực tiếp trong inventory
    {
        if (!buyItemPanel.activeInHierarchy)
        {
            if (ItemUsed != null)
            {
                ItemUsed(this, new InventoryEventArgs(item));
            }
            item.OnUse(indexSlot); //item remove nằm trong đây
            if (InventoryUpdate != null)
            {
                InventoryUpdate(this, new InventoryEventArgs(item));
            }
            canActive = true;
        }
        else
        {
            buyItemPanel.GetComponent<ItemsManager>().CheckInfoToSell(item, indexSlot);
            canActive=false;
        }
    }
    #region old code

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
