using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public CinemachineInputAxisController camController;
    private PlayerControls playerControls;

    private Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;

    public bool jumpInput;
    public bool interactInput;
    public bool escapeInteractInput;
    public bool sprintInput;
    public bool openInventoryInput;
    public bool closeInventoryInput;
    public bool selectInput;

    public int hotbarInput = -1;

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Movement.canceled += i => movementInput = Vector2.zero;

            playerControls.PlayerMovement.Jump.performed += i => jumpInput = i.ReadValueAsButton();
            playerControls.PlayerMovement.Jump.canceled += i => jumpInput = i.ReadValueAsButton();

            playerControls.PlayerAction.Interact.performed += i => interactInput = i.ReadValueAsButton();
            playerControls.PlayerAction.Interact.canceled += i => interactInput = i.ReadValueAsButton();

            playerControls.PlayerAction.CloseInteraction.performed += i => escapeInteractInput = i.ReadValueAsButton();
            playerControls.PlayerAction.CloseInteraction.canceled += i => escapeInteractInput = i.ReadValueAsButton();

            playerControls.PlayerMovement.Sprint.performed += i => sprintInput = i.ReadValueAsButton();
            playerControls.PlayerMovement.Sprint.canceled += i => sprintInput = i.ReadValueAsButton();

            playerControls.PlayerAction.OpenInventory.performed += i => openInventoryInput = i.ReadValueAsButton();
            playerControls.PlayerAction.OpenInventory.canceled += i => openInventoryInput = i.ReadValueAsButton();
            playerControls.Inventory.CloseInventory.performed += i => closeInventoryInput = i.ReadValueAsButton();
            playerControls.Inventory.CloseInventory.canceled += i => closeInventoryInput = i.ReadValueAsButton();

            playerControls.Inventory.Select.performed += i => selectInput = i.ReadValueAsButton();
            playerControls.Inventory.Select.canceled += i => selectInput = i.ReadValueAsButton();

            playerControls.PlayerAction.HotBar.performed += OnHotbarPressed;
        }

        playerControls.Enable();
        playerControls.Inventory.Disable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void DisableMovement() => playerControls.PlayerMovement.Disable();
    public void EnableMovement() => playerControls.PlayerMovement.Enable();
    public void DisableActions() => playerControls.PlayerAction.Disable();
    public void EnableActions() => playerControls.PlayerAction.Enable();
    public void ToggleCursorVisibility(bool visible) => Cursor.visible = visible;

    public void EnableInventoryControls()
    {
        playerControls.Inventory.Enable();
        DisableMovement();
        DisableActions();
        DisableCameraLook();
    }

    public void DisableInventoryControls()
    {
        playerControls.Inventory.Disable();
        EnableActions();
        EnableMovement();
        EnableCameraLook();
    }

    public void DisableCameraLook()
    {
        camController.enabled = false;
    }

    public void EnableCameraLook()
    {
        camController.enabled = true;
    }

    public void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }

    private void OnHotbarPressed(InputAction.CallbackContext context)
    {
        string keyName = context.control.name;

        switch (keyName)
        {
            case "1": hotbarInput = 0; break;
            case "2": hotbarInput = 1; break;
            case "3": hotbarInput = 2; break;
            case "4": hotbarInput = 3; break;
            case "5": hotbarInput = 4; break;
            case "6": hotbarInput = 5; break;
            case "7": hotbarInput = 6; break;
            case "8": hotbarInput = 7; break;
            case "9": hotbarInput = 8; break;
        }
    }

    public bool TryGetHotbarInput(out int slotIndex)
    {
        if (hotbarInput >= 0)
        {
            slotIndex = hotbarInput;
            hotbarInput = -1;
            return true;
        }

        slotIndex = -1;
        return false;
    }

    public void HandleResetJump() => jumpInput = false;
    public void HandleResetInteract() => interactInput = false;
    public void HandleResetCloseInteract() => escapeInteractInput = false;
    public void HandleResetSelect() => selectInput = false;

    public bool HasPressedInteract()
    { return interactInput; }

    public bool HasPressedExitInteract()
    { return escapeInteractInput; }

    public bool IsHoldingSprintInput()
    { return sprintInput; }

    public bool HasPressedSelect()
    { return selectInput; }
}