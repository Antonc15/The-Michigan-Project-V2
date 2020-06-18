using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitals : MonoBehaviour
{
    public Image healthBar;
    public float maxHealth;

    [SerializeField]
    float health;

    void Awake()
    {
        health = maxHealth;    
    }
    public void TakeDamage(int amount)
    {
        health -= amount;

        if(healthBar != null)
        {
            healthBar.fillAmount = health / maxHealth;
        }
    }
}
