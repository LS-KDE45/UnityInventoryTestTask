using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackpackMouse : MonoBehaviour
{
    CapsuleCollider backpackCollider;

    private void Awake()
    {
        InventoryManager.Instance.inventory.SetActive(false);
        backpackCollider = GetComponent<CapsuleCollider>();
        if(backpackCollider == null)
        {
            Debug.Log("Error");
        }
    }
    private void OnMouseDown()
    {
        if (IsMouseOverCapsule())
        {
            InventoryManager.Instance.inventory.SetActive(true);
            InventoryManager.Instance.ListItems();
        }
    }

    private void OnMouseUp()
    {
        if(!InventoryManager.Instance.isInInventory)
        {
            InventoryManager.Instance.inventory.SetActive(false);
        }
    }
    private bool IsMouseOverCapsule()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (backpackCollider.Raycast(ray, out hit, Mathf.Infinity))
        {
            return true;
        }
        return false;
    }
}
