using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundSprite;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            backgroundSprite.enabled = !backgroundSprite.enabled;
        }
    }
}
