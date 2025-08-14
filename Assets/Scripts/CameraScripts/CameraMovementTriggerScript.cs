using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementTriggerScript : MonoBehaviour
{
    [SerializeField] private BoxCollider2D cameraMovementTriggerBoxCollider;
    [SerializeField] private MainCameraScript mainCameraScript;

    [SerializeField] private enum Direction
    {
        Right, Left, Up, Down, NoDirection
    }
    [SerializeField] private Direction forwardDirection = Direction.Right;

    private Direction backwardDirection = Direction.Left;
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
            case Direction.Down:
                backwardDirection = Direction.Up;
                break;
            default:    // case Direction.NoDirection:
                Debug.Log("Forward Direction for CameraMovementTrigger CANNOT be NoDirection!");
                forwardDirection = Direction.Right;
                backwardDirection = Direction.Left;
                break;
        }
    }

    private Direction playerEnterDirectionX;
    private Direction playerEnterDirectionY;
    private Direction playerExitDirectionX;
    private Direction playerExitDirectionY;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only consider collisions from Player
        if (collision.gameObject.tag != "Player")
            return;
        // Continue if playerRigidBody is acquired
        if (!collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D playerRigidBody))
        {
            Debug.Log("playerRigidBody NOT FOUND in CameraMovementTriggerScript!");
            return;
        }
        // Check Player's directions when entering Trigger
        if (playerRigidBody.velocity.x > 0.0f)
            playerEnterDirectionX = Direction.Right;
        else if (playerRigidBody.velocity.x < 0.0f)
            playerEnterDirectionX = Direction.Left;
        else
            playerEnterDirectionX = Direction.NoDirection;

        if (playerRigidBody.velocity.y > 0.0f)
            playerEnterDirectionY = Direction.Up;
        else if (playerRigidBody.velocity.y < 0.0f)
            playerEnterDirectionY = Direction.Down;
        else
            playerEnterDirectionY = Direction.NoDirection;     
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Only consider collisions from Player
        if (collision.gameObject.tag != "Player")
            return;
        // Continue if playerRigidBody is acquired
        if (!collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D playerRigidBody))
        {
            Debug.Log("playerRigidBody NOT FOUND in CameraMovementTriggerScript!");
            return;
        }
        // Check Player's direction when exiting Trigger
        if (playerRigidBody.velocity.x > 0.0f)
            playerExitDirectionX = Direction.Right;
        else if (playerRigidBody.velocity.x < 0.0f)
            playerExitDirectionX = Direction.Left;
        else
            playerExitDirectionX = Direction.NoDirection;

        if (playerRigidBody.velocity.y > 0.0f)
            playerExitDirectionY = Direction.Up;
        else if (playerRigidBody.velocity.y < 0.0f)
            playerExitDirectionY = Direction.Down;
        else
            playerExitDirectionY = Direction.NoDirection;

        // Update Camera Position
        if (forwardDirection == Direction.Right || backwardDirection == Direction.Right)
        {
            if (playerEnterDirectionX == playerExitDirectionX)
                mainCameraScript.UpdateCamera(forwardDirection == playerEnterDirectionX);
        }
        else if (forwardDirection == Direction.Up || backwardDirection == Direction.Up)
        {
            if (playerEnterDirectionY == playerExitDirectionY)
                mainCameraScript.UpdateCamera(forwardDirection == playerEnterDirectionY);
        }
    }
}
