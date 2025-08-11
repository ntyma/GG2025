using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMemoryScript : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GridLayout gridLayout;

    public Vector3 worldPosition;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("START()");
        BoundsInt bounds = new BoundsInt(((int)this.transform.position.x) - 1, ((int)this.transform.position.y) - 1, 0, 2, 2, 1);
        TileBase[] tileArray = tilemap.GetTilesBlock(bounds);
        for (int index = 0; index < tileArray.Length; index++)
        {
            //Debug.Log(tileArray[index]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Vector3Int cellPosition = gridLayout.WorldToCell(worldPosition);
            //Debug.Log(cellPosition);
            //tilemap.SetTileFlags(cellPosition, TileFlags.None);
            tilemap.SetColor(cellPosition, new Vector4(1, 1, 1, 0));
            //tilemap.SetColor(cellPosition, new Vector4(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1));
            //Debug.Log (tilemap.GetColor(cellPosition));

            //tilemap.SetTileFlags(new Vector3Int(-1, -5 , 0), TileFlags.None);
            tilemap.SetColor(new Vector3Int(-1, -5, 0), new Vector4(1, 1, 1, 0));

            //tilemap.SetTileFlags(new Vector3Int(-2, -5, 0), TileFlags.None);
            tilemap.SetColor(new Vector3Int(-2, -5, 0), new Vector4(1, 1, 1, 0));

            //tilemap.SetTileFlags(new Vector3Int(-3, -5, 0), TileFlags.None);
            tilemap.SetColor(new Vector3Int(-3, -5, 0), new Vector4(1, 1, 1, 0));

            //tilemap.SetTileFlags(new Vector3Int(0, -5, 0), TileFlags.None);
            tilemap.SetColor(new Vector3Int(0, -5, 0), new Vector4(1, 1, 1, 0));
            
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Vector3Int cellPosition = gridLayout.WorldToCell(worldPosition);
            //Debug.Log("RGB AT " + cellPosition + " is " + tilemap.GetColor(cellPosition));

            tilemap.SetColor(cellPosition, Color.white);
        }
        /*Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, 7);
        foreach (Collider2D hitCollider in hitColliders)
        {
            Debug.Log("collided with " + hitCollider);
            // Process hitCollider.gameObject
        }*/
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7) // Memory Layer
            Debug.Log("collided with " + collision);
    }*/
}
