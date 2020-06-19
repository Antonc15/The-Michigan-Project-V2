using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAll : MonoBehaviour
{

    public float reach;
    public int damage;
    public int pierceAmount;
    public LayerMask targets;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position, transform.forward, reach);

            for (int i = 0; i < hits.Length; i++)
            {
                if (i < pierceAmount)
                {
                    RaycastHit hit = hits[i];

                    if (hit.transform.GetComponent<Target>())
                    {
                        Target target = hit.transform.GetComponent<Target>();
                        target.TakeDamage(damage);
                    }
                }
            }

        }

    }
}
