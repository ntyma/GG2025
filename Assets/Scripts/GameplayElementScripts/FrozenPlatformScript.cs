using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenPlatformScript : MonoBehaviourWithReset
{
    [SerializeField] private SpriteRenderer frozenPlatformSpriteRenderer;
    [SerializeField] private BoxCollider2D frozenPlatformBoxCollider;
    [SerializeField] private Rigidbody2D frozenPlatformRigidBody;

    public bool isFrozen = true;
    [SerializeField] private float freezeLevel = 100.0f;
    [SerializeField] private float thawRate = 50.0f;

    [SerializeField] private GameObject platformMovePositions;
    private Vector3 movementOrigin;
    private Vector3 movementDestination;
    [SerializeField] private float moveTime = 2.0f;

    // Reset Component Variable
    private Color ColorInitial;
    private bool isFrozenInitial;
    private float freezeLevelInitial;

    // Awake is called before the first frame update and before Start
    void Awake()
    {
        // Initialize Platform Start and End Positions
        movementOrigin = platformMovePositions.transform.GetChild(0).position;
        this.transform.position = movementOrigin;

        movementDestination = platformMovePositions.transform.GetChild(1).position;

        // Record Instantiation Variables
        isFrozenInitial = isFrozen;
        freezeLevelInitial = freezeLevel;
        ColorInitial = frozenPlatformSpriteRenderer.color;
    }

    // Thaw Frozen Platform at the rate of thawRate per Second when Guide's Light touches
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light" && freezeLevel > 0.0f)
        {
            freezeLevel = freezeLevel - (thawRate * Time.deltaTime);
            // Update Frozen Platform Sprite Opacity to reflect Freeze Level
            Color currColor = frozenPlatformSpriteRenderer.color;
            currColor = currColor - new Color(0.0f, 0.0f, 0.0f, (thawRate * Time.deltaTime * 0.01f));
            frozenPlatformSpriteRenderer.color = currColor;
            // Platform Thaws when freezeLevel becomes 0.0f or less
            if (freezeLevel <= 0.0f)
            {
                ThawPlatform();
            }
        }
    }
    // Thaw Frozen Platform and engage normal behaviour
    private void ThawPlatform()
    {
        freezeLevel = 0.0f;
        isFrozen = !isFrozen;
        frozenPlatformSpriteRenderer.color = Color.yellow;
        // Move Platform to movementDestination in moveTime Seconds
        StartCoroutine(MoveToPosition(movementDestination, moveTime));
    }
    // Coroutine that moves the Platform to targetPosition in moveTime Seconds
    private IEnumerator MoveToPosition(Vector3 targetPosition, float moveTime)
    {
        Vector3 currPosition = transform.position;

        float elapsedTime = 0.0f;
        while (elapsedTime < moveTime)
        {
            // (elapsedTime / moveTime) is a ratio, and can be thought of as the percentage progress between starting and ending time
            transform.position = Vector3.Lerp(currPosition, targetPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public override void ResetToInstantiation()
    {
        this.transform.position = movementOrigin;
        isFrozen = isFrozenInitial;
        freezeLevel = freezeLevelInitial;
        frozenPlatformSpriteRenderer.color = ColorInitial;
    }
}
