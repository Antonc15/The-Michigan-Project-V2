using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitals : MonoBehaviour
{
    [Header("Natural Regen")]
    public bool doNaturalRegen;
    public float healthPerSecond;
    public float regenAfterDamageTime;

    [Header("Other Stuff")]
    public Image healthBar;
    public float maxHealth;

    float health;
    float timeUntilRegen;

    void Update()
    {
        //************* This handles natural regen ***********\\
        if (doNaturalRegen && timeUntilRegen < Time.time)
        {
                if (health < maxHealth)
                {
                    health += healthPerSecond * Time.deltaTime;
                }
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        //************* This handles the health bar display ***********\\
        if (healthBar != null)
        {
            healthBar.fillAmount = health / maxHealth;
        }
    }

    void Awake()
    {
        health = maxHealth;
    }
    public void TakeDamage(int amount)
    {
        timeUntilRegen = regenAfterDamageTime + Time.time;
        health -= amount;

        //This is the dead thingy
        if(health < 1)
        {
            Debug.Log("Dead, Health: " + health);
        }
    }
}
