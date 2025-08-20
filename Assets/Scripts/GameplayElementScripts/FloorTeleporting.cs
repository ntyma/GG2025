using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTeleporting : MonoBehaviourWithReset
{
    [SerializeField] private BoxCollider2D floorTeleportingCollider;
    [SerializeField] private GameObject teleportPointsGameObject;

    public int teleportPointsCurrentIndex = 0;
    [SerializeField] private Vector3[] teleportPointsCollection;
    [SerializeField] private int teleportPointsCount;

    // Reset Component Variable
    private int teleportPointsCurrentIndexInitial;

    // Awake is called before the first frame update and before Start
    void Awake()
    {
        // Collect all Teleport Points from teleportPointsGameObject
        teleportPointsCount = teleportPointsGameObject.transform.childCount;
        teleportPointsCollection = new Vector3[teleportPointsCount];

        // Index all points (Order seems to be preserved based on the order in the Hierarchy)
        int i = 0;
        foreach (Transform childTransform in teleportPointsGameObject.transform)
            teleportPointsCollection[i++] = childTransform.position;

        // Clamp values for Index Out of Bounds Protection
        teleportPointsCurrentIndex = Mathf.Clamp(teleportPointsCurrentIndex, 0, teleportPointsCount - 1);
        // Set position of object based on starting Index
        this.transform.position = teleportPointsCollection[teleportPointsCurrentIndex];

        // Record Instantiation Variables
        teleportPointsCurrentIndexInitial = teleportPointsCurrentIndex;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Light")
            return;
            
        Teleport();
    }
    // Teleport Object to the position in the next Index/ next Teleport Point in Hierarchy
    private void Teleport()
    {
        teleportPointsCurrentIndex = (teleportPointsCurrentIndex + 1) % teleportPointsCount;
        this.transform.position = teleportPointsCollection[teleportPointsCurrentIndex];
    }

    public override void ResetToInstantiation()
    {
        teleportPointsCurrentIndex = teleportPointsCurrentIndexInitial;
        this.transform.position = teleportPointsCollection[teleportPointsCurrentIndex];
    }
}
