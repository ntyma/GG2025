using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMemoryScript : MonoBehaviour
{
    [SerializeField] private Tilemap currentPlayerMemoryTilemap;
    [SerializeField] private GridLayout gridLayout;

    [SerializeField] int playerMemoryRadius = 3;

    // Update is called once per frame
    void Update()
    {
        Vector3Int playerPositionInGrid = gridLayout.WorldToCell(this.transform.position);
        // Check tiles surrounding the Player for Memory Tiles
        for (int i = playerPositionInGrid.x - playerMemoryRadius + 1; i <= playerPositionInGrid.x + playerMemoryRadius - 1; i ++)
        {
            for (int j = playerPositionInGrid.y - playerMemoryRadius + 1; j <= playerPositionInGrid.y + playerMemoryRadius - 1; j ++)
            {
                currentPlayerMemoryTilemap.SetColor
                    (
                        new Vector3Int(i, j, 0),
                        new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                    );
            }
        }
    }

    public void SetPlayerMemoryTilemap (Tilemap tilemapInput)
    {
        currentPlayerMemoryTilemap = tilemapInput;
    }
}
