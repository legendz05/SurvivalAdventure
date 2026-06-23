using System.Collections;
using UnityEngine;

public class Lever : Interactable
{
    public int interactionTime = 2;

    private Coroutine leverRoutine;

    public override void Interact()
    {
        base.Interact();

        gameObject.transform.localScale = Vector3.one * 5f;

        if (leverRoutine != null)
            StopCoroutine(leverRoutine);

        leverRoutine = StartCoroutine(OnActionOver(interactionTime));

        CloseInteraction();

    }

    public override void CloseInteraction()
    {
        base.CloseInteraction();
        action.FinishInteraction();
    }

    private IEnumerator OnActionOver(int delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.transform.localScale = Vector3.one;
        leverRoutine = null;
    }
}