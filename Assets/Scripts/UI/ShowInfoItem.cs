using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInfoItem : MonoBehaviour
{
    public InventoryItemBase item;
    public ItemsManager allItems;
    public void ShowInfoOfItem()
    {
        allItems.ShowInfoItem(item);
    }
}
