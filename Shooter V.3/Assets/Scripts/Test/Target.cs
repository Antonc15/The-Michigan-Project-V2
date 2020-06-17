using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    public int health;
    public bool isHead;

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
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
