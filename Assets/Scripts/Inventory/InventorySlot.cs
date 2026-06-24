using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Color startColor;
    Color iconColor;
    Image image;

    public Item itemInSlot;
    public int amountInSlot;

    public bool isHotbarSlot;

    public Image slotIcon;

    private Coroutine selectRoutine;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        startColor = image.color;
        iconColor = slotIcon.color;

        UpdateSlot(null);
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

    public void UpdateSlot(Item item)
    {
        itemInSlot = item;

        if (item != null)
        {
            slotIcon.sprite = item.itemIcon;

            Color color = slotIcon.color;
            color.a = 1f;
            slotIcon.color = color;
        }
        else
        {
            slotIcon.sprite = null;

            Color color = slotIcon.color;
            color.a = 0f;
            slotIcon.color = color;
        }
    }

    public void OnSelect()
    {
        InventoryManager.Instance.selectedSlot = this;

        if (selectRoutine != null)
            selectRoutine = null;

        selectRoutine = StartCoroutine(OnSelectRoutine());
    }

    private IEnumerator OnSelectRoutine()
    {
        image.color = Color.green;

        yield return new WaitForSecondsRealtime(0.125f);

        image.color = startColor;
    }
}
