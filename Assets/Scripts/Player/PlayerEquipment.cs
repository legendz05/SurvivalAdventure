using Unity.VisualScripting;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    private Animator animator;
    private Transform rightHand;

    private Item currentRightHandItem;
    private ItemData currentItemData;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("No Animator found in PlayerEquipment.");
            return;
        }

        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

        if (rightHand == null)
        {
            Debug.LogError("Right hand bone not found. Check Humanoid avatar rig setup.");
        }
    }

    public void EquipToRightHand(ItemData itemData)
    {
        if (itemData == null)
        {
            UnequipRightHand();
            return;
        }

        if (rightHand == null) return;

        if (currentRightHandItem != null && currentItemData == itemData)
            return;

        UnequipRightHand();

        Item equippedItem = Instantiate(
            InventoryManager.Instance.worldItemPrefab,
            rightHand,
            false
        );

        equippedItem.InitializeEquippedItem(itemData);

        equippedItem.transform.localPosition = new Vector3(-1, 0, 0);
        equippedItem.transform.localRotation = Quaternion.identity;
        equippedItem.transform.localScale = Vector3.one;

        currentRightHandItem = equippedItem;
        currentItemData = itemData;
    }

    public void UnequipRightHand()
    {
        if (currentRightHandItem != null)
        {
            Destroy(currentRightHandItem.gameObject);
            currentRightHandItem = null;
        }

        currentItemData = null;
    }
}