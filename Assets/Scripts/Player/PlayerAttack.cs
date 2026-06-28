using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public void LightAttack()
    {
        AnimationManager.PlayAnimation(gameObject.transform.GetChild(0).gameObject, "LightAttack");
    }
}
