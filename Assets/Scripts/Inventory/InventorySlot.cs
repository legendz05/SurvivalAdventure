using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Color startColor;
    Image image;

    ItemData itemInSlot;
    int amountInSlot;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        startColor = image.color;
    }

    void Update()
    {
        if (InventoryManager.Instance.highlightedSlot == this)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }

        if (InventoryManager.Instance.selectedSlot != this)
        {
            image.color = startColor;
        }
    }

    public void OnSelect()
    {
        image.color = Color.green;
        InventoryManager.Instance.selectedSlot = this;

        if (itemInSlot != null && amountInSlot >= itemInSlot.maxStack)
        {
            // do nothing
        }
        else
        {
            Place();
        }
    }

    public void Pickup()
    {

    }

    public void Place()
    {

    }
}
