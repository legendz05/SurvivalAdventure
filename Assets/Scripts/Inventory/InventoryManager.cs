using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private InputManager inputManager;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    public bool inventoryOpen = false;

    public InventorySlot highlightedSlot;
    public InventorySlot selectedSlot;

    public GameObject hotbarObj;
    public GameObject inventoryObj;

    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public List<InventorySlot> hotbarSlots = new List<InventorySlot>();

    public Dictionary<Item, InventorySlot> inventory = new Dictionary<Item, InventorySlot>();

    private int hotbarSlot = -1;
    public Item pickedUpItem;
    GameObject pickedItem;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        inputManager = FindAnyObjectByType<InputManager>();

        graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        inventoryObj.SetActive(false);
    }

    private void Stuff()
    {
        foreach (InventorySlot slot in inventorySlots)
        {

        }
    }

    public void OpenInventory()
    {
        inventoryObj.SetActive(true);
        inputManager.EnableInventoryControls();
        inputManager.ToggleCursorVisibility(true);
        inventoryOpen = true;
    }

    public void CloseInventory()
    {
        inputManager.DisableInventoryControls();
        inputManager.ToggleCursorVisibility(false);
        inventoryObj.SetActive(false);
        inventoryOpen = false;
    }

    private void Update()
    {
        if (inventoryOpen)
        {
            InventoryRayCast();

            if (pickedUpItem != null)
            {
                ItemTrackMouse();
            }
        }
        else
        {
            inputManager.TryGetHotbarInput(out hotbarSlot);
            SelectHotBarSlot(hotbarSlot);
        }
    }

    //Picked up item will track mouse position
    private void ItemTrackMouse()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        pickedItem.transform.position = pointerData.position;
    }

    // use mouse position to determine which slot is highlighted in the inventory
    private void InventoryRayCast()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();

        graphicRaycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent(out InventorySlot slot))
            {
                // Debug.Log("Slot");
                highlightedSlot = slot;
                return;
            }
            else
            {
                highlightedSlot = null;
            }
        }
    }

    // Select a slot in inventory screen
    public void SelectSlot(InventorySlot slot)
    {
        if (slot == null) return;

        slot.OnSelect();

        if (slot.itemInSlot != null)
        {
            pickedUpItem = slot.itemInSlot;
            CreateItemIconTracker(out pickedItem);
        }

        Debug.Log("Selected Slot");
    }

    private void CreateItemIconTracker(out GameObject pickedItem)
    {
        GameObject pickedItemInstance = new GameObject();
        pickedItemInstance.AddComponent<SpriteRenderer>().sprite = pickedUpItem.itemIcon;

        pickedItem = pickedItemInstance;
    }

    // Select a hotbar slot outside of inventory
    private void SelectHotBarSlot(int slot)
    {
        if (slot >= 0 && slot < hotbarSlots.Count)
            highlightedSlot = hotbarSlots[slot];
    }

    // add an item to the inventory
    public void AddItem(Item item, InventorySlot designatedSlot = null)
    {
        // if no designated slot then pick first empty slot
        if (designatedSlot == null)
        {
            foreach (InventorySlot hotbarSlot in hotbarSlots)
            {
                if (hotbarSlot.itemInSlot == null)
                {
                    hotbarSlot.UpdateSlot(item);
                    return;
                }
            }

            foreach (InventorySlot inventorySlot in inventorySlots)
            {
                if (inventorySlot.itemInSlot == null)
                {
                    inventorySlot.UpdateSlot(item);
                    return;
                }
            }
        }

    }

    public void RemoveItem(Item item, InventorySlot slot)
    {
        if (slot == null) return;

        slot.UpdateSlot(null);

    }
}