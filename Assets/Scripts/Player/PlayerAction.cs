using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public List<Interactable> interactables;
    public bool canInteract = false;
    public bool isInteracting = false;

    private Interactable interactedWith;

    public void Interact()
    {
        if (!canInteract || isInteracting) return;
        if (interactables.Count <= 0) return;

        interactedWith = interactables[0];
        isInteracting = true;

        Debug.Log($"Interacted with: {interactedWith.name}");

        interactedWith.Interact();
    }

    public void CloseInteraction()
    {
        if (!isInteracting) return;

        interactedWith.CloseInteraction();
        isInteracting = false;

        interactedWith = null;
    }

    public void FinishInteraction()
    {
        isInteracting = false;
        interactedWith = null;
    }

    private void Update()
    {
        if (interactables.Count > 0)
            canInteract = true;
        else
            canInteract = false;
    }
}
