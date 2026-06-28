using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private InputManager inputManager;
    private Transform mainCamera;
    private Rigidbody rb;

    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 10f;
    float sprintSpeed;
    float oldSpeed;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();

        if (Camera.main != null)
            mainCamera = Camera.main.transform;

        sprintSpeed = movementSpeed * 2f;
        oldSpeed = movementSpeed;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    public void HandleJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void HandleSpeed(bool isSprinting)
    {
        //Debug.Log(isSprinting);

        if (isSprinting)
        {
            movementSpeed = sprintSpeed;
            AnimationManager.SetBool(gameObject.transform.GetChild(0).gameObject, "isRunning", isSprinting);
        }
        else
        {
            movementSpeed = oldSpeed;
            AnimationManager.SetBool(gameObject.transform.GetChild(0).gameObject, "isRunning", isSprinting);
        }

    }

    private Vector3 GetCameraRelativeMoveDirection()
    {
        if (mainCamera == null)
            return Vector3.zero;

        Vector3 cameraForward = mainCamera.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = mainCamera.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * inputManager.verticalInput +
            cameraRight * inputManager.horizontalInput;

        moveDirection.Normalize();

        return moveDirection;
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = GetCameraRelativeMoveDirection();

        Vector3 targetPosition =
            rb.position + moveDirection * movementSpeed * Time.fixedDeltaTime;

        rb.MovePosition(targetPosition);
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = GetCameraRelativeMoveDirection();

        if (targetDirection == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        Quaternion smoothedRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(smoothedRotation);
    }
}