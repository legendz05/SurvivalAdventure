using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;

    private Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;

    public bool jumpInput;
    public bool interactInput;
    public bool escapeInteractInput;
    public bool sprintInput;

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
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void DisableMovement() => playerControls.PlayerMovement.Disable();
    public void EnableMovement() => playerControls.PlayerMovement.Enable();
    public void ToggleCursorVisibility(bool visible) => Cursor.visible = visible;


    public void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }

    public void HandleResetJump() => jumpInput = false;
    public void HandleResetInteract() => interactInput = false;
    public void HandleResetCloseInteract() => escapeInteractInput = false;

    public bool HasPressedInteract()
    { return interactInput; }

    public bool HasPressedExitInteract()
    { return escapeInteractInput; }

    public bool IsHoldingSprintInput()
    { return sprintInput; }
}