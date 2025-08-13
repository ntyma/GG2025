using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    void takeDamage(int amount)
    {
        // add sound here

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            // dead, we'll have to link to a death animation later
        }
    }

    void heal(int amount)
    {
        // add sound here

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // cap health to max health
        }
    }
}
