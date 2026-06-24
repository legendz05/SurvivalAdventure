using Unity.VisualScripting;
using UnityEngine;

public class Item : Interactable
{
    public ItemData itemData;

    public Sprite itemIcon { get; private set; }
    public int maxStack { get; private set; }

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    public bool isPickedUp = false;

    public override void Awake()
    {
        GenerateItem();
    }

    private void GenerateItem()
    {
        if (itemData == null)
            return;

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        meshFilter.mesh = itemData.itemMesh;
        meshRenderer.material = itemData.itemMaterial;
        itemIcon = itemData.itemIcon;
        maxStack = itemData.maxStack;

        meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    private void Update()
    {
        if (!isPickedUp)
        {
            //meshCollider.enabled = false;
            transform.Rotate(Vector3.up, 10f * Time.deltaTime);
        }
        else
        {
            meshCollider.enabled = true;
        }
    }

    public override void Interact()
    {
        if (!isPickedUp)
        {
            isPickedUp = true;

            if (action == null)
                action = FindAnyObjectByType<PlayerAction>();

            action.interactables.Remove(this);
            action.FinishInteraction();

            InventoryManager.Instance.AddItem(this);

            gameObject.SetActive(false);
        }
    }
}
