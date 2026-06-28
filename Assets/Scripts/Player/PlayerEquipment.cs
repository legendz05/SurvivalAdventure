using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    private Animator animator;

    private Transform rightHand;
    private GameObject currentRightHandItem;

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

    public void EquipToRightHand(GameObject itemPrefab)
    {
        if (itemPrefab == null) return;
        if (rightHand == null) return;

        // Remove previous equipped item
        if (currentRightHandItem != null)
            Destroy(currentRightHandItem);

        currentRightHandItem = Instantiate(itemPrefab, rightHand);

        currentRightHandItem.transform.localPosition = Vector3.zero;
        currentRightHandItem.transform.localRotation = Quaternion.identity;
        currentRightHandItem.transform.localScale = Vector3.one;
    }

    public void UnequipRightHand()
    {
        if (currentRightHandItem != null)
        {
            Destroy(currentRightHandItem);
            currentRightHandItem = null;
        }
    }
}