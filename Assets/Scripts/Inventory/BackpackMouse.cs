using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Script related to hit detection from the mouse to the backpack
public class BackpackMouse : MonoBehaviour
{
    CapsuleCollider backpackCollider;

    private void Awake()
    {
        backpackCollider = GetComponent<CapsuleCollider>();
        if(backpackCollider == null)
        {
            Debug.Log("Error");
        }
    }

    //If There's a hit then the canvas inventory is set to active and the items are listed
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
    //Hit Detection from Mouse Positon
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
