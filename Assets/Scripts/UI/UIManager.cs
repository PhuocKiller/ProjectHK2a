using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion;
using Unity.VisualScripting;
using System.Reflection;

public class UIManager : MonoBehaviour
{
    PlayerController player;
    NetworkManager networkManager;
    int numberHealPotionInt, numberManaPotionInt;
    [SerializeField] Transform inventoryPanel;
    public RectTransform crossHairFollow, crossHairUnFollow;
    public MouseFollower mouseFollower;
    private int currentlyDraggedItemIndex = -1;

    void Start()
    {
        networkManager=FindObjectOfType<NetworkManager>();
        Singleton<Inventory>.Instance.ItemAdded += InventoryScript_ItemAdded;
        Singleton<Inventory>.Instance.ItemRemoved += Inventory_ItemRemoved;
        Singleton<Inventory>.Instance.InventoryUpdate += Inventory_Update;
        InitializeInventoryUI();
    }
    public void InitializeInventoryUI()
    {
        int index = -1;
        foreach (Transform slot in inventoryPanel)
        {
            index++;
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            UnityEngine.UI.Image image = imageTransform.GetComponent<Image>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
            itemDragHandler.OnItemClicked += HandleItemSelection;
            itemDragHandler.OnItemBeginDrag += HandleBeginDrag;
            itemDragHandler.OnItemDroppedOn += HandleSwap;
            itemDragHandler.OnItemEndDrag += HandleEndDrag;
            itemDragHandler.OnRightMouseBtnClick += HandleShowItemActions;
        }
    }

    private void HandleShowItemActions(ItemDragHandler itemDrag)
    {
        
    }

    private void HandleEndDrag(ItemDragHandler itemDrag)
    {
        mouseFollower.Toggle(false);
    }

    private void HandleSwap(ItemDragHandler itemDrag)
    {
        Debug.Log("vo swap");
        Debug.Log(itemDrag.transform.parent.parent.name);
    }

    private void HandleBeginDrag(ItemDragHandler itemDrag)
    {
        mouseFollower.Toggle(true);
       mouseFollower.itemDragHandler.GetComponent<Image>().sprite= itemDrag.GetComponent<Image>().sprite;
            }

    private void HandleItemSelection(ItemDragHandler itemDrag)
    {
        
    }

    public void ChosePlayer(int playerIndex)
    {
        networkManager.playerIndex = playerIndex;
    }
    public void ChoseTeam(int teamIndex)
    {
        networkManager.playerTeam = teamIndex;
    }
    
    #region Inventory
    void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        int index = -1;
        foreach (Transform slot in inventoryPanel)
        {
            
            index++;
            Transform imageTransform = slot.GetChild(0).GetChild(0);
                        UnityEngine.UI.Image image = imageTransform.GetComponent<Image>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
            Transform textTransform = slot.GetChild(0).GetChild(0).GetChild(0);
            Text txtCount=textTransform.GetComponent<Text>();


            if (index==e.Item.Slot.Id)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;
                int itemCount=e.Item.Slot.Count;
                if (itemCount > 1)
                {
                    txtCount.text = itemCount.ToString();
                }
                else txtCount.text = "";
                itemDragHandler.Item = e.Item;
                break;
            }
            //we found the empty slot
        }
    }
    void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        int index = -1; 
        foreach (Transform slot in inventoryPanel)
        {
            index++;
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            UnityEngine.UI.Image image = imageTransform.GetComponent<Image>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
            Transform textTransform = slot.GetChild(0).GetChild(1);
            Text txtCount = textTransform.GetComponent<Text>();
          
            if (itemDragHandler.Item == null) continue;
            if (e.Item.Slot.Id == index)
            {
                int itemCount = e.Item.Slot.Count;
                itemDragHandler.Item=e.Item.Slot.FirstItem;
                if (itemCount <2)
                {
                    txtCount.text = "";
                }
                else txtCount.text= itemCount.ToString();
                if (itemCount == 0)
                {
                    image.enabled = false;
                    image.sprite = null;
                }
                break;
            }
            
        }
    }
    void Inventory_Update(object sender, InventoryEventArgs e)
    {
        numberHealPotionInt = 0; numberManaPotionInt = 0;
      //  Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach (Transform slot in inventoryPanel)
        {
            if (slot.childCount == 1)  //vì nút close panel ko có child
            {
                Transform imageTransform = slot.GetChild(0).GetChild(0);
                UnityEngine.UI.Image image = imageTransform.GetComponent<Image>();
                ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
                if (itemDragHandler.Item != null)
                {
                    if (itemDragHandler.Item.itemTypes == ItemTypes.HealPotion || itemDragHandler.Item.itemTypes == ItemTypes.LargeHealPotion)
                    {
                        numberHealPotionInt += 1;

                    }
                    if (itemDragHandler.Item.itemTypes == ItemTypes.ManaPotion || itemDragHandler.Item.itemTypes == ItemTypes.LargeManaPotion)
                    {
                        numberManaPotionInt += 1;

                    }
                    if (itemDragHandler.Item.itemTypes == ItemTypes.Key)
                    {
                    }
                }
            }

        }
       
    }
    #endregion

}
