using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{

    public Slider healthBar;
    public int health;
    public bool isHead;

    int maxHealth;

    void Awake()
    {
        maxHealth = health;
    }
    public void TakeDamage(int amount)
    {
        if (!isHead)
        {
            health -= amount;

            if (health <= 0f)
            {
                Die();
            }
        }
        else
        {
            Target target = transform.parent.GetComponent<Target>();
            target.TakeDamage(amount * 2);
        }

        float healthFloat = health;
        float maxHealthFloat = maxHealth;

        if(healthBar != null)
            healthBar.value = healthFloat / maxHealthFloat;

    }

    void Die()
    {
        Destroy(gameObject);
    }
}
