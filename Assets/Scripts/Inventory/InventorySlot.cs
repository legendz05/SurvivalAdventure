using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    Color startColor;
    Color iconColor;
    Image image;

    public ItemData itemInSlot;
    public int amountInSlot;
    public TextMeshProUGUI amountText;

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

    public void UpdateSlot(ItemData item, int amount = 0)
    {
        itemInSlot = item;
        amountInSlot = amount;

        if (item != null && amount > 0)
        {
            slotIcon.sprite = item.itemIcon;

            Color color = slotIcon.color;
            color.a = 1f;
            slotIcon.color = color;
        }
        else
        {
            itemInSlot = null;
            amountInSlot = 0;

            slotIcon.sprite = null;

            Color color = slotIcon.color;
            color.a = 0f;
            slotIcon.color = color;
        }

        amountText.text = amountInSlot > 1 ? amountInSlot.ToString() : "";
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
