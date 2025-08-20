using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Transform respawnPoint;
    public FollowMouse guide;
    public GameManagerScript gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        // add sound here

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            // dead, we'll have to link to a death animation later
            UnityEngine.Debug.Log("dead");

            Respawn();
        }
    }

    public void Respawn()
    {
        transform.position = respawnPoint.position;
        guide.transform.position = respawnPoint.position;
        currentHealth = maxHealth; // restore full health on respawn

        gameManagerScript.ResetLevelObstacles(gameManagerScript.currentGameLevel);
    }
}
