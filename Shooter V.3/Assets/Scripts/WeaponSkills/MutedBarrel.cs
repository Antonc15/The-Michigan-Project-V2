using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutedBarrel : MonoBehaviour
{

    [Range(0f, 1f)]
    public float mutedVolume = 0.5f;
    [Range(0f, 3f)]
    public float mutedPitch = 1.2f;

    void Awake()
    {
        RaycastGun gun;
        gun = GetComponent<RaycastGun>();
        gun.shotVolume = mutedVolume;
        gun.shotPitch = mutedPitch;
    }
}