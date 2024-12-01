using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemClickHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    
    public void OnItemClicked()
    {
        ItemDragHandler dragHandler = gameObject.transform.Find("ItemImage").GetComponent<ItemDragHandler>();
        IInventoryItem item = dragHandler.Item;
        if (item != null)
        {
            Singleton<Inventory>.Instance.UseItemClickInventory(item);
        }
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        if(Singleton<Inventory>.Instance.buyItemPanel.activeInHierarchy)
        {
            GetComponent<Image>().color = Color.green;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
    }
}
