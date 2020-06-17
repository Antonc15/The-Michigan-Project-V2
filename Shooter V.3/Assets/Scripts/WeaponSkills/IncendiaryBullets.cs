using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncendiaryBullets : MonoBehaviour
{
    RaycastGun gun;
    public GameObject explosion;
    public GameObject explosionDecal;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();


        if (gun.explosion == null)
        {
            gun.explosion = explosion;
        }

        if (gun.incendiaryBulletHole == null)
        {
            gun.incendiaryBulletHole = explosionDecal;
        }
    }
}
