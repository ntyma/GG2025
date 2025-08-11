using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tilemap_PlayerMemory_Manager_Script : MonoBehaviour
{
    [SerializeField] private Tilemap PlayerMemory_Tilemap;
    [SerializeField] private BoundsInt PlayerMemory_Tilemap_Bounds;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMemory_Tilemap.CompressBounds();
        PlayerMemory_Tilemap_Bounds = PlayerMemory_Tilemap.cellBounds;
        Debug.Log("CURRENT BOUNDARIES ARE " + PlayerMemory_Tilemap_Bounds);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerMemory_Tilemap.CompressBounds();
            Debug.Log("CURRENT BOUNDARIES ARE " + PlayerMemory_Tilemap_Bounds);

            for (int i = PlayerMemory_Tilemap_Bounds.xMin; i <= PlayerMemory_Tilemap_Bounds.xMax; i ++)
            {
                for (int j = PlayerMemory_Tilemap_Bounds.yMin; j <= PlayerMemory_Tilemap_Bounds.yMax; j ++)
                {
                    PlayerMemory_Tilemap.SetColor(new Vector3Int(i, j, 0), new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f));
                }
            }
        }
    }
}
