using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class KeyManager : MonoBehaviourWithReset
{
    [SerializeField] GameObject guide;

    public bool isPickedUp;
    private Vector2 vel;
    public float smoothTime;

    public float maxFollowDistance = 5f;
    private Vector3 originalPosition;

    // Reset Component Variable
    private bool isPickedUpInitial;
    private Vector3 originalPositionInitial;

    // Awake is called before the first frame update and before Start
    void Awake()
    {
        originalPosition = transform.position;

        // Record Instantiation Variables
        isPickedUpInitial = isPickedUp;
        originalPositionInitial = originalPosition;
    }
    // Update is called once per frame
    void Update()
    {
        if (isPickedUp)
        {
            Vector3 offset = new Vector3(0, 0, 0); // offset so the key doesn't clip into the sprite, add actual values once art is added
            float dist = Vector2.Distance(transform.position, guide.transform.position + offset);

            if (dist > maxFollowDistance) // guide too far from key
            {
                UntetherKey();
            } else
            {
                transform.position = Vector2.SmoothDamp(transform.position, guide.transform.position + offset, ref vel, smoothTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Guide") && !isPickedUp)
        {
            isPickedUp = true;
            FindObjectOfType<AudioManager>().Play("KeyPickUp");
        }
    }

    public void UntetherKey()
    {
        isPickedUp = false;
        FindObjectOfType<AudioManager>().Play("KeyDrop");
        transform.position = originalPosition;

        // reset velocity so it doesn’t keep sliding
        vel = Vector2.zero;

        UnityEngine.Debug.Log("Key detached");
    }

    public override void ResetToInstantiation()
    {
        isPickedUp = isPickedUpInitial;
        this.transform.position = originalPositionInitial;
    }
}
