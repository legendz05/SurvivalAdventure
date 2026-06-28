using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private static bool VerifyAnimatorComponents(GameObject gameObject)
    {
        if (gameObject == null) return false;
        if (!gameObject.GetComponent<Animator>()) return false;

        return true;
    }

    public static void ActivateTrigger(GameObject gameObject, string triggerName)
    {
        if (!VerifyAnimatorComponents(gameObject))
            return;

        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetTrigger(triggerName);
    }

    public static void SetBool(GameObject gameObject, string boolName, bool value)
    {
        if (!VerifyAnimatorComponents(gameObject))
            return;

        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool(boolName, value);
    }

    public static void SetFloat(GameObject gameObject, string floatName, float value)
    {
        if (!VerifyAnimatorComponents(gameObject))
            return;

        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetFloat(floatName, value);
    }
}