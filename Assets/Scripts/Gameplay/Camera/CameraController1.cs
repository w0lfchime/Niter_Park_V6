using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public Transform target; // The target GameObject to follow
    public float smoothSpeed = 5f; // Speed of smoothing
    public float zoomSpeed = 10f; // Speed of zooming
    public float minZoom = 2f; // Minimum zoom distance
    public float maxZoom = 15f; // Maximum zoom distance

    private Vector3 offset; // Initial relative position

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera Controller1: No target assigned");
            enabled = false;
            return;
        }

        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Handle zooming
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        offset = offset.normalized * Mathf.Clamp(offset.magnitude - scrollInput * zoomSpeed, minZoom, maxZoom);

        // Smoothly follow the target
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
