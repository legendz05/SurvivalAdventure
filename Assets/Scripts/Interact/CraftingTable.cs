using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingTable : Interactable
{
    public override void Interact()
    {
        base.Interact();

        GetComponent<MeshRenderer>().material.color = Color.red;

        inputManager.DisableMovement();
        inputManager.ToggleCursorVisibility(true);
    }

    public override void CloseInteraction()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;

        inputManager.EnableMovement();
    }
}
