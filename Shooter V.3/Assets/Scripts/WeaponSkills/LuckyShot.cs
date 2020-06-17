using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyShot : MonoBehaviour
{

    RaycastGun gun;

    void Awake()
    {
        gun = GetComponent<RaycastGun>();

        gun.criticalStrikeChance = gun.criticalStrikeChance * 2;
    }
}
