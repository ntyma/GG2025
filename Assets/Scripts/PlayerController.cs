using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpSpeedMultiplier;
    [SerializeField] private float speedMultiplier = 1;

    [Header("Gravity")]
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
    //[SerializeField] private AnimationEndBehaviour jumpStartAnimationBehaviour;
    [SerializeField] private AnimationClip jumpStartClip;
    [SerializeField] private AnimationClip jumpLandClip;

    [SerializeField] private bool wasGrounded;
    [SerializeField] private bool isGrounded;
    private bool isJumping;

    [Header("Testing")]
    [SerializeField] private bool testingMode;
    private Vector2 moveDirection;
    private InputAction move;
    private InputAction jump;

    private void Awake()
    {
        playerControls = new PlayerControls();
        //jumpStartAnimationBehaviour.OnAnimationEnded += JumpStartAnimEnded;
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
        wasGrounded = isGrounded;
        isGrounded = IsGrounded();

        moveDirection = move.ReadValue<Vector2>();
        Gravity();
        UpdateAnimation();
        animator.SetBool("isJumping", !isGrounded);

        if(!isJumping)
        {
            animator.SetBool("isFalling", !isGrounded);
        }
        if(!wasGrounded && isGrounded)
        {
            Debug.Log("XX 80");
            OnLand();
        }
        
        if (testingMode)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("Die");
                Die();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("Lock controls");
                LockPlayerControls();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("unLock controls");
                UnlockPlayerControls();
            }
        }
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = new Vector2(moveDirection.x * speed * speedMultiplier, rigidBody.velocity.y);
        
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("speed", Mathf.Abs(moveDirection.x * speed));
        if(moveDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        } else if (moveDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(IsGrounded())
        {
            animator.SetTrigger("StartJump");
            LockPlayerControls();
            isJumping = true;
            speedMultiplier = jumpSpeedMultiplier;
            //animator.SetBool("isJumping", true);
            //rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce);
            Invoke(nameof(JumpStartAnimEnded), jumpStartClip.length);
        }
        
    }

    private void OnLand()
    {
        Debug.Log("PLAYER LANDED");
        animator.SetTrigger("LandJump");
        LockPlayerControls();
        rigidBody.velocity = Vector2.zero;
        speedMultiplier = 1f;
        Invoke(nameof(JumpLandAnimEnded), jumpLandClip.length);
        isJumping = false;
    }

    private void JumpStartAnimEnded()
    {
        //Debug.Log("PLAYER jump start ended");
        animator.SetBool("isJumping", true);
        UnlockPlayerControls();
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce);
    }

    private void JumpLandAnimEnded()
    {
        Debug.Log("PLAYER LANDED animation ended");
        UnlockPlayerControls();
    }

    // True -> touching ground
    // False -> Not touching ground
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

    public void Die()
    {
        animator.SetTrigger("TriggerDeath");
        LockPlayerControls();
    }

    public void LockPlayerControls()
    {
        playerControls.Disable();
    }

    public void UnlockPlayerControls()
    {
        playerControls.Enable();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(groundChecker.position, groundCheckSize);
    }
}
