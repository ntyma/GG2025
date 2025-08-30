using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private float maxSpeed = 5f;

    [Header("Texture Settings")]
    public int texWidth = 512;
    public int texHeight = 512;
    public float maskThreshold = 2.0f;

    private Texture2D mask;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main; // cache this value
        GenerateTexture();
        ApplyTexture();
    }

    // Update is called once per frame
    void Update()
    {
        FollowMousePositionDelayed(maxSpeed);
    }

    private void FollowMousePosition()
    {
        transform.position = GetWorldPositionFromMouse();
    }

    private void FollowMousePositionDelayed(float maxSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, GetWorldPositionFromMouse(), maxSpeed * Time.deltaTime);
    }

    private Vector2 GetWorldPositionFromMouse()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void GenerateTexture()
    {
        mask = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        Vector2 maskCenter = new Vector2(texWidth * 0.5f, texHeight * 0.5f);

        for (int y = 0; y < texHeight; ++y)
        {
            for (int x = 0; x < texWidth; ++x)
            {
                float distFromCenter = Vector2.Distance(maskCenter, new Vector2(x, y));
                float maskPixel = (0.5f - (distFromCenter / texWidth)) * maskThreshold;
                maskPixel = Mathf.Clamp01(maskPixel);

                Color c = new Color(maskPixel, maskPixel, maskPixel, 1.0f);
                mask.SetPixel(x, y, c);
            }
        }
        mask.Apply();
    }

    private void ApplyTexture()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            r.material.mainTexture = mask;
        }
    }
}
