using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [HideInInspector]
    public int damage;

    void OnTriggerEnter(Collider otherCol)
    {
        if (otherCol.GetComponent<Target>())
        {
            Debug.Log(damage);
            otherCol.GetComponent<Target>().TakeDamage(Mathf.RoundToInt(damage * 0.5f));
        }

        if (otherCol.GetComponent<PlayerVitals>())
        {
            otherCol.GetComponent<PlayerVitals>().TakeDamage(Mathf.RoundToInt((damage * 0.5f)));
        }
    }
}
