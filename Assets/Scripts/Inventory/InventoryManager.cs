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
    [SerializeField] bool pickupStack = false;

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

            RemoveItem(pickedUpItem, slot, pickedAmount);
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
    }

    // Add an item to the inventory
    public void AddItem(Item item, InventorySlot designatedSlot = null, int amount = 0)
    {
        if (item == null) return;

        // Designated slot
        if (designatedSlot != null)
        {
            designatedSlot.UpdateSlot(item, designatedSlot.amountInSlot + amount);
            return;
        }

        // Automatic slot
        foreach (InventorySlot hotbarSlot in hotbarSlots)
        {
            if (hotbarSlot.itemInSlot != item)
            {
                hotbarSlot.UpdateSlot(item, hotbarSlot.amountInSlot + amount);
                return;
            }
        }

        foreach (InventorySlot inventorySlot in inventorySlots)
        {
            if (inventorySlot.itemInSlot != item)
            {
                inventorySlot.UpdateSlot(item, inventorySlot.amountInSlot + amount);
                return;
            }
        }
    }

    public void DropItem()
    {
        if (highlightedSlot == null) return;

        if (pickupStack)
            RemoveItem(highlightedSlot.itemInSlot, highlightedSlot, highlightedSlot.amountInSlot);
        else
            RemoveItem(highlightedSlot.itemInSlot, highlightedSlot, 1);

        ThrowItemOnGround(highlightedSlot.itemInSlot);
    }

    private void ThrowItemOnGround(Item item)
    {
        item.transform.position = inputManager.transform.position;
        item.gameObject.SetActive(true);
        item.TryGetComponent(out Rigidbody rb);
        rb.AddForce(Vector3.forward * 5, ForceMode.Impulse);
    }

    public void RemoveItem(Item item, InventorySlot slot, int amount)
    {
        if (slot == null) return;

        if (slot.amountInSlot - amount <= 0)
        {
            item = null;
        }

        slot.UpdateSlot(item, slot.amountInSlot - amount);
    }
}