using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    [SerializeField] private CircleCollider2D lightCollider;

    private KeyManager keyManager;

    // Start is called before the first frame update
    void Start()
    {
        keyManager = FindObjectOfType<KeyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) // untether key when light hits enemy
    {
        if (other.CompareTag("Enemy"))
        {
            if (keyManager != null)
            {
                keyManager.UntetherKey();
            }
        }
    }
}
