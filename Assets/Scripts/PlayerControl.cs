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
        // disable character input if dialogue is active
        //if (DialogueManager.Instance.isDialogueActive)
        //{
        //    horizontal = 0f;
        //    return;
        //}

        // wasd input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        movement = new Vector2(h, v).normalized;
    }

    void FixedUpdate()
    {
        // uses Rigidbody2D to move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
