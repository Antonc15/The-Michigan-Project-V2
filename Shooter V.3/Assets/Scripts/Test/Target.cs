using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    public float health;
    public bool isHead;

    public void TakeDamage(float amount)
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
            Debug.Log("Headshot " + amount * 2);
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
