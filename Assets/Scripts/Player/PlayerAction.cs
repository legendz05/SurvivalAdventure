using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public List<Interactable> interactables;
    public bool canInteract = false;
    public bool isInteracting = false;

    public void Interact()
    {
        if (!canInteract || isInteracting) return;

        interactables[0].Interact();
        Debug.Log($"Interacted with: {interactables[0].name}");

        isInteracting = true;
    }

    public void CloseInteraction()
    {
        if (!isInteracting) return;

        interactables[0].CloseInteraction();
        isInteracting = false;
    }

    private void Update()
    {
        if (interactables.Count > 0)
            canInteract = true;
        else
            canInteract = false;
    }
}
