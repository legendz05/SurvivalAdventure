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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        inputManager = FindAnyObjectByType<InputManager>();

        graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        gameObject.SetActive(false);
    }

    public void OpenInventory()
    {
        gameObject.SetActive(true);
        inputManager.EnableInventoryControls();
        inputManager.ToggleCursorVisibility(true);
        inventoryOpen = true;
    }

    public void CloseInventory()
    {
        inputManager.DisableInventoryControls();
        inputManager.ToggleCursorVisibility(false);
        gameObject.SetActive(false);
        inventoryOpen = false;
    }

    private void Update()
    {
        if (inventoryOpen)
        {
            InventoryRayCast();
        }
    }

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

    public void SelectSlot(InventorySlot slot)
    {
        if (slot == null) return;

        slot.OnSelect();
    }
}