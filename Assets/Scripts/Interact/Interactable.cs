using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isCurrentlyInteractingWith = false;

    public virtual void Interact()
    {
        isCurrentlyInteractingWith = true;
    }
}
