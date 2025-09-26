using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideScript : MonoBehaviour
{
    public SpriteMask guideSpriteMask;

    public Vector2 screenPosition;
    public Vector2 worldPosition;

    // Update is called once per frame
    void Update()
    {
        // Convert Mouse Position on Monitor to a Coordinate in Game World
        screenPosition = Input.mousePosition;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        // Update Guide Position
        this.transform.position = worldPosition;
    }
}
