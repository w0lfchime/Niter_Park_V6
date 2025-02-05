using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugVector : MonoBehaviour
{
    public static float vectorMagRenderCutoff = 0.05f;
    private static float inactiveTimeThreshold = 0.1f;

    [SerializeField] private GameObject vectorLine;
    [SerializeField] private GameObject vectorArrow;
    private Renderer lineRenderer;
    private Renderer arrowRenderer;
    private float thickness;
    private float lastUpdateTime;
    private bool isActive;

    private void Awake()
    {
        if (vectorLine) lineRenderer = vectorLine.GetComponent<Renderer>();
        if (vectorArrow) arrowRenderer = vectorArrow.GetComponent<Renderer>();
    }

    //private void Update()
    //{
    //    if (isActive && Time.time - lastUpdateTime > inactiveTimeThreshold)
    //    {
    //        SetActiveState(false);
    //    }
    //}

    public void Initialize(float thickness)
    {
        this.thickness = thickness;
        SetActiveState(false);
    }

    public void UpdateVector(Vector3 startPos, Vector3 vector, Color color)
    {
        isActive = false;


        lastUpdateTime = Time.time;

        if (vector.magnitude > vectorMagRenderCutoff)
        {
            SetActiveState(true);
            isActive = true;
            Vector3 endPos = startPos + vector;
            Vector3 midPoint = (startPos + endPos) / 2f;

            vectorLine.transform.position = midPoint;
            vectorLine.transform.rotation = Quaternion.LookRotation(vector.sqrMagnitude > 0 ? vector.normalized : Vector3.forward);
            vectorLine.transform.localScale = new Vector3(thickness, thickness, vector.magnitude);

            if (lineRenderer) lineRenderer.material.color = color;

            if (vectorArrow)
            {
                vectorArrow.transform.position = endPos;
                vectorArrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, vector);
                if (arrowRenderer) arrowRenderer.material.color = color;
            }
        }

        SetActiveState(isActive);
    }

    private void SetActiveState(bool active)
    {
        if (vectorLine) vectorLine.SetActive(active);
        if (vectorArrow) vectorArrow.SetActive(active);
    }
}
