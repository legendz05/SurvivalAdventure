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

        HandlePlayerMovement();

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

        if (inputManager.HasPressedSelect())
        {
            InventoryManager.Instance.SelectSlot(InventoryManager.Instance.highlightedSlot);
            inputManager.HandleResetSelect();
        }
    }

    private void FixedUpdate()
    {
        playerMovement.HandleAllMovement();

        if (inputManager.jumpInput && playerCollision.isGrounded)
        {
            playerMovement.HandleJump();
            AnimationManager.PlayAnimation(gameObject.transform.GetChild(0).gameObject, "Jump");
            inputManager.HandleResetJump();
        }
    }

    private void HandlePlayerMovement()
    {
        inputManager.HandleMovementInput();

        bool isMoving = Mathf.Abs(inputManager.horizontalInput) > 0.1f || Mathf.Abs(inputManager.verticalInput) > 0.1f;

        AnimationManager.SetBool(gameObject.transform.GetChild(0).gameObject, "isWalking", isMoving);

        playerMovement.HandleSpeed(inputManager.IsHoldingSprintInput());
    }
}