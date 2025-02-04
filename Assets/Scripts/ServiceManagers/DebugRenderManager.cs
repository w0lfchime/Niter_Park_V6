using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRenderManager : MonoBehaviour
{
    private Dictionary<string, LineRenderer> persistentVectors = new Dictionary<string, LineRenderer>();
    private Dictionary<string, GameObject> vectorTips = new Dictionary<string, GameObject>(); // Stores the cone tips

    [SerializeField] private Material lineMaterial; // Assign a material with an unlit shader
    [SerializeField] private GameObject conePrefab; // Assign a small cone mesh prefab in the Inspector
    [SerializeField] private float vectorLengthFactor = 0.3f;

    private void Awake()
    {
        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Unlit/Color"));
        }
    }

    private void SetConeColor(GameObject coneTip, Color color)
    {
        if (coneTip)
        {
            Renderer renderer = coneTip.GetComponentInChildren<Renderer>();
            if (renderer)
            {
                renderer.material = new Material(renderer.material);
                renderer.material.color = color;
            }
        }
    }

    /// <summary>
    /// Creates a new persistent vector with a cone tip. If a vector with the same name exists, it is replaced.
    /// </summary>
    public void CreateVector(string name, Color color)
    {
        if (persistentVectors.ContainsKey(name))
        {
            persistentVectors[name].material.color = color;
        }
        else
        {
            GameObject vectorObj = new GameObject($"{name}_Vector");
            vectorObj.transform.parent = transform;

            LineRenderer lineRenderer = vectorObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(lineMaterial);
            lineRenderer.material.color = color;

            persistentVectors[name] = lineRenderer;

            // Create the cone tip
            if (conePrefab)
            {
                GameObject coneTip = Instantiate(conePrefab, Vector3.zero, Quaternion.identity, transform); // Fixed size
                vectorTips[name] = coneTip;
                SetConeColor(coneTip, color);
            }
        }
    }

    /// <summary>
    /// Updates an existing persistent vector and its cone tip.
    /// </summary>
    public void UpdateVector(string name, Vector3 startPos, Vector3 vector, Color color)
    {
        vector *= vectorLengthFactor;
        if (persistentVectors.TryGetValue(name, out LineRenderer lineRenderer))
        {
            Vector3 endPos = startPos + vector;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
            lineRenderer.material.color = color;

            // Update the cone tip position and rotation
            if (vectorTips.TryGetValue(name, out GameObject coneTip))
            {
                coneTip.transform.position = endPos;
                coneTip.transform.rotation = Quaternion.LookRotation(vector.normalized);
                SetConeColor(coneTip, color);
            }
        }
        else
        {
            CreateVector(name, color);
            UpdateVector(name, startPos, vector, color);
        }
    }

    /// <summary>
    /// Destroys a persistent vector and its associated cone tip.
    /// </summary>
    public void DestroyVector(string name)
    {
        if (persistentVectors.TryGetValue(name, out LineRenderer lineRenderer))
        {
            Destroy(lineRenderer.gameObject);
            persistentVectors.Remove(name);
        }

        if (vectorTips.TryGetValue(name, out GameObject coneTip))
        {
            Destroy(coneTip);
            vectorTips.Remove(name);
        }
    }

    /// <summary>
    /// Creates a temporary vector with a cone tip that lasts for a set duration before being removed.
    /// </summary>
    public void StampVector(string name, Vector3 startPos, Vector3 vector, Color color, float duration)
    {
        vector *= vectorLengthFactor;
        StartCoroutine(StampVectorCoroutine(name, startPos, vector, color, duration));
    }

    private IEnumerator StampVectorCoroutine(string name, Vector3 startPos, Vector3 vector, Color color, float duration)
    {
        GameObject vectorObj = new GameObject($"ImpulseForceVector_{name}");
        vectorObj.transform.parent = transform;

        LineRenderer lineRenderer = vectorObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(lineMaterial);
        lineRenderer.material.color = color;

        Vector3 endPos = startPos + vector;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        GameObject coneTip = null;
        if (conePrefab)
        {
            coneTip = Instantiate(conePrefab, endPos, Quaternion.LookRotation(vector.normalized), transform);
            SetConeColor(coneTip, color);
        }

        yield return new WaitForSeconds(duration);

        Destroy(vectorObj);
        if (coneTip) Destroy(coneTip);
    }
}
