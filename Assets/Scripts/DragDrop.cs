using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script related to all items that you can drag and drop
public class DragDrop : MonoBehaviour
{
    Vector3 mousePosition;
    [SerializeField] GameObject backpack;
    CapsuleCollider backpackCollider;
    Item item;

    //Boolean to detect if the item is already in the backpack in order to not drag it while it is in the inventory
    private bool dragable = true;

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

    //Change the position from the click to the new position
    private void OnMouseDrag()
    {
        if (dragable) 
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        }
    }

    //Hit detection from the dragged item with the backpack
    private void OnMouseUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(GetMousePos());
        RaycastHit hit;

        if (backpackCollider.Raycast(ray, out hit, Mathf.Infinity) && dragable)
        {
            dragable = false;
            InventoryManager.Instance.Add(item,gameObject);
        }
    }

    public void AbleDrag()
    {
        dragable = true;
    }
}
