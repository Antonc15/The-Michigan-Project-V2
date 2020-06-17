using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleReceiver : MonoBehaviour
{
    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();

        gun.isBurstFire = true;
        gun.burstAmount = 2;
        gun.delayBetweenBursts = 0.1f;
        gun.shotDelay = gun.shotDelay + 0.2f;

    }
}
