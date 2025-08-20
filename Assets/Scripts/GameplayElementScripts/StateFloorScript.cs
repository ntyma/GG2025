using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFloorScript : MonoBehaviourWithReset
{
    [SerializeField] private BoxCollider2D stateFloorCollider;
    [SerializeField] private SpriteRenderer stateFloorSpriteRenderer;

    [SerializeField] private bool isTangible = true;

    // Reset Component Variable

    // Awake is called before the first frame update and before Start
    void Awake()
    {
        // Update isTrigger to match Tangibility
        // Note: isTrigger and isTangible should always be opposites of each other
        stateFloorCollider.isTrigger = !isTangible;
        UpdateColor();

        // Record Instantiation Variables
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Update Tangibility if Light touches this object
        if (collision.gameObject.tag == "Light")
            ChangeStates();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Update Tangibility if Light touches this object
        if (collision.gameObject.tag == "Light")
            ChangeStates();
    }

    // Function to update Sprite Color to reflect tangibility
    private void UpdateColor()
    {
        Color currColor = stateFloorSpriteRenderer.color;
        if (isTangible)
            stateFloorSpriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, 1.0f);
        else
            stateFloorSpriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, 0.3f);
    }

    [ContextMenu("ChangeStates()")]
    // Function to change the Tangibility of Object
    private void ChangeStates()
    {
        //Debug.Log("CHANGESTATES()");
        isTangible = !isTangible;
        stateFloorCollider.isTrigger = !stateFloorCollider.isTrigger;

        UpdateColor();
    }

    public override void ResetToInstantiation()
    {
        return;
    }
}
