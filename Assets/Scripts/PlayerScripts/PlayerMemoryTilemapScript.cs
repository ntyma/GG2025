using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMemoryTilemapScript : MonoBehaviourWithReset
{
    [SerializeField] private Tilemap playerMemoryTilemap;
    [SerializeField] private BoundsInt playerMemoryTilemapBounds;

    [SerializeField] private int siblingIndex;

    // Start is called before the first frame update
    void Start()
    {
        siblingIndex = this.transform.GetSiblingIndex();

        playerMemoryTilemap = this.transform.GetComponent<Tilemap>();
        playerMemoryTilemap.CompressBounds();
        playerMemoryTilemapBounds = playerMemoryTilemap.cellBounds;
        
        ResetToInstantiation();
        // Load tile data from Save
    }
    private void Update()
    {
        if (siblingIndex == 0 && Input.GetKeyDown(KeyCode.T))
            SaveTileData();
    }
    public override void ResetToInstantiation()
    {
        // Set every Tile to be Invisible
        for (int i = playerMemoryTilemapBounds.xMin; i <= playerMemoryTilemapBounds.xMax; i++)
        {
            for (int j = playerMemoryTilemapBounds.yMin; j <= playerMemoryTilemapBounds.yMax; j++)
            {
                playerMemoryTilemap.SetColor
                    (
                        new Vector3Int(i, j, 0), 
                        new Color(1.0f, 1.0f, 1.0f, 0.0f)
                    );
            }
        }
    }

    public void SaveTileData()
    {
        int arraySize = (this.playerMemoryTilemapBounds.size.x+1) * (this.playerMemoryTilemapBounds.size.y+1);
        bool[] memorizedTilesArray = new bool[arraySize];
        int currentCount = 0;
        for (int i = playerMemoryTilemapBounds.xMin; i <= playerMemoryTilemapBounds.xMax; i++)
        {
            for (int j = playerMemoryTilemapBounds.yMax; j >= playerMemoryTilemapBounds.yMin; j--)
            {
                if (playerMemoryTilemap.GetColor(new Vector3Int(i, j, 0)).a <= 0.0f)
                {
                    memorizedTilesArray[currentCount++] = false;
                }
                else
                {
                    memorizedTilesArray[currentCount++] = true;
                }
            }
        }

        SaveData data = new SaveData
        {
            
        };
    }
    public void LoadTileData()
    {
        int arraySize = (this.playerMemoryTilemapBounds.size.x + 1) * (this.playerMemoryTilemapBounds.size.y + 1);

        //
        // Load Data from Save
        //
        bool[] memorizedTilesArray = new bool[arraySize];
        

        int currentCount = 0;
        for (int i = playerMemoryTilemapBounds.xMin; i <= playerMemoryTilemapBounds.xMax; i++)
        {
            for (int j = playerMemoryTilemapBounds.yMax; j >= playerMemoryTilemapBounds.yMin; j--)
            {
                if (memorizedTilesArray[currentCount++])
                    playerMemoryTilemap.SetColor
                        (
                            new Vector3Int(i, j, 0), 
                            new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                        );
            }
        }
    }

    // Fun little function that colors each Tile a random Color
    private void ColorRandomly()
    {
        for (int i = playerMemoryTilemapBounds.xMin; i <= playerMemoryTilemapBounds.xMax; i++)
        {
            for (int j = playerMemoryTilemapBounds.yMin; j <= playerMemoryTilemapBounds.yMax; j++)
            {
                playerMemoryTilemap.SetColor
                    (
                        new Vector3Int(i, j, 0), 
                        new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f)
                    );
            }
        }
    }
}
