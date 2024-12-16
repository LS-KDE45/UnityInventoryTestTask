using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
        // Get the GraphicRaycaster and EventSystem from the Canvas
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            InventoryManager.Instance.isInInventory = false;
            InventoryManager.Instance.inventory.SetActive(false);
            if(tempSelectedItem != null)
            {
                results.Clear();
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

    public void DetectUIElement()
    {
        // Create PointerEventData for the current mouse position
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // Perform the raycast
        raycaster.Raycast(pointerEventData, results);

        if(results != null)
        {
            foreach (var result in results)
            {
                InventoryManager.Instance.isInInventory = true;
                collider = result.gameObject.GetComponentInParent<BoxCollider2D>();
                if (collider != null)
                {
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
