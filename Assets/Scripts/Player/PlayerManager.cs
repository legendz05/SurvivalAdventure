using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputManager inputManager;
    private PlayerMovement playerMovement;
    private PlayerCollision playerCollision;
    private PlayerAction action;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCollision = GetComponent<PlayerCollision>();
        action = GetComponent<PlayerAction>();
    }

    private void Update()
    {
        if (inputManager.openInventoryInput && !InventoryManager.Instance.inventoryOpen)
            InventoryManager.Instance.OpenInventory();

        if (inputManager.closeInventoryInput && InventoryManager.Instance.inventoryOpen)
            InventoryManager.Instance.CloseInventory();

        inputManager.HandleMovementInput();

        if (inputManager.HasPressedInteract())
        {
            action.Interact();
            inputManager.HandleResetInteract();
        }

        if (inputManager.HasPressedExitInteract())
        {
            action.CloseInteraction();
            inputManager.HandleResetCloseInteract();
        }

        playerMovement.HandleSpeed(inputManager.IsHoldingSprintInput());
    }

    private void FixedUpdate()
    {
        playerMovement.HandleAllMovement();

        if (inputManager.jumpInput && playerCollision.isGrounded)
        {
            playerMovement.HandleJump();
            inputManager.HandleResetJump();
        }
    }
}