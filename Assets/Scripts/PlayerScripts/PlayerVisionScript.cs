using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisionScript : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GameObject playerVisionGameObject;
    [SerializeField] private SpriteMask playerVisionSpriteMask;
    [SerializeField] private SpriteRenderer playerParanoiaSpriteRenderer;

    [SerializeField] private Vector3 playerVisionScaleMax = new Vector3(3.0f, 3.0f, 1.0f);
    [SerializeField] private Vector3 playerVisionScaleMin = new Vector3(0.8f, 0.8f, 1.0f);

    [SerializeField] private float playerSanity = 100.0f;
    [SerializeField] private float playerSanityLowerBound = 0.0f;
    [SerializeField] private float playerSanityUpperBound = 100.0f;
    [SerializeField] private float playerSanityRegainRate = 50.0f;
    [SerializeField] private float playerSanityDepletionRate = 20.0f;

    private bool isForwardRoute = true;

    // Start is called before the first frame update
    void Start()
    {
        SetAllSpriteComponents(true);
        playerSanity = playerSanityUpperBound;
    }

    // Update is called once per frame
    void Update()
    {
        // Regain Sanity when Player is in Light
        if (playerScript.playerIsInLight)
        {
            playerSanity = Mathf.Clamp
            (
                playerSanity + playerSanityRegainRate * Time.deltaTime,
                playerSanityLowerBound,
                playerSanityUpperBound
            );
        }
        // Deplete Sanity when Player is NOT in Light
        else
        {
            playerSanity = Mathf.Clamp
            (
                playerSanity - playerSanityDepletionRate * Time.deltaTime,
                playerSanityLowerBound,
                playerSanityUpperBound
            );
        }

        // Shrink player vision based on Sanity
        playerVisionGameObject.transform.localScale = Vector3.Lerp
        (
            playerVisionScaleMin, 
            playerVisionScaleMax,
            playerSanity*0.01f
        );
    }

    public void SetRoute(bool isForwardRoute = true)
    {
        this.isForwardRoute = isForwardRoute;

        // Enable Paranoia only for ForwardRoute, and Disables in for BackwardRoute
        SetAllSpriteComponents(this.isForwardRoute);
    }
    // Enables or Disables the Player Vision Sprite Mask and Player Paranoia Sprite Renderer based on Input
    private void SetAllSpriteComponents(bool Input = true)
    {
        playerVisionSpriteMask.enabled = Input;
        playerParanoiaSpriteRenderer.enabled = Input; 
    }
}
