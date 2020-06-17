using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreciseBarrel : MonoBehaviour
{

    RaycastGun gun;
    void Awake()
    {
        gun = GetComponent<RaycastGun>();
        gun.bloom = gun.bloom * 0.5f;
    }
}
