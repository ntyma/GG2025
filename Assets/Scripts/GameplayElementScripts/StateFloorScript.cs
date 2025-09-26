using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFloorScript : MonoBehaviourWithReset
{
    [SerializeField] private BoxCollider2D stateFloorCollider;
    [SerializeField] private SpriteRenderer stateFloorSpriteRenderer;
    [SerializeField] private SpriteRenderer stateFloorIlluminatedSpriteRenderer;
    [SerializeField] private bool isTangible = true;

    [SerializeField] private float tangibleAlpha = 1.0f;
    [SerializeField] private float intangibleAlpha = 0.0f;

    // Reset Component Variable

    // Awake is called before the first frame update and before Start
    void Awake()
    {
        // Update isTrigger to match Tangibility
        // Note: isTrigger and isTangible should always be opposites of each other
        stateFloorCollider.isTrigger = !isTangible;
        this.gameObject.layer = (isTangible ? LayerMask.NameToLayer("Floor") : 0);
        UpdateColor();

        // Record Instantiation Variables
    }
    [SerializeField] private int lightCount = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Light")
            return;

        // Update Tangibility if Light touches this object
        lightCount = lightCount + 1;
        if (lightCount == 1)
            ChangeStates();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Light")
            return;

        // Update Tangibility if Light touches this object
        lightCount = lightCount - 1;
        if (lightCount < 0)
            Debug.Log("lightCount should NEVER be less than 0 - from OnTriggerExit2D in StateFloorScript");
        if (lightCount == 0)
            ChangeStates();
    }

    // Function to update Sprite Color to reflect tangibility
    private void UpdateColor()
    {
        Color currColor = stateFloorSpriteRenderer.color;
        Color currColorIlluminated = stateFloorIlluminatedSpriteRenderer.color;
        if (isTangible)
        {
            //stateFloorSpriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, tangibleAlpha);
            stateFloorIlluminatedSpriteRenderer.color = new Color(currColorIlluminated.r, currColorIlluminated.g, currColorIlluminated.b, tangibleAlpha);
        }
        else
        {
            //stateFloorSpriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, intangibleAlpha);
            stateFloorIlluminatedSpriteRenderer.color = new Color(currColorIlluminated.r, currColorIlluminated.g, currColorIlluminated.b, intangibleAlpha); 
        }
    }

    [ContextMenu("ChangeStates()")]
    // Function to change the Tangibility of Object
    private void ChangeStates()
    {
        isTangible = !isTangible;
        stateFloorCollider.isTrigger = !stateFloorCollider.isTrigger;
        this.gameObject.layer = (isTangible ? LayerMask.NameToLayer("Floor") : 0);

        UpdateColor();
    }

    public override void ResetToInstantiation()
    {
        return;
    }
}
