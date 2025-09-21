using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class InputHandler : MonoBehaviour
{
    private Camera _mainCamera;

    [SerializeField] private SpriteMask spriteMask;
    [SerializeField] private Light2D light2DScript;
    private float targetScale;

    [SerializeField] private float scrollSensitivity = 0.001f;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 2f;
    [SerializeField] private float lerpSpeed = 10f;

    // For 2D light
    [SerializeField] private float lightInnerRadius = 0.54f;
    [SerializeField] private float lightOuterRadius = 1.93f;
    private float lightOuterRadiusTarget;

    // Variable to keep track of current Scroll Progress as a percentage
    // This is so I can use Lerp with this as the interpolation percentage
    // for both SpriteMask Scale and the Light Radius
    private float scrollProgress = 1.0f;
    private void Awake()
    {
        _mainCamera = Camera.main;

        if (spriteMask != null)
        {
            targetScale = spriteMask.transform.localScale.x;
        }

        if (light2DScript != null)
        {
            lightOuterRadiusTarget = light2DScript.pointLightOuterRadius;
        }
    }

    private void Update()
    {
        if (spriteMask != null)
        {
            float current = spriteMask.transform.localScale.x;
            float newScale = Mathf.Lerp(current, targetScale, Time.deltaTime * lerpSpeed);

            spriteMask.transform.localScale = new Vector3(newScale, newScale, 1f);
        }
        // Light Radius Adjustment
        if (light2DScript != null)
        {
            float currentOuterRadius = light2DScript.pointLightOuterRadius;
            float newOuterRadius = Mathf.Lerp(currentOuterRadius, lightOuterRadiusTarget, Time.deltaTime * lerpSpeed);

            light2DScript.pointLightOuterRadius = newOuterRadius;
        }
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        float scrollY = context.ReadValue<Vector2>().y;

        if (Mathf.Abs(scrollY) > 0.01f)
        {
            // accumulate target size instead of snapping
            //targetScale += scrollY * scrollSensitivity;

            // update scrolProgress percentage so I can determine Target Sizes between maxSize and minSize
            scrollProgress = Mathf.Clamp(scrollProgress + (scrollY * scrollSensitivity), 0.0f, 1.0f);
            targetScale = Mathf.Lerp(minSize, maxSize, scrollProgress);

            // Don't need below because targetScale is guaranteed to be between minSize and maxSize
                        // clamp so it doesn't grow/shrink forever
                        //targetScale = Mathf.Clamp(targetScale, minSize, maxSize);

            lightOuterRadiusTarget = Mathf.Lerp(lightInnerRadius, lightOuterRadius, scrollProgress);
        }
    }
}
