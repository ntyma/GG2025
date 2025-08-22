using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 velocity;
    public float lifeTime;
    public bool destroyOnWall;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime < 0) Destroy(gameObject);
        rb.velocity = velocity;
        //Debug.Log(rb.velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision) // Take damage when colliding with the player
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var healthComponent = collision.gameObject.GetComponent<Health>();
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(1);
                Destroy(gameObject);
            }
        }
        if(destroyOnWall) Destroy(gameObject);
    }
}
