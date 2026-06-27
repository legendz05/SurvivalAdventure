using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;

    public Sprite itemIcon { get; private set; }
    public int maxStack { get; private set; }

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public bool isPickedUp = false;
    public int stackAmount = 1;

    private float pickupAllowedTime;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();

        if (itemData != null)
            Initialize(itemData, stackAmount);
    }

    public void Initialize(ItemData data, int amount = 1, float pickupDelayTime = 0f)
    {
        itemData = data;

        if (itemData == null)
            return;

        meshFilter.mesh = itemData.itemMesh;
        meshRenderer.material = itemData.itemMaterial;

        meshCollider.sharedMesh = itemData.itemMesh;
        meshCollider.convex = true;
        meshCollider.enabled = true;

        itemIcon = itemData.itemIcon;
        maxStack = itemData.maxStack;

        stackAmount = Mathf.Max(1, amount);
        isPickedUp = false;

        pickupAllowedTime = Time.time + pickupDelayTime;
    }

    private void Update()
    {
        if (!isPickedUp)
        {
            transform.Rotate(Vector3.up, 10f * Time.deltaTime);
        }
    }

    public void Interact()
    {
        if (isPickedUp) return;

        if (Time.time < pickupAllowedTime)
            return;

        bool added = InventoryManager.Instance.AddItem(itemData, null, stackAmount);

        if (added)
        {
            isPickedUp = true;
            Destroy(gameObject);
        }
    }
}