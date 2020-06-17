using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{

    public float liveTime = 0.5f;
    float currentLiveTime;

    void Awake()
    {
        currentLiveTime = Time.time + liveTime;
    }

    void Update()
    {
        if (currentLiveTime <= Time.time)
        {
            Destroy(gameObject);
        }
    }
}
