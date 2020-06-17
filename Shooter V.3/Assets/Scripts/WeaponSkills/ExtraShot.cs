using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraShot : MonoBehaviour
{

    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();
        gun.bulletsPerShot = gun.bulletsPerShot + 1;
    }
}
