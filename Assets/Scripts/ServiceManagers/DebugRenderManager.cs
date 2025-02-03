using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRenderManager : MonoBehaviour
{
    private Dictionary<string, LineRenderer> persistentVectors = new Dictionary<string, LineRenderer>();

    [SerializeField] private Material lineMaterial; // Assign a material with an unlit shader for visibility

    private void Awake()
    {
        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Unlit/Color"));
        }
    }

    /// <summary>
    /// Creates a new persistent vector. If a vector with the same name exists, it is replaced.
    /// </summary>
    public void CreateVector(string name, Color color)
    {
        if (persistentVectors.TryGetValue(name, out LineRenderer existingLine))
        {
            existingLine.material.color = color;
        } else
        {
            GameObject vectorObj = new GameObject($"ForceVector_{name}");
            vectorObj.transform.parent = transform; // Keep things organized

            LineRenderer lineRenderer = vectorObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(lineMaterial);
            lineRenderer.material.color = color;

            persistentVectors[name] = lineRenderer;
  
        }
    }

    /// <summary>
    /// Updates an existing persistent vector.
    /// </summary>
    public void UpdateVector(string name, Vector3 startPos, Vector3 vector, Color color)
    {
        if (persistentVectors.TryGetValue(name, out LineRenderer lineRenderer))
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, startPos + vector);
        } else
        {
            CreateVector(name, color);
            UpdateVector(name, startPos, vector, color);
        }
    }

    /// <summary>
    /// Destroys a persistent vector by name.
    /// </summary>
    public void DestroyVector(string name)
    {
        if (persistentVectors.TryGetValue(name, out LineRenderer lineRenderer))
        {
            Destroy(lineRenderer.gameObject);
            persistentVectors.Remove(name);
        }
    }

    /// <summary>
    /// Creates a temporary vector that lasts for a set duration before being removed.
    /// </summary>
    public void StampVector(string name, Vector3 startPos, Vector3 vector, Color color, float duration)
    {
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

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos + vector);

        yield return new WaitForSeconds(duration);

        Destroy(vectorObj);
    }
}

