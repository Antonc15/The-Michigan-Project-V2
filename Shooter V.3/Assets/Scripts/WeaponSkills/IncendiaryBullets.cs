using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncendiaryBullets : MonoBehaviour
{
    RaycastGun gun;
    public GameObject explosion;
    void Awake()
    {
        gun = GetComponent<RaycastGun>();


        if (gun.explosion == null)
        {
            gun.explosion = explosion;
        }
    }
}
