using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsistentBullets : MonoBehaviour
{

    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();

        gun.minDamage = gun.maxDamage;
    }
}
