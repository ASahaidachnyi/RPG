using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float distance = 5f; // Distance from player
    public float height = 2f; // Height above player
    public float mouseSensitivity = 5f; // Mouse rotation speed
    public float maxPitch = 80f; // Max upward tilt
    public float minPitch = -20f; // Max downward tilt
    private float currentYaw = 0f; // Horizontal rotation
    private float currentPitch = 10f; // Vertical rotation

    void Start()
    {
        // Lock cursor for mouse control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Get mouse input
        currentYaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        currentPitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        // Calculate camera position
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 offset = new Vector3(0, height, -distance);
        Vector3 desiredPosition = player.position + rotation * offset;

        // Update camera position and rotation
        transform.position = desiredPosition;
        transform.LookAt(player.position + Vector3.up * height);
    }
}