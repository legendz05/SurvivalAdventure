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

    public Item pickedUpItem;
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

        inventoryObj.SetActive(false);
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

    // Select a slot in inventory screen
    public void SelectSlot(InventorySlot slot)
    {
        if (slot == null) return;

        slot.OnSelect();

        if (slot.itemInSlot != null && !pickedUpItem)
        {
            pickedUpItem = slot.itemInSlot;
            pickedAmount = slot.amountInSlot;

            CreateItemIconTracker();

            RemoveItem(pickedUpItem, slot);
        }
        else
        {
            AddItem(pickedUpItem, slot, pickedAmount);
            DestroyPickedItemIcon();
            pickedUpItem = null;
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
    }

    // Add an item to the inventory
    public void AddItem(Item item, InventorySlot designatedSlot = null, int amount = 0)
    {
        if (item == null) return;

        // Designated slot
        if (designatedSlot != null)
        {
            designatedSlot.UpdateSlot(item, amount);
            return;
        }

        // Automatic slot
        foreach (InventorySlot hotbarSlot in hotbarSlots)
        {
            if (hotbarSlot.itemInSlot != item)
            {
                hotbarSlot.UpdateSlot(item, amount);
                return;
            }
        }

        foreach (InventorySlot inventorySlot in inventorySlots)
        {
            if (inventorySlot.itemInSlot != item)
            {
                inventorySlot.UpdateSlot(item, amount);
                return;
            }
        }
    }

    public void RemoveItem(Item item, InventorySlot slot)
    {
        if (slot == null) return;

        slot.UpdateSlot(null);
    }
}