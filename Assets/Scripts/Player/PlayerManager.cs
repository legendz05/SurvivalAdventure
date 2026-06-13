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
        inputManager.HandleMovementInput();

        if (inputManager.HasPressedInteract() && action.canInteract)
        {
            action.Interact();
            inputManager.HandleResetInteract();
        }
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