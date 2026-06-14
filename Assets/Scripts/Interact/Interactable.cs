using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isCurrentlyInteractingWith = false;
    protected InputManager inputManager;

    public virtual void Awake()
    {
        inputManager = FindAnyObjectByType<InputManager>();
    }

    public virtual void Interact()
    {
        isCurrentlyInteractingWith = true;
    }

    public virtual void CloseInteraction()
    {
        isCurrentlyInteractingWith = false;
        inputManager.ToggleCursorVisibility(false);
    }
}
