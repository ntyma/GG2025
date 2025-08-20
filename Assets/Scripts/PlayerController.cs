using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    [Header("Gravit")]
    [SerializeField] private float baseGravity;
    [SerializeField] private float fallSpeedMultiplier;
    [SerializeField] private float maxFallSpeed;

    [Header("Required Components")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    

    private Vector2 moveDirection;
    private InputAction move;
    private InputAction jump;
    [SerializeField] private bool checkAirStatus = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        jump.performed -= Jump;
    }

    private void Start()
    {
        baseGravity = rigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        Gravity();
        UpdateAnimation();
        animator.SetBool("isJumping", !IsGrounded());
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = new Vector2(moveDirection.x * speed, rigidBody.velocity.y);
        
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("speed", Mathf.Abs(moveDirection.x * speed));
        if(moveDirection.x >= 0)
        {
            spriteRenderer.flipX = true;
        } else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(IsGrounded())
        {
            animator.SetBool("isJumping", true);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce);
        }
        
    }

    private bool IsGrounded()
    {
        if(Physics2D.OverlapBox(groundChecker.position, groundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void Gravity()
    {
        if (rigidBody.velocity.y < 0)
        {
            rigidBody.gravityScale = baseGravity * fallSpeedMultiplier;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Max(rigidBody.velocity.y, -maxFallSpeed));
        } else
        {
            rigidBody.gravityScale = baseGravity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(groundChecker.position, groundCheckSize);
    }
}
