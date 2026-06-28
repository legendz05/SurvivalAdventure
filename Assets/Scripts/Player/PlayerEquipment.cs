using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    private Animator animator;

    private Transform rightHand;
    private Item currentRightHandItem;

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
        if (itemData == null) return;
        if (rightHand == null) return;

        // Remove previous equipped item
        if (currentRightHandItem != null)
            Destroy(currentRightHandItem.gameObject);

        Vector3 spawnPosition = rightHand.transform.position;

        Item equippedItem = Instantiate(
            InventoryManager.Instance.worldItemPrefab,
            spawnPosition,
            Quaternion.identity
        );

        equippedItem.InitializeEquippedItem(itemData);

        currentRightHandItem = equippedItem;
    }

    public void UnequipRightHand()
    {
        if (currentRightHandItem != null)
        {
            Destroy(currentRightHandItem.gameObject);
            currentRightHandItem = null;
        }
    }
}