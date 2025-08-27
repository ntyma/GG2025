using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingLightWallScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightWallSpriteRenderer;
    [SerializeField] private SpriteMask lightWallSpriteMask;
    [SerializeField] private BoxCollider2D lightWallBoxCollider;
    [SerializeField] private Rigidbody2D lightWallRigidBody;

    [SerializeField] private GameObject Target;

    [SerializeField] private float scrollingSpeed = 3.0f;
    [SerializeField] private float catchUpMultiplier = 1.0f;

    private void Update()
    {
        if (Vector3.Distance(this.transform.position, Target.transform.position) > (this.transform.localScale.x/2)+2)
        {
            catchUpMultiplier = 2.0f;
        }
        else
        {
            catchUpMultiplier = 1.0f;
        }
        this.transform.position = Vector3.MoveTowards
            (
                this.transform.position,
                Target.transform.position,
                scrollingSpeed * catchUpMultiplier * Time.deltaTime
            );
    }
    public void SetTarget(GameObject Input)
    {
        Target = Input;
    }
}
