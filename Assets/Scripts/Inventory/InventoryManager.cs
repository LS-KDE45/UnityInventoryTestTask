using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class InventoryManager : MonoBehaviour
{
    //Inventory instance
    public static InventoryManager Instance;
    //Inventory panel
    public GameObject inventory;

    //Lists of each item type (currently implemented for only one object)
    public List<Item> weaponItems = new List<Item>();
    public List<Item> upgradeItems = new List<Item>();
    public List<Item> potionItems = new List<Item>();

    //Lists of each gameObjectType
    public List<GameObject> weaponObjects = new List<GameObject>();
    public List<GameObject> upgradeObjects = new List<GameObject>();
    public List<GameObject> potionObjects = new List<GameObject>();

    //GameObject of the item placed on the inventory panel
    public GameObject inventoryItem;

    //stores the item to allow upload
    private Item currentItem;

    //List View of each item type
    public Transform weaponContent;
    public Transform potionContent;
    public Transform upgradeContent;

    //Location of the each item on the packback
    public GameObject weaponLocation;
    public GameObject upgradeLocation;
    public GameObject potionLocation;

    //URL and Token for post request
    private string url = "https://wadahub.manerai.com/api/inventory/status";
    private string token = "kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP";

    //UnityEvent objects
    public UnityEvent foldingEvent;
    public UnityEvent retrievingEvent;

    //boolean to know if the inventory is currently opened
    public bool isInInventory = false;

    private void Awake()
    {
        //Inicialize the inventory
        if (Instance != null)
        {
            Destroy(this.gameObject.transform.parent.gameObject);
            return;
        }

        Instance = this;
        Instance.inventory.SetActive(false);
    }

    //Add listeners to the events for the upload
    private void Start()
    {
        foldingEvent.AddListener(UploadFolding);
        retrievingEvent.AddListener(UploadRetrieving);
    }

    //Adds the Item and the object to the inventory Lists
    public void Add(Item item, GameObject itemObject)
    {
        currentItem = item;
        //Invoke the event for the upload
        foldingEvent.Invoke();
        Rigidbody rigidbody = itemObject.GetComponent<Rigidbody>();
        Collider collider = itemObject.GetComponent<Collider>();
        //To each item type it adds that item to the corresponding item list and excludes all layers for collisions and activates smooth movement to the backpack
        switch (item.type)
        {
            case Item.ItemType.Weapon: 
                weaponItems.Add(item); 
                collider.excludeLayers = 1;
                StartCoroutine(WaitForPosition(itemObject,weaponLocation, rigidbody,0.5f));
                weaponObjects.Add(itemObject);  
                break;
            case Item.ItemType.Potion: 
                potionItems.Add(item); 
                collider.excludeLayers = 1;
                StartCoroutine(WaitForPosition(itemObject, potionLocation, rigidbody, 0.5f));
                potionObjects.Add(itemObject);
                break;
            case Item.ItemType.Upgrade:
                upgradeItems.Add(item); 
                collider.excludeLayers = 1;
                StartCoroutine(WaitForPosition(itemObject, upgradeLocation, rigidbody, 0.5f));
                upgradeObjects.Add(itemObject);
                break;
        }

    }
    ////Removes the Item and the object from the inventory Lists
    public void Remove(Item item)
    {
        currentItem = item;
        //Invoke the event for the upload
        retrievingEvent.Invoke();
        //To each item type it removes that item from the corresponding item list and includes all layers for collisions and enables that the gameObject is dragable
        switch (item.type)
        {
            case Item.ItemType.Weapon:
                weaponItems.Remove(item);
                RemoveObject(weaponObjects[0]);
                weaponObjects[0].GetComponent<DragDrop>().AbleDrag();
                weaponObjects.Remove(weaponObjects[0]);
                break;
            case Item.ItemType.Potion: 
                potionItems.Remove(item); 
                RemoveObject(potionObjects[0]);
                potionObjects[0].GetComponent<DragDrop>().AbleDrag();
                potionObjects.Remove(potionObjects[0]);
                break;
            case Item.ItemType.Upgrade: 
                upgradeItems.Remove(item); 
                RemoveObject(upgradeObjects[0]);
                upgradeObjects[0].GetComponent<DragDrop>().AbleDrag();
                upgradeObjects.Remove(upgradeObjects[0]);
                break;
        }

    }

    //Remove GameObject from parent and enables gravity and includes all layers to the collider
    public void RemoveObject(GameObject itemObject)
    {
        itemObject.transform.parent = null;
        Rigidbody rigidbody = itemObject.GetComponent<Rigidbody>();
        Collider collider = itemObject.GetComponent<Collider>();
        rigidbody.useGravity = true;
        collider.excludeLayers = 0;

    }

    //Lists all items in the inventory
    public void ListItems()
    {
        //Clears previous items in the inventory canvas
        foreach (Transform item in weaponContent)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in potionContent)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in upgradeContent)
        {
            Destroy(item.gameObject);
        }
        //foreach item in each item type list it instanciates the object with its icon and name 
        foreach (Item item in weaponItems)
        {
            var obj = Instantiate(inventoryItem, weaponContent);
            var itemName = obj.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();

            itemName.text = item.name;
            itemIcon.sprite = item.icon;
        }
        foreach (Item item in upgradeItems)
        {
            var obj = Instantiate(inventoryItem, upgradeContent);
            var itemName = obj.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();

            itemName.text = item.name;
            itemIcon.sprite = item.icon;
        }
        foreach (Item item in potionItems)
        {
            var obj = Instantiate(inventoryItem, potionContent);
            var itemName = obj.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();

            itemName.text = item.name;
            itemIcon.sprite = item.icon;
        }
    }

    //Allows smooth movement to the backpack
    public IEnumerator WaitForPosition(GameObject itemObject, GameObject itemPosition, Rigidbody rigidbody, float lerpSpeed)
    {
        float time = 0;
        float distance = 10f;
        while (time < 1 && distance > 0.7)
        {
            itemObject.transform.position = Vector3.Lerp(itemObject.transform.position, itemPosition.transform.position, time);
            time += Time.deltaTime * lerpSpeed;
            distance = Vector3.Distance(itemObject.transform.position, itemPosition.transform.position);
            yield return null;
        }
        //Disable gravity and velocity and puts the item in the correct backpack location
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = false;
        itemObject.transform.position = itemPosition.transform.position;
        itemObject.transform.SetParent(itemPosition.transform);
        itemObject.transform.localPosition = Vector3.zero;
    }

    //Finds the Item Object in the list
    public void FindItemIndex(int currentTypeIndex, int currentIndex)
    {
        switch (currentTypeIndex)
        {
            case 0://weaponitem
                Remove(weaponItems[currentIndex]);
                ListItems();
                break;
            case 1://upgrade
                Remove(upgradeItems[currentIndex]);
                ListItems();
                break;
            case 2: //potions
                Remove(potionItems[currentIndex]);
                ListItems();
                break;
        }
    }

    //Starts the Upload Coroutine for retrieving an item
    private void UploadRetrieving()
    {
        StartCoroutine(Upload("Retrieving"));
    }
    //Starts the Upload Coroutine for folding an item
    private void UploadFolding()
    {
        StartCoroutine(Upload("Folding"));
    }

    //Coroutine for Uploading the information through post request
    IEnumerator Upload(string eventType)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        //Object creation for upload
        string jsonData = JsonUtility.ToJson(new ItemUpload(currentItem.identifier, eventType));

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        request.SetRequestHeader("Content-Type", "application/json");

        request.SetRequestHeader("Authorization", "Bearer " + token);

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    //Class used for uploading the id and the event
    public class ItemUpload
    {
        public uint id;
        public string eventType;

        public ItemUpload(uint id, string eventType)
        {
            this.id = id;
            this.eventType = eventType;
        }
    }
}
