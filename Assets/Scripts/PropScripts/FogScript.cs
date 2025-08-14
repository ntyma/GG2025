using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class FogScript : MonoBehaviour
{
    [Range(0f, 1f)]
    public float fogOpacity = 1f;

    [Range(0f, 10f)]
    public int speed;

    public SpriteRenderer spriteRenderer;
    public int width;
    public int height;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public Vector2 offset;

    public int seed;

    public bool fogEnabled = false;

    public bool autoUpdate;

    public bool animate = true;

    public Transform playerPos;

    private float cumulativeOffset = 0;

    private void Update()
    {
        if (cumulativeOffset > 10000)
        {
            cumulativeOffset = 0;
        }
        transform.position = new Vector3(playerPos.position.x - 10, playerPos.position.y + 6, -1f);
        offset.x = transform.position.x + cumulativeOffset;
        cumulativeOffset+=speed;
        offset.y = transform.position.y;
        GenerateFog();
    }

    public void GenerateFog()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, persistance, lacunarity, offset, seed);
        Texture2D texture = TextureFromNoiseMap(noiseMap, fogOpacity);
        DrawTextureAsSprite(texture);
    }
    public void DrawTextureAsSprite(Texture2D texture)
    {
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0f, 1f));
    }

    public static Texture2D TextureFromNoiseMap(float[,] noiseMap, float opacity)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = new Color(
                    Mathf.Lerp(Color.grey.r, Color.white.r, noiseMap[x, y]),
                    Mathf.Lerp(Color.grey.g, Color.white.g, noiseMap[x, y]),
                    Mathf.Lerp(Color.grey.b, Color.white.b, noiseMap[x, y]),
                    opacity
                );
            }
        }

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
}
