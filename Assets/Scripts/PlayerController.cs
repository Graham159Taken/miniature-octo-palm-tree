using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public Transform cameraPivot;
    public float mouseSensitivity = 175f;

    [Header("Camera Offsets")]
    public float pivotOffsetX = 0.6f;
    public float pivotOffsetY = 1.5f;
    public float cameraDistance = 5f;
    public float cameraCollisionOffset = 0.2f;

    [Header("Camera Rotation")]
    public float minCameraAngle = -25f;
    public float maxCameraAngle = 70f;

    [Header("Movement Settings")]
    public float movementSpeed = 5f;
    public float gravity = -9.81f;

    private Vector3 velocity;
    private float verticalRotation = 0f;
    private bool isRightSide = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        UpdateCameraPivot();
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        ApplyGravity();
        UpdateCameraPosition();

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            SwitchCameraSide();
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = (cameraTransform.forward * vertical + cameraTransform.right * horizontal).normalized;
        moveDirection.y = 0f;

        if (moveDirection.magnitude >= 0.1f)
        {
            controller.Move(moveDirection * movementSpeed * Time.deltaTime);
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minCameraAngle, maxCameraAngle);
        cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -0.1f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public LayerMask cameraCollisionLayers;

    private void UpdateCameraPosition()
    {
        Vector3 desiredPosition = cameraPivot.position - cameraPivot.forward * cameraDistance;
        Vector3 direction = (desiredPosition - cameraPivot.position).normalized;
        float adjustedDistance = cameraDistance;

        if (Physics.Raycast(cameraPivot.position, direction, out RaycastHit hit, cameraDistance, cameraCollisionLayers))
        {
            adjustedDistance = Mathf.Clamp(hit.distance - cameraCollisionOffset, 0.5f, cameraDistance);
        }

        cameraTransform.position = cameraPivot.position - cameraPivot.forward * adjustedDistance;
        cameraTransform.LookAt(cameraPivot.position);
    }

    private void SwitchCameraSide()
    {
        isRightSide = !isRightSide;
        UpdateCameraPivot();
    }

    private void UpdateCameraPivot()
    {
        float newPivotX = isRightSide ? Mathf.Abs(pivotOffsetX) : -Mathf.Abs(pivotOffsetX);
        cameraPivot.localPosition = new Vector3(newPivotX, pivotOffsetY, 0f);
    }
}