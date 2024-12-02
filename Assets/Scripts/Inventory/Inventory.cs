using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 6;
    private IList<InventorySlot> mSlots = new List<InventorySlot>();
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
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
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
        
        networkManager.SpawnObjWhenAddItem(networkManager.IndexItemBaseOnName(item.Name), freeSlot.Id);
        indexItem = freeSlot.Id;
        SkillButton btn = inventoryPanel.GetChild(freeSlot.Id).GetComponent<SkillButton>();
        btn.Initialize(item.skillName);
    }

    public void RemoveItem(IInventoryItem item) //Quăng item ra đất
    {
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.Remove(item))
            {
                if (ItemRemoved != null)
                {
                    ItemRemoved(this,new InventoryEventArgs(item));
                    networkManager.DespawnObjWhenRemoveItem(item.Name);
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
    internal void UseItemClickInventory(IInventoryItem item) //Use item khi click trực tiếp trong inventory
    {
        if (!buyItemPanel.activeInHierarchy)
        {
            if (ItemUsed != null)
            {
                ItemUsed(this, new InventoryEventArgs(item));
            }
            item.OnUse(); //item remove nằm trong đây
            if (InventoryUpdate != null)
            {
                InventoryUpdate(this, new InventoryEventArgs(item));
            }
        }
        else
        {
            buyItemPanel.GetComponent<ItemsManager>().CheckInfoToSell(item);
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
