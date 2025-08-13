using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private BoxCollider2D playerBoxCollider;

    public bool isFacingRight = true;
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float jumpForce = 1.0f;
    [SerializeField] private float checkRadius = 0.5f;

    [SerializeField] private LayerMask whatIsGround;

    // Update is called once per frame
    void Update()
    {
        // GetAxisRaw returns int value {-1, 0, 1} depending of A or D is pressed
        playerRigidBody.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * walkSpeed, playerRigidBody.velocity.y);

        // Player is moving Right
        if (playerRigidBody.velocity.x >= 0.0f)
            isFacingRight = true;
        else
            isFacingRight = false;
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            playerRigidBody.velocity = Vector2.up * jumpForce;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D temp = Physics2D.BoxCast(playerBoxCollider.bounds.center, playerBoxCollider.bounds.size,
                                     0, Vector2.down, checkRadius, whatIsGround);
        return temp.collider != null;
    }
}
