using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform player1;
	public Transform player2;

	public float smoothSpeedX = 5f; // Speed of smoothing in X
	public float smoothSpeedY = 5f; // Speed of smoothing in Y
	public float smoothSpeedZ = 5f; // Speed of smoothing in Z

	public float zoomSpeed = 10f; // Speed of zooming
	public float minZoom = 2f; // Minimum zoom distance
	public float maxZoom = 15f; // Maximum zoom distance
	public float zoomMultiplier = 1.5f; // Adjusts zoom based on distance between players

	public Transform leftBound;  // Left boundary (transform position.x)
	public Transform rightBound; // Right boundary (transform position.x)

	private Vector3 initialOffset; // Initial relative position from the midpoint

	void Start()
	{
		if (player1 == null || player2 == null)
		{
			Debug.LogError("Camera Controller: Players not assigned!");
			enabled = false;
			return;
		}

		// Set initial offset based on the midpoint of the two players
		initialOffset = transform.position - GetMidpoint();
	}

	void LateUpdate()
	{
		if (player1 == null || player2 == null) return;

		Vector3 midpoint = GetMidpoint();

		// Adjust zoom based on player distance
		float playerDistance = Vector3.Distance(player1.position, player2.position);
		float zoom = Mathf.Clamp(playerDistance * zoomMultiplier, minZoom, maxZoom);

		// Calculate desired position
		Vector3 desiredPosition = midpoint + initialOffset.normalized * zoom;

		// Apply boundaries to X position
		if (leftBound != null && rightBound != null)
		{
			float minX = leftBound.position.x;
			float maxX = rightBound.position.x;
			desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
		}

		// Smooth movement for each axis separately
		Vector3 smoothedPosition = new Vector3(
			Mathf.Lerp(transform.position.x, desiredPosition.x, smoothSpeedX * Time.deltaTime),
			Mathf.Lerp(transform.position.y, desiredPosition.y, smoothSpeedY * Time.deltaTime),
			Mathf.Lerp(transform.position.z, desiredPosition.z, smoothSpeedZ * Time.deltaTime)
		);

		transform.position = smoothedPosition;
	}

	private Vector3 GetMidpoint()
	{
		return (player1.position + player2.position) / 2f;
	}
}
