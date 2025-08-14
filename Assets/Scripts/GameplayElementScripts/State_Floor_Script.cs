using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Floor_Script : MonoBehaviour
{
    [SerializeField] private BoxCollider2D State_Floor_Collider2D;
    [SerializeField] private SpriteRenderer State_Floor_SpriteRenderer;

    [SerializeField] private bool isTangible = true;

    private void Start()
    {
        // Update isTrigger to match Tangibility
        // Note: isTrigger and isTangible should always be opposites of each other
        State_Floor_Collider2D.isTrigger = !isTangible;
        UpdateColor();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("entering " + collision.gameObject.tag);
        // Update Tangibility if Light touches this object
        if (collision.gameObject.tag == "Light")
            ChangeStates();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("exiting " + collision.gameObject.tag);
        // Update Tangibility if Light touches this object
        if (collision.gameObject.tag == "Light")
            ChangeStates();
    }

    // Function to update Sprite Color to reflect tangibility
    private void UpdateColor()
    {
        Color currColor = State_Floor_SpriteRenderer.color;
        if (isTangible)
            State_Floor_SpriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, 1.0f);
        else
            State_Floor_SpriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, 0.3f);
    }

    [ContextMenu("ChangeStates()")]
    // Function to change the Tangibility of Object
    private void ChangeStates()
    {
        //Debug.Log("CHANGESTATES()");
        isTangible = !isTangible;
        State_Floor_Collider2D.isTrigger = !State_Floor_Collider2D.isTrigger;

        UpdateColor();
    }
}
