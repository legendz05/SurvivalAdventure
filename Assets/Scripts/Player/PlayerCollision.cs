using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerAction action;

    public bool isGrounded = false;

    private void Awake()
    {
        action = GetComponent<PlayerAction>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Interactable interactable))
        {
            action.interactables.Add(interactable);

            interactable.TryGetComponent(out MeshRenderer renderer);
            renderer.material.color = Color.green;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Item item))
        {
            item.Interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Interactable interactable))
        {
            action.interactables.Remove(interactable);

            interactable.TryGetComponent(out MeshRenderer renderer);
            renderer.material.color = Color.white;
        }
    }
}
