using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public void SetAnimationLayerWeight(int layerIDX, float weight)
    {
        AnimationManager.ChangeLayerWeight(gameObject, layerIDX, weight);
    }
}
