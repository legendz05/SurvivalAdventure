using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;

    Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;

    public bool jumpToggle = false;


    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Movement.canceled += i => movementInput = Vector2.zero;
            playerControls.PlayerMovement.Jump.performed += i => jumpToggle = i.ReadValueAsButton();
            playerControls.PlayerMovement.Jump.canceled += i => jumpToggle = false;
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
        HandleJump();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }

    private bool HandleJump()
    {
        if (jumpToggle)
            return true;

        return false;
    }
}
