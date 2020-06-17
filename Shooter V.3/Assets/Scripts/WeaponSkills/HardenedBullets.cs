using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardenedBullets : MonoBehaviour
{

    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();

        gun.maxDamage = gun.maxDamage + 2;
        gun.minDamage = gun.minDamage + 2;
    }
}
