using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    Vector3 mousePosition;
    [SerializeField] GameObject backpack;
    CapsuleCollider backpackCollider;
    Item item;

    private void Awake()
    {
        item = gameObject.GetComponent<ItemController>().item;
        backpackCollider = backpack.GetComponent<CapsuleCollider>();
    }

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePos();
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }

    private void OnMouseUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(GetMousePos());
        RaycastHit hit;

        if (backpackCollider.Raycast(ray, out hit, Mathf.Infinity))
        {
            InventoryManager.Instance.Add(item,gameObject);
        }
    }
}
