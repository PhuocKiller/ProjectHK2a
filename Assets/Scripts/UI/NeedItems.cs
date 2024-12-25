using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class NeedItems : MonoBehaviour
{
    [SerializeField] int[] indexItems=new int[6];
    [SerializeField] Transform panelNeedItems;
    [SerializeField] NetworkManager networkManager;
    public void ShowNeedItems()
    {
        for (int i=0;i <indexItems.Length;i++)
        {
            Image imageItem = panelNeedItems.GetChild(i).GetChild(0).GetComponent<Image>();
            imageItem.enabled = true;
            imageItem.sprite = networkManager.onlineItems[indexItems[i]].GetComponent<IInventoryItem>().Image;
            panelNeedItems.GetChild(i).GetComponent<ShowInfoItem>().item
                = networkManager.onlineItems[indexItems[i]].GetComponent<InventoryItemBase>();
        }
    }
}
