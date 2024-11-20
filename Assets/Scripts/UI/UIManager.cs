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

public class UIManager : MonoBehaviour
{
    PlayerController player;

    int numberHealPotionInt, numberManaPotionInt;
    [SerializeField] Transform inventoryPanel;
    void Start()
    {
       Singleton<Inventory>.Instance.ItemAdded += InventoryScript_ItemAdded;
        Singleton<Inventory>.Instance.ItemRemoved += Inventory_ItemRemoved;
        Singleton<Inventory>.Instance.InventoryUpdate += Inventory_Update;
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        int index = -1;
        Debug.Log(inventoryPanel);
        foreach (Transform slot in inventoryPanel)
        {
            
            index++;
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            
            UnityEngine.UI.Image image = imageTransform.GetComponent<Image>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
            Transform textTransform = slot.GetChild(0).GetChild(1);
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
    
    
}
