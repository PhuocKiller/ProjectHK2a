using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Camera _camera;
    private void Start()
    {
       // _camera ??= FindObjectOfType<CameraUI>().gameObject.GetComponent<Camera>();
    }
    public IInventoryItem Item { get; set; }
    public void OnDrag(PointerEventData eventData)
    {
      /* Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(Input.mousePosition);
        Debug.Log(worldPoint);*/
        // transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
        transform.position=Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;

    }

}
