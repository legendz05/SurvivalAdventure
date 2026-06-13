using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;

    private Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;

    public bool jumpInput;
    public bool interactInput;

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i =>
                movementInput = i.ReadValue<Vector2>();

            playerControls.PlayerMovement.Movement.canceled += i =>
                movementInput = Vector2.zero;

            playerControls.PlayerMovement.Jump.performed += i =>
                jumpInput = i.ReadValueAsButton();

            playerControls.PlayerMovement.Jump.canceled += i =>
              jumpInput = i.ReadValueAsButton();

            playerControls.PlayerAction.Interact.performed += i =>
              interactInput = i.ReadValueAsButton();

            playerControls.PlayerAction.Interact.canceled += i =>
             interactInput = i.ReadValueAsButton();
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }

    public void HandleResetJump() => jumpInput = false;
    public void HandleResetInteract() => interactInput = false;

    public bool HasPressedInteract()
    {
        return interactInput;
    }
}