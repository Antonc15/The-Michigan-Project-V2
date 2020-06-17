using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticReceiver : MonoBehaviour
{
    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();


        if (!gun.isBurstFire)
        {
            gun.isAutoFire = true;
        }
        else
        {
            gun.isBurstFire = false;
            gun.isAutoFire = true;
            gun.shotDelay = gun.delayBetweenBursts;
        }
    }
}
