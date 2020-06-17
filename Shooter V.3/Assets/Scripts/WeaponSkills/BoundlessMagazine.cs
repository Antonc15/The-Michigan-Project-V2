using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundlessMagazine : MonoBehaviour
{

    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();
        gun.maxAmmo = Mathf.RoundToInt(gun.maxAmmo * 1.5f);
    }
}
