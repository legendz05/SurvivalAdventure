using UnityEngine;

public class Chest : Interactable
{
    public override void Interact()
    {
        base.Interact();

        gameObject.transform.localScale = Vector3.one * 5;

        inputManager.DisableMovement();
        inputManager.ToggleCursorVisibility(true);
    }

    public override void CloseInteraction()
    {
        base.CloseInteraction();

        gameObject.transform.localScale = Vector3.one;

        inputManager.EnableMovement();
    }
}
