using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera _mainCamera;

    [SerializeField] private SpriteMask spriteMask;

    private float targetScale;

    [SerializeField] private float scrollSensitivity = 0.001f;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 2f;
    [SerializeField] private float lerpSpeed = 10f;

    private void Awake()
    {
        _mainCamera = Camera.main;

        if (spriteMask != null)
        {
            targetScale = spriteMask.transform.localScale.x;
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
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        float scrollY = context.ReadValue<Vector2>().y;

        if (Mathf.Abs(scrollY) > 0.01f)
        {
            // accumulate target size instead of snapping
            targetScale += scrollY * scrollSensitivity;

            // clamp so it doesn't grow/shrink forever
            targetScale = Mathf.Clamp(targetScale, minSize, maxSize);
        }
    }
}
