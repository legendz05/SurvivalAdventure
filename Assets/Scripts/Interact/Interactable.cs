using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isCurrentlyInteractingWith = false;
    protected InputManager inputManager;
    protected PlayerAction action;

    public virtual void Awake()
    {
        inputManager = FindAnyObjectByType<InputManager>();
        action = FindAnyObjectByType<PlayerAction>();
    }

    public virtual void Interact()
    {
        isCurrentlyInteractingWith = true;
    }

    public virtual void CloseInteraction()
    {
        if (!isCurrentlyInteractingWith)
        { return; }

        isCurrentlyInteractingWith = false;
        inputManager.ToggleCursorVisibility(false);
    }
}
