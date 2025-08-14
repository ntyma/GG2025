using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CircleCollider2D keyCollider;
    [SerializeField] private BoxCollider2D lockCollider;
    [SerializeField] private SpriteRenderer lockSpriteRenderer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player collects the Key
        if (collision.gameObject.tag == "Player")
        {
            // Unlock the the door
            lockCollider.isTrigger = true;
            lockSpriteRenderer.color = new Color(0.0f, 1.0f, 1.0f, 0.3f);
        }
    }
}
