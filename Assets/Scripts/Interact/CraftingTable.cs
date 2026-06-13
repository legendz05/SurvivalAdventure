using UnityEngine;

public class CraftingTable : Interactable
{
    public override void Interact()
    {
        base.Interact();

        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
