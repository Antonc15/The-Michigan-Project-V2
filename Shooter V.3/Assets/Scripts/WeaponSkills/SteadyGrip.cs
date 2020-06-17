using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyGrip : MonoBehaviour
{

    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();
        gun.recoilRotation = gun.recoilRotation * 0.5f;
    }
}
