using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public void LightAttack()
    {
        AnimationManager.ChangeLayerWeight(gameObject.transform.GetChild(0).gameObject, 1, 1);
        AnimationManager.PlayAnimation(gameObject.transform.GetChild(0).gameObject, "LightAttack", 1);
    }
}
