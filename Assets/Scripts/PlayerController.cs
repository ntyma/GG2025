using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    [Header("Required Components")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    

    private Vector2 moveDirection;
    private InputAction move;
    private InputAction jump;

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
        jump.canceled += SmallJump;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        jump.performed -= Jump;
        jump.canceled -= SmallJump;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = new Vector2(moveDirection.x * speed, rigidBody.velocity.y);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(!isGrounded())
        {
            return;
        }
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce);
    }

    private void SmallJump(InputAction.CallbackContext context)
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y * 0.5f);
    }

    private bool isGrounded()
    {
        if(Physics2D.OverlapBox(groundChecker.position, groundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
    }
}
