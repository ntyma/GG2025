using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisScript : MonoBehaviourWithReset
{
    [SerializeField] private Vector3 respawnPoint;
    [SerializeField] private float respawnHeight;
    [SerializeField] private Rigidbody2D debrisRigidBody;
    [SerializeField] private int Damage = 5;
    public float debrisSpeed = 2.0f;

    // Reset Component Variable

    // Awake is called before the first frame update and before Start
    void Awake()
    {
        respawnPoint = this.transform.position;
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
                healthComponent.TakeDamage(Damage);
            }
        }
    }
    public override void ResetToInstantiation()
    {
        this.transform.position = respawnPoint;
    }
}
