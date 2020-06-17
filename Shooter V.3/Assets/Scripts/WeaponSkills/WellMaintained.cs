using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellMaintained : MonoBehaviour
{

    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();
        gun.reloadTime = gun.reloadTime * 0.75f;
    }
}
