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
        //LoadTileData();
    }
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.T)){
            Debug.Log("saving");
            SaveTileData();
        }
        /*
        if (siblingIndex == 0 && Input.GetKeyDown(KeyCode.L))
            LoadTileData();*/
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
        for (int i = 0; i < memorizedTilesArray.Length; i++)
        {
            Debug.Log("currently trying this sibling index: " + (siblingIndex * 100 + i));
            Debug.Log("my sibling index is " + siblingIndex);
            SaveManager.UpdateSaveData(data => data.playerMemory[siblingIndex * 100 + i] = memorizedTilesArray[i]);
        }

    }
    public void LoadTileData()
    {
        int arraySize = (this.playerMemoryTilemapBounds.size.x + 1) * (this.playerMemoryTilemapBounds.size.y + 1);

        //
        // Load Data from Save
        //
        bool[] allMemorizedTiles = new bool[19*100];
        bool[] memorizedTilesArray = new bool[arraySize];

        allMemorizedTiles = SaveManager.GetSpecificData(data => data.playerMemory);

        for(int i = 0; i < arraySize; i++)
        {
            memorizedTilesArray[i] = allMemorizedTiles[siblingIndex * 100 + i];
        }

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
