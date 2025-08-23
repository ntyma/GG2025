using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class FillStatusBar : MonoBehaviour
{
    public Health health;
    public Image fillImage;
    private Slider healthBar;

    // Start is called before the first frame update
    void Awake()
    {
        healthBar = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Clear health bar when at 0 health
        if (healthBar.value <= 0)
        {
            fillImage.enabled = false;
        }

        // If health is above 0, show bar
        if (healthBar.value > healthBar.minValue && !fillImage.enabled)
        {
            fillImage.enabled = true;
        }

        float fillValue = (float)health.currentHealth / health.maxHealth;

        // danger health below 1/3
        if (fillValue <= (float)1/3)
        {
            fillImage.color = Color.red;
        }
        else if(fillValue > (float)1/3)
        {
            fillImage.color = Color.green;
        }

        healthBar.value = fillValue;
    }
}
