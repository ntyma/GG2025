using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private BoxCollider2D playerBoxCollider;
    [SerializeField] private Health playerHealthScript;
    [SerializeField] private PlayerVisionScript playerVisionScript;
    [SerializeField] private PlayerMemoryScript playerMemoryScript;

    public bool isFacingRight = true;
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float jumpForce = 1.0f;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private float stepInterval = 0.4f; // how often footsteps play when walking

    [SerializeField] private LayerMask whatIsGround;

    public bool isForwardRoute = false;
    public bool playerIsInLight = false;
    public bool playerIsInCover = false;

    private float stepTimer = 0f; // timer for footsteps
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

        Footsteps();
    }

    private bool isGrounded()
    {
        RaycastHit2D temp = Physics2D.BoxCast(playerBoxCollider.bounds.center, playerBoxCollider.bounds.size,
                                     0, Vector2.down, checkRadius, whatIsGround);
        return temp.collider != null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
            playerIsInLight = true;
        else if (collision.gameObject.tag == "Cover")
            playerIsInCover = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
            playerIsInLight = false;
        else if (collision.gameObject.tag == "Cover")
            playerIsInCover = false;
    }

    public void SetRoute(bool isForwardRoute = true)
    {
        this.isForwardRoute = isForwardRoute;
        this.playerHealthScript.SetRoute(isForwardRoute);
        this.playerVisionScript.SetRoute(isForwardRoute);
    }
    public void SetPlayerMemoryTilemap (Tilemap Input)
    {
        playerMemoryScript.SetPlayerMemoryTilemap(Input);
    }
    private void Footsteps()
    {
        bool isMoving = Mathf.Abs(playerRigidBody.velocity.x) > 0.1f;
        bool grounded = isGrounded();

        if (isMoving && grounded)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                AudioManager.instance.Play("Footstep"); // single step sound
                stepTimer = stepInterval; // reset timer
            }
        }
        else
            stepTimer = 0f; // reset when not moving
    }
}
