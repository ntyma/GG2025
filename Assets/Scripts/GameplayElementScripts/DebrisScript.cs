using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisScript : MonoBehaviourWithReset
{
    [SerializeField] private Vector3 respawnPoint;
    [SerializeField] private float respawnHeight;
    [SerializeField] private Rigidbody2D debrisRigidBody;

    public float debrisSpeed = 2.0f;

    // Reset Component Variable
    private Vector3 positionInitial;
    // Awake is called before the first frame update and before Start
    void Awake()
    {
        // Record Instantiation Variables
        positionInitial = this.transform.position;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Set falling velocity of Debris
        debrisRigidBody.velocity = Vector3.down * debrisSpeed;

        // Respawn debris if it falls below a certain height
        if (this.transform.position.y <= respawnHeight)
        {
            this.transform.position = respawnPoint;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) // Take damage when colliding with the player
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            var healthComponent = collision.gameObject.GetComponent<Health>();
            if(healthComponent != null)
            {
                healthComponent.TakeDamage(1);
            }
        }
    }
    public override void ResetToInstantiation()
    {
        this.transform.position = positionInitial;
    }
}
