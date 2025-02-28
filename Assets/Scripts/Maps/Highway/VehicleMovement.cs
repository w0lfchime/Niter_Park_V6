using UnityEngine;
using UnityEngine.Splines;

public class VehicleMovement : MonoBehaviour
{
	private SplineContainer splinePath;
	private int splineIndex; // Index of the specific spline to follow
	private float speed;
	private float progress = 0f;
	private bool reverse;

	public void SetUp(SplineContainer splineContainer, int laneIndex, float moveSpeed, bool moveReverse)
	{
		splinePath = splineContainer;
		splineIndex = laneIndex;
		speed = moveSpeed;
		reverse = moveReverse;
		progress = reverse ? 1f : 0f; // Start at the correct end
	}

	void Update()
	{
		if (splinePath == null || splinePath.Splines.Count <= splineIndex) return;

		// Move along the selected spline
		float direction = reverse ? -1f : 1f;
		progress += (speed / splinePath.Splines[splineIndex].GetLength()) * Time.deltaTime * direction;

		// Clamp progress and despawn at the end
		if ((reverse && progress <= 0f) || (!reverse && progress >= 1f))
		{
			Destroy(gameObject);
			return;
		}

		// Get position and forward direction from spline
		transform.position = splinePath.EvaluatePosition(splineIndex, progress);
		Quaternion forwardRotation = Quaternion.LookRotation(splinePath.EvaluateTangent(splineIndex, progress));

		// If reverse, flip the vehicle 180 degrees
		if (reverse)
		{
			forwardRotation *= Quaternion.Euler(0, 180, 0);
		}

		transform.rotation = forwardRotation;
	}
}
