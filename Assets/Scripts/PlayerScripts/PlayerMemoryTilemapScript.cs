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

    }
    public void LoadTileData()
    {

    }

    // Fun little function that colors each Tile a random Color
    private void ColorRandomly()
    {
        for (int i = playerMemoryTilemapBounds.xMin; i <= playerMemoryTilemapBounds.xMax; i++)
        {
            for (int j = playerMemoryTilemapBounds.yMin; j <= playerMemoryTilemapBounds.yMax; j++)
            {
                playerMemoryTilemap.SetColor(new Vector3Int(i, j, 0), new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f));
            }
        }
    }
}
