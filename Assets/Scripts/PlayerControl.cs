using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private float horizontal;
    private float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // reset movement if dialogue is active
        if (DialogueManager.Instance.isDialogueActive)
        {
            movement = Vector2.zero;
            return;
        }

        // wasd input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        movement = new Vector2(h, v).normalized;
    }

    void FixedUpdate()
    {
        // only move the player if dialogue is not active
        if (!DialogueManager.Instance.isDialogueActive)
        {
            // uses Rigidbody2D to move the player
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
