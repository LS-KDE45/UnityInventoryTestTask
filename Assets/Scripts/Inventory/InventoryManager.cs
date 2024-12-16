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
    public static InventoryManager Instance;

    public GameObject inventory;

    public List<Item> weaponItems = new List<Item>();
    public List<Item> upgradeItems = new List<Item>();
    public List<Item> potionItems = new List<Item>();

    public List<GameObject> weaponObjects = new List<GameObject>();
    public List<GameObject> upgradeObjects = new List<GameObject>();
    public List<GameObject> potionObjects = new List<GameObject>();

    public GameObject inventoryItem;
    private Item currentItem;

    public Transform weaponContent;
    public Transform potionContent;
    public Transform upgradeContent;

    public GameObject weaponLocation;
    public GameObject upgradeLocation;
    public GameObject potionLocation;

    public int weaponCount = 0;
    public int potionCount = 0;
    public int upgradeCount = 0;

    private string url = "https://wadahub.manerai.com/api/inventory/status";
    private string token = "kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP";

    public UnityEvent foldingEvent;
    public UnityEvent retrievingEvent;

    public bool isInInventory = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject.transform.parent.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        foldingEvent.AddListener(UploadFolding);
        retrievingEvent.AddListener(UploadRetrieving);
    }

    public void Add(Item item, GameObject itemObject)
    {
        currentItem = item;
        foldingEvent.Invoke();
        Rigidbody rigidbody = itemObject.GetComponent<Rigidbody>();
        Collider collider = itemObject.GetComponent<Collider>();
        switch (item.type)
        {
            case Item.ItemType.Weapon: 
                weaponItems.Add(item); 
                weaponCount++;
                collider.excludeLayers = 1;
                StartCoroutine(WaitForPosition(itemObject,weaponLocation, rigidbody,0.5f));
                weaponObjects.Add(itemObject);  
                break;
            case Item.ItemType.Potion: 
                potionItems.Add(item); 
                potionCount++;
                collider.excludeLayers = 1;
                StartCoroutine(WaitForPosition(itemObject, potionLocation, rigidbody, 0.5f));
                potionObjects.Add(itemObject);
                break;
            case Item.ItemType.Upgrade:
                upgradeItems.Add(item); 
                upgradeCount++;
                collider.excludeLayers = 1;
                StartCoroutine(WaitForPosition(itemObject, upgradeLocation, rigidbody, 0.5f));
                upgradeObjects.Add(itemObject);
                break;
        }

    }
    public void Remove(Item item)
    {
        currentItem = item;
        retrievingEvent.Invoke();
        switch (item.type)
        {
            case Item.ItemType.Weapon: 
                weaponItems.Remove(item); 
                weaponCount--;
                RemoveObject(weaponObjects[0]);
                weaponObjects.Remove(weaponObjects[0]);
                break;
            case Item.ItemType.Potion: 
                potionItems.Remove(item); 
                potionCount--;
                RemoveObject(potionObjects[0]);
                potionObjects.Remove(potionObjects[0]);
                break;
            case Item.ItemType.Upgrade: 
                upgradeItems.Remove(item); 
                upgradeCount--;
                RemoveObject(upgradeObjects[0]);
                upgradeObjects.Remove(upgradeObjects[0]);
                break;
        }

    }

    public void RemoveObject(GameObject itemObject)
    {
        itemObject.transform.parent = null;
        Rigidbody rigidbody = itemObject.GetComponent<Rigidbody>();
        Collider collider = itemObject.GetComponent<Collider>();
        rigidbody.useGravity = true;
        collider.excludeLayers = 0;

    }
    public void ListItems()
    {

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
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = false;
        itemObject.transform.position = itemPosition.transform.position;
        itemObject.transform.SetParent(itemPosition.transform);
        itemObject.transform.localPosition = Vector3.zero;
    }

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

    private void UploadRetrieving()
    {
        StartCoroutine(Upload("Retrieving"));
    }
    private void UploadFolding()
    {
        StartCoroutine(Upload("Folding"));
    }

    IEnumerator Upload(string eventType)
    {
        // Create a UnityWebRequest for a POST request
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        string jsonData = JsonUtility.ToJson(new ItemUpload(currentItem.identifier, eventType));

        // Convert JSON data to a byte array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Set the content type to JSON
        request.SetRequestHeader("Content-Type", "application/json");

        // Add the Bearer token to the Authorization header
        request.SetRequestHeader("Authorization", "Bearer " + token);

        // Download handler to capture the response
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        // Check the result
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

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
