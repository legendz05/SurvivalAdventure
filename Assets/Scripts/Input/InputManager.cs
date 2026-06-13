using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;

    private Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;

    public bool jumpInput;

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
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }

    public void HandleResetJump()
    {
        jumpInput = false;
    }
}