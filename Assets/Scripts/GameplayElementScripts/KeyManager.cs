using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    [SerializeField] GameObject guide;

    public bool isPickedUp;
    private Vector2 vel;
    public float smoothTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickedUp)
        {
            Vector3 offset = new Vector3(0, 0, 0); // offset so the key doesn't clip into the sprite, add actual values once art is added
            transform.position = Vector2.SmoothDamp(transform.position, guide.transform.position + offset, ref vel, smoothTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Guide") && !isPickedUp)
        {
            isPickedUp = true;
        }
    }
}
