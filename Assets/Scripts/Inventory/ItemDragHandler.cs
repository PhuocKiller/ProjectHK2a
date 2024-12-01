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
    public Action<ItemDragHandler> OnItemClicked, OnItemBeginDrag, OnItemDroppedOn, OnItemEndDrag, OnRightMouseBtnClick;
    private void Start()
    {
    }
    public IInventoryItem Item { get; set; }
    public void OnBeginDrag( )
    {
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnEndDrag()
    {
        OnItemEndDrag?.Invoke(this);    
    }

    public void OnPointerClick( )
    {
        OnItemClicked?.Invoke(this);
    }
    
    public void OnDrop()
    {
        Debug.Log("voday" + transform.parent.parent.name);
        OnItemDroppedOn?.Invoke(this);  
    }
    
   
}
