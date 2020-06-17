using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleReceiver : MonoBehaviour
{
    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();

        gun.isBurstFire = true;
        gun.burstAmount = 3;
        gun.delayBetweenBursts = 0.1f;
        gun.shotDelay = gun.shotDelay + 0.3f;

    }
}
