using UnityEngine;
using System.Collections;
using UnityEngine.Splines;

public class TrafficManager : MonoBehaviour
{
	public GameObject[] vehiclePrefabs;
	public float spawnInterval = 5f;
	public float baseVehicleSpeed = 10f;
	public SplineContainer splines; // Single container with multiple splines
	private Transform vehicleParent;

	void Start()
	{
		FindSplines();
		FindOrCreateVehicleParent();
		StartCoroutine(SpawnTraffic());
	}

	void FindSplines()
	{
		Transform splinesParent = transform.Find("Splines"); // Find child named "Splines"
		if (splinesParent != null)
		{
			splines = splinesParent.GetComponent<SplineContainer>(); // Get the single SplineContainer
		}
		else
		{
			Debug.LogError("TrafficManager: No 'Splines' child object found!");
		}
	}

	void FindOrCreateVehicleParent()
	{
		vehicleParent = transform.Find("Vehicles"); // Try to find an existing "Vehicles" object
		if (vehicleParent == null)
		{
			vehicleParent = new GameObject("Vehicles").transform; // Create one if missing
			vehicleParent.SetParent(transform); // Set it as a child of TrafficManager
		}
	}

	IEnumerator SpawnTraffic()
	{
		while (true)
		{
			yield return new WaitForSeconds(spawnInterval);
			if (splines != null && splines.Splines.Count > 0)
			{
				SpawnVehicle();
			}
		}
	}

	void SpawnVehicle()
	{
		int laneIndex = Random.Range(0, splines.Splines.Count); // Choose a spline from the container

		GameObject vehiclePrefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
		GameObject newVehicle = Instantiate(vehiclePrefab, vehicleParent); // Set parent to "Vehicles"

		VehicleMovement vehicleMovement = newVehicle.AddComponent<VehicleMovement>();
		float assignedSpeed = baseVehicleSpeed;

		switch (laneIndex)
		{
			case 0:
				assignedSpeed *= 1.2f;
				break;
			case 1:
				assignedSpeed *= 1.4f;
				break;
			case 2:
				assignedSpeed *= 1.3f;
				break;
			default:
				break;
		}

		// Ensure the vehicle starts in the correct position immediately
		Spline spline = splines.Splines[laneIndex];
		float startT = 0f; // Start at the beginning of the spline
		Vector3 startPosition = spline.EvaluatePosition(startT);
		Quaternion startRotation = Quaternion.LookRotation(spline.EvaluateTangent(startT));

		newVehicle.transform.position = startPosition;
		newVehicle.transform.rotation = startRotation;

		vehicleMovement.SetUp(splines, laneIndex, assignedSpeed, laneIndex < 2);
	}

}
