using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SyncRadiusWColliderRadius : MonoBehaviour
{
    [Tooltip("Inner radius relative to outer radius (e.g. 0.5 = half of outer).")]
    public float innerRadiusToOuterRatio = 0.5f;

    private CircleCollider2D circleCollider;
    private Light2D light2D;

    private float lastRadius;

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        light2D = GetComponentInChildren<Light2D>();

        if (light2D != null)
        {
            float initialRadius = circleCollider.radius * transform.lossyScale.x;
            lastRadius = initialRadius;

            light2D.pointLightOuterRadius = initialRadius;
            light2D.pointLightInnerRadius = innerRadiusToOuterRatio * initialRadius;
        }
        else
        {
            Debug.LogWarning("No Light2D found in children of " + gameObject.name);
        }
    }

    void Update()
    {
        if (light2D == null) return;

        // Adjust for scaling to correct units (world units)
        float currentRadius = circleCollider.radius * transform.lossyScale.x;

        // Only update if radius has changed
        if (!Mathf.Approximately(currentRadius, lastRadius))
        {
            light2D.pointLightOuterRadius = currentRadius;
            light2D.pointLightInnerRadius = innerRadiusToOuterRatio * currentRadius;
            lastRadius = currentRadius;
        }
    }
}
