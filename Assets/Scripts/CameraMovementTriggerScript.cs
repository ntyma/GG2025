using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementTriggerScript : MonoBehaviour
{
    [SerializeField] private BoxCollider2D cameraMovementTriggerBoxCollider;
    [SerializeField] private MainCameraScript mainCameraScript;

    [SerializeField] private enum Direction
    {
        Right, Left, Up, Down
    }
    [SerializeField] private Direction forwardDirection = Direction.Right;
    [SerializeField] private Direction backwardDirection = Direction.Left;
    private void Start()
    {
        // set backwardDirection to opposite of forwardDirection
        switch (forwardDirection)
        {
            case Direction.Right:
                backwardDirection = Direction.Left;
                break;
            case Direction.Left:
                backwardDirection = Direction.Right;
                break;
            case Direction.Up:
                backwardDirection = Direction.Down;
                break;
            default:    //case Direction.Down:
                backwardDirection = Direction.Up;
                break;
        }
    }
    //
    // BUG, Player can manipulate the Camera by not passing through the collider entirely, then leaving from where they came from
    //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        if (collision.gameObject.TryGetComponent<PlayerScript>(out PlayerScript playerScript))
        {
            // playerScript.isFacingRight is True when Player progresses Forwards
            // playerScript.isFacingRight is False when Player progresses Backwards
            mainCameraScript.UpdateCamera(playerScript.isFacingRight);
        }
        else
            Debug.Log("PlayerScript NOT FOUND!");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
