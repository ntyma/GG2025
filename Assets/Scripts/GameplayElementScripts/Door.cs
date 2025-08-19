using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool locked;
    private Collider2D col;
    private SpriteRenderer lockSpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        locked = true;
        col = GetComponent<Collider2D>();
        col.isTrigger = false;

        lockSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            locked = false;
            col.isTrigger = true;
            lockSpriteRenderer.color = new Color(0.0f, 1.0f, 1.0f, 0.3f);
            Destroy(other.gameObject);
        }
    }
}
