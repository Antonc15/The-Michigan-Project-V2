using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyKick : MonoBehaviour
{

    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();
        gun.knockbackForce = gun.knockbackForce * 1.5f;
    }
}
