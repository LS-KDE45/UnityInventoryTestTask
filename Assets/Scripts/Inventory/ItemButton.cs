using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

//Script related to hit detection from the mouse with items in the inventory
public class ItemButton : MonoBehaviour
{
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private GameObject tempSelectedItem;
    private BoxCollider2D collider;
    private List<RaycastResult> results = new List<RaycastResult>();

    void Start()
    {
        // Get the GraphicRaycaster and EventSystem from the canvas
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        //if the mouse was realeased disable canvas and clear the results from pointerEventData and deletion of item in inventory
        if (Input.GetMouseButtonUp(0))
        {
            InventoryManager.Instance.isInInventory = false;
            InventoryManager.Instance.inventory.SetActive(false);
            if(tempSelectedItem != null)
            {
                results.Clear();
                //index to know what the item type
                int currentTypeIndex = tempSelectedItem.transform.parent.parent.GetSiblingIndex();
                int currentIndex = tempSelectedItem.transform.GetSiblingIndex();
                InventoryManager.Instance.FindItemIndex(currentTypeIndex, 0);
            }
        }
        if (Input.GetMouseButton(0))
        {
            DetectUIElement();
        }
    }

    //Detection of current pointed items through pointerEventData
    public void DetectUIElement()
    {
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        raycaster.Raycast(pointerEventData, results);

        if(results != null)
        {
            foreach (var result in results)
            {
                //variable to allow time for the raycast to hit the canvas due to another script using onMouseUp
                InventoryManager.Instance.isInInventory = true;
                //Item gameObject
                collider = result.gameObject.GetComponentInParent<BoxCollider2D>();
                if (collider != null)
                {
                    //Time delay to check keep the data from the last item that was hit for 0.2s
                    tempSelectedItem = collider.gameObject;
                    WaitForItem();
                }
            }
        }
    }

    IEnumerator WaitForItem()
    {
        yield return new WaitForSeconds(0.2f);

        tempSelectedItem = null;
        
    }
}
