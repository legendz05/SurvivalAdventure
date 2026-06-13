using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public List<Interactable> interactables;
    public bool canInteract = false;

    public void Interact()
    {
        Debug.Log($"Interacted with: {interactables[0].name}");
    }

    private void Update()
    {
        if (interactables.Count > 0)
            canInteract = true;
        else
            canInteract = false;
    }
}
