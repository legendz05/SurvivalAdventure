using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;

    public Sprite itemIcon { get; private set; }
    public int maxStack { get; private set; }

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private BoxCollider boxCollider;
    private Rigidbody rb;

    public bool isPickedUp = false;
    public int stackAmount = 1;

    private float pickupAllowedTime;

    private float heightSpeed = 2f;
    private float height = 0.25f;

    private bool isFloating = false;
    private float floatStartTime;
    private Vector3 floatCenterPos;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider>();

        if (itemData != null)
            Initialize(itemData, stackAmount);

        pickupAllowedTime = Time.time + 3f;
    }

    public void Initialize(ItemData data, int amount = 1, float pickupDelayTime = 0f)
    {
        itemData = data;

        if (itemData == null)
            return;

        meshFilter.mesh = itemData.itemMesh;
        meshRenderer.material = itemData.itemMaterial;

        boxCollider.enabled = true;

        itemIcon = itemData.itemIcon;
        maxStack = itemData.maxStack;

        stackAmount = Mathf.Max(1, amount);
        isPickedUp = false;

        pickupAllowedTime = Time.time + pickupDelayTime;
    }

    public void InitializeEquippedItem(ItemData data)
    {
        itemData = data;

        if (itemData == null)
            return;

        meshFilter.mesh = itemData.itemMesh;
        meshRenderer.material = itemData.itemMaterial;

        boxCollider.enabled = true;
        boxCollider.isTrigger = true;

        itemIcon = itemData.itemIcon;
        maxStack = itemData.maxStack;
    }

    private void Update()
    {
        if (isPickedUp)
            return;

        if (!isFloating)
            return;

        transform.Rotate(Vector3.up, 10f * Time.deltaTime);

        float elapsed = Time.time - floatStartTime;
        float newY = floatCenterPos.y + Mathf.Sin(elapsed * heightSpeed) * height;

        transform.position = new Vector3(floatCenterPos.x, newY, floatCenterPos.z);
    }

    private void BeginFloating()
    {
        isFloating = true;

        floatStartTime = Time.time;
        floatCenterPos = transform.position;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            BeginFloating();
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