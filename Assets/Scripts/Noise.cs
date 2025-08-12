using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//for game goobers
public class Noise : MonoBehaviour
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, int seed)
    {

        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-100000, 100000) + offset.x;
        float offsetY = prng.Next(-100000, 100000) - offset.y;

        float[,] noiseMap = new float[mapWidth, mapHeight];

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleY = (y - halfHeight + offsetY) / scale * frequency;
                    float sampleX = (x - halfWidth + offsetX) / scale * frequency;

                    float perlinVal = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinVal * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}
