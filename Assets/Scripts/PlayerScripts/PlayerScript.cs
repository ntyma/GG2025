using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{
    //[SerializeField] private Rigidbody2D playerRigidBody;
    //[SerializeField] private SpriteRenderer playerSpriteRenderer;
    //[SerializeField] private BoxCollider2D playerBoxCollider;
    [SerializeField] private Health playerHealthScript;
    [SerializeField] private PlayerVisionScript playerVisionScript;
    [SerializeField] private PlayerMemoryScript playerMemoryScript;
    [SerializeField] private PlayerController playerController;

    //public bool isFacingRight = true;
    //[SerializeField] private float walkSpeed = 1.0f;
    //[SerializeField] private float jumpForce = 1.0f;
    //[SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private float stepInterval = 0.4f; // how often footsteps play when walking

    //[SerializeField] private LayerMask whatIsGround;

    public bool isForwardRoute = false;
    public bool playerIsInLight = false;
    public bool playerIsInCover = false;

    private float stepTimer = 0f; // timer for footsteps
    private bool isPlayingBuzz;
    //private bool wasGrounded = false;

    private void Start()
    {
        playerController.OnJumpStart += PlayJumpAudio;
        playerController.OnJumpLand += PlayJumpLandAudio;
        playerController.OnMoving += Footsteps;
    }

    private void PlayJumpLandAudio()
    {
        AudioManager.instance.Play("JumpLand");
    }

    private void PlayJumpAudio()
    {
        AudioManager.instance.Play("Jump");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            playerIsInLight = true;
            if (!isPlayingBuzz)
            {
                isPlayingBuzz = true;
                AudioManager.instance.Play("GuideBuzz");
            }
        }
        else if (collision.gameObject.tag == "Cover")
            playerIsInCover = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            playerIsInLight = false;
            if (isPlayingBuzz)
            {
                isPlayingBuzz = false;
                AudioManager.instance.Stop("GuideBuzz");
            }
        }
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
    private void Footsteps(bool onMoving)
    {

        if (onMoving)
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
