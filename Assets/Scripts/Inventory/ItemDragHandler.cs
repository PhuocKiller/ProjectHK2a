using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemDragHandler : MonoBehaviour
{
    private Camera _camera;
    RectTransform _rectTransform;
    Vector2 localPoint;
    
    private void Start()
    {
    }
    public IInventoryItem Item { get; set; }
    public void OnBeginDrag( )
    {
        Singleton<Inventory>.Instance.indexItemSlot_1 = Item.Slot.Id;
        Singleton<Inventory>.Instance.OnItemBeginDrag?.Invoke(this);
    }

    public void OnEndDrag()
    {
        Singleton<Inventory>.Instance.OnItemEndDrag?.Invoke(this);    
    }

    public void OnPointerClick( )
    {
        Singleton<Inventory>.Instance.OnItemClicked?.Invoke(this);
    }
    
    public void OnDrop()
    {
        Singleton<Inventory>.Instance.indexItemSlot_2 = Item.Slot.Id;
        Singleton<Inventory>.Instance.SwapItem();
    }
    
   
}
