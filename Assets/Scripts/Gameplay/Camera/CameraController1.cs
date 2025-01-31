using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public Transform target; // The target GameObject to follow
    public float smoothSpeed = 5f; // Speed of smoothing
    private Vector3 offset; // Initial relative position

    void Start()
    {
        if (target == null)
        {
            LogCore.Log("Camera Controller1: No target assigned");
            enabled = false;
            return;
        }

        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
