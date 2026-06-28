using NUnit.Framework.Interfaces;
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
    private PlayerEquipment playerEquipment;
    private EventSystem eventSystem;
    [SerializeField] private Item worldItemPrefab;

    private Canvas canvas;
    private RectTransform canvasRect;

    public bool inventoryOpen = false;

    public InventorySlot highlightedSlot;
    public InventorySlot selectedSlot;

    public GameObject hotbarObj;
    public GameObject inventoryObj;

    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public List<InventorySlot> hotbarSlots = new List<InventorySlot>();

    public Dictionary<Item, InventorySlot> inventory = new Dictionary<Item, InventorySlot>();

    private int hotbarSlot = -1;
    int pickedAmount = 0;
    [SerializeField] bool pickupStack = false;

    public ItemData pickedUpItem;
    private GameObject pickedItem;
    private RectTransform pickedItemRect;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        inputManager = FindAnyObjectByType<InputManager>();

        graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        playerEquipment = FindAnyObjectByType<PlayerEquipment>();

        inventoryObj.SetActive(false);
        pickedAmount = 0;
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

        DestroyPickedItemIcon();
    }

    private void Update()
    {
        if (inventoryOpen)
        {
            InventoryRayCast();

            if (pickedUpItem != null && pickedItem != null)
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

    // Picked up item follows mouse position inside the UI canvas
    private void ItemTrackMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePosition,
            uiCamera,
            out Vector2 localPoint
        );

        pickedItemRect.anchoredPosition = localPoint;
    }

    // Use mouse position to determine which slot is highlighted in the inventory
    private void InventoryRayCast()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        highlightedSlot = null;

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent(out InventorySlot slot))
            {
                highlightedSlot = slot;
                return;
            }
        }
    }

    public void ToggleStackPickup(bool trueOrFalse)
    {
        pickupStack = trueOrFalse;
    }

    // Select a slot in inventory screen
    public void SelectSlot(InventorySlot slot)
    {
        if (slot == null) return;

        slot.OnSelect();

        if (slot.itemInSlot != null && !pickedUpItem)
        {
            pickedUpItem = slot.itemInSlot;

            if (pickupStack)
            {
                pickedAmount = slot.amountInSlot;
            }
            else
            {
                pickedAmount = Mathf.Clamp(pickedAmount + 1, 0, slot.amountInSlot);
            }


            CreateItemIconTracker();

            RemoveItem(slot, pickedAmount);
        }
        else
        {
            AddItem(pickedUpItem, slot, pickedAmount);
            DestroyPickedItemIcon();
            pickedUpItem = null;
            pickedAmount = 0;
        }

        Debug.Log("Selected Slot");
    }

    // Create an icon of a picked up item that tracks the mouse
    private void CreateItemIconTracker()
    {
        DestroyPickedItemIcon();

        pickedItem = new GameObject("Picked Up Item Icon");

        pickedItem.transform.SetParent(canvas.transform, false);

        pickedItemRect = pickedItem.AddComponent<RectTransform>();
        pickedItemRect.sizeDelta = new Vector2(64f, 64f);

        Image image = pickedItem.AddComponent<Image>();
        image.sprite = pickedUpItem.itemIcon;
        image.raycastTarget = false; // prevents dragged icon blocking slot raycasts

        pickedItem.transform.SetAsLastSibling();

        ItemTrackMouse();
    }

    private void DestroyPickedItemIcon()
    {
        if (pickedItem != null)
        {
            Destroy(pickedItem);
            pickedItem = null;
            pickedItemRect = null;
        }
    }

    // Select a hotbar slot outside of inventory
    private void SelectHotBarSlot(int slot)
    {
        if (slot >= 0 && slot < hotbarSlots.Count)
            highlightedSlot = hotbarSlots[slot];

        if (playerEquipment != null)
        {
            // playerEquipment.EquipToRightHand();
        }
    }

    // Add an item to the inventory
    public bool AddItem(ItemData item, InventorySlot designatedSlot = null, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        // If placing into a specific slot
        if (designatedSlot != null)
        {
            // Empty slot
            if (designatedSlot.itemInSlot == null)
            {
                int amountToAdd = item.maxStack > 1 ? Mathf.Min(item.maxStack, amount) : 1;

                designatedSlot.UpdateSlot(item, amountToAdd);
                return amountToAdd == amount;
            }

            // Same item, stack it
            if (designatedSlot.itemInSlot == item)
            {
                int spaceLeft = item.maxStack - designatedSlot.amountInSlot;
                int amountToAdd = Mathf.Min(spaceLeft, amount);

                if (amountToAdd <= 0)
                    return false;

                designatedSlot.UpdateSlot(item, designatedSlot.amountInSlot + amountToAdd);
                return amountToAdd == amount;
            }

            // Different item in slot
            return false;
        }

        // First: stack into existing hotbar slots
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.itemInSlot == item)
            {
                int spaceLeft = item.maxStack - slot.amountInSlot;
                int amountToAdd = Mathf.Min(spaceLeft, amount);

                if (amountToAdd <= 0)
                    continue;

                slot.UpdateSlot(item, slot.amountInSlot + amountToAdd);
                amount -= amountToAdd;

                if (amount <= 0)
                    return true;
            }
        }

        // Then: stack into existing inventory slots
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.itemInSlot == item)
            {
                int spaceLeft = item.maxStack - slot.amountInSlot;
                int amountToAdd = Mathf.Min(spaceLeft, amount);

                if (amountToAdd <= 0)
                    continue;

                slot.UpdateSlot(item, slot.amountInSlot + amountToAdd);
                amount -= amountToAdd;

                if (amount <= 0)
                    return true;
            }
        }

        // Then: empty hotbar slots
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.itemInSlot == null)
            {
                int amountToAdd = item.maxStack > 1 ? Mathf.Min(item.maxStack, amount) : 1;

                slot.UpdateSlot(item, amountToAdd);
                amount -= amountToAdd;

                if (amount <= 0)
                    return true;
            }
        }

        // Then: empty inventory slots
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.itemInSlot == null)
            {
                int amountToAdd = item.maxStack > 1 ? Mathf.Min(item.maxStack, amount) : 1;

                slot.UpdateSlot(item, amountToAdd);
                amount -= amountToAdd;

                if (amount <= 0)
                    return true;
            }
        }

        return false;
    }

    public void DropItem()
    {
        if (highlightedSlot == null) return;
        if (highlightedSlot.itemInSlot == null) return;

        ItemData itemToDrop = highlightedSlot.itemInSlot;

        int amountToDrop = pickupStack ? highlightedSlot.amountInSlot : 1;

        amountToDrop = Mathf.Clamp(amountToDrop, 1, highlightedSlot.amountInSlot);

        RemoveItem(highlightedSlot, amountToDrop);

        ThrowItemOnGround(itemToDrop, amountToDrop);
    }

    private void ThrowItemOnGround(ItemData itemData, int amount)
    {
        if (itemData == null) return;
        if (worldItemPrefab == null) return;

        Vector3 spawnPosition = inputManager.transform.position + inputManager.transform.forward * 1.25f;

        Item droppedItem = Instantiate(
            worldItemPrefab,
            spawnPosition,
            Quaternion.identity
        );

        droppedItem.Initialize(itemData, amount, 2f);

        if (droppedItem.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(inputManager.transform.forward * 5f, ForceMode.Impulse);
        }
    }

    public void RemoveItem(InventorySlot slot, int amount)
    {
        if (slot == null) return;
        if (slot.itemInSlot == null) return;
        if (amount <= 0) return;

        int newAmount = slot.amountInSlot - amount;

        if (newAmount <= 0)
        {
            slot.UpdateSlot(null, 0);
        }
        else
        {
            slot.UpdateSlot(slot.itemInSlot, newAmount);
        }
    }
}