using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera _mainCamera;

    [SerializeField]
    private SpriteMask spriteMask;

    [SerializeField]
    private float scaleAmount = 0.2f;      // how much to expand
    [SerializeField]
    private float scaleDuration = 0.1f;    // how long to expand/shrink
    [SerializeField]
    private float holdTime = 0.1f;         // how long to stay expanded

    private Vector3 originalScale;
    private Coroutine pulseCoroutine;

    private void Awake()
    {
        _mainCamera = Camera.main;

        if (spriteMask != null)
        {
            originalScale = spriteMask.transform.localScale;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (spriteMask != null)
        {
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
            }
            pulseCoroutine = StartCoroutine(PulseSpriteMask());
        }
    }

    private IEnumerator PulseSpriteMask()
    {
        Vector3 targetScale = originalScale + Vector3.one * scaleAmount;

        // expand
        yield return ScaleOverTime(spriteMask.transform, originalScale, targetScale, scaleDuration);

        // hold at expanded size
        yield return new WaitForSeconds(holdTime);

        // shrink
        yield return ScaleOverTime(spriteMask.transform, targetScale, originalScale, scaleDuration);
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            target.localScale = Vector3.Lerp(from, to, t);
            time += Time.deltaTime;
            yield return null;
        }
        target.localScale = to;
    }
}
