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
		float assingedSpeed = baseVehicleSpeed;
		switch (laneIndex)
		{
			case 0:
				assingedSpeed *= 1.2f;
				break;
			case 1:
				assingedSpeed *= 1.4f;
				break;
			case 2:
				assingedSpeed *= 1.3f;
				break;
			default: 
				break;
		}
		vehicleMovement.SetUp(splines, laneIndex, assingedSpeed, laneIndex < 2); // Pass lane index to movement
	}
}
