using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public float maxDamageMultiplier;
    public float liveTime;
    public Vector3 minSize = new Vector3(0, 0, 0);
    public Vector3 maxSize = new Vector3(1, 1, 1);

    [HideInInspector]
    public int damage;
    float currentLiveTime;
    float initialTime;
    bool hitObject = false;
    int minDamage;
    int maxDamage;
    bool isCritical;
    float damageOut;

    void Awake()
    {
        initialTime = Time.time;
        currentLiveTime = Time.time + liveTime;
        gameObject.transform.localScale = minSize;
    }

    void FixedUpdate()
    {
        if (currentLiveTime <= Time.time)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.transform.localScale = Vector3.Lerp(minSize, maxSize, (Time.time - initialTime) / liveTime);
            damageOut = damage * maxDamageMultiplier * (1 - (Time.time - initialTime) / liveTime);
        }
    }

    public void AssignValues(int minDam, int maxDam, bool isCrit)
    {
        minDamage = minDam;
        maxDamage = maxDam;
        isCritical = isCrit;
    }

    void OnTriggerEnter(Collider otherCol)
    {
        if (otherCol.gameObject.GetComponent<Target>())
        {
            hitObject = true;

            if (damageOut < minDamage)
            {
                otherCol.gameObject.GetComponent<Target>().AssignColor(damageOut, maxDamage * maxDamageMultiplier, isCritical);
            }
            else
            {
                otherCol.gameObject.GetComponent<Target>().AssignColor(minDamage, maxDamage * maxDamageMultiplier, isCritical);
            }

            otherCol.gameObject.GetComponent<Target>().HitPosition(transform.position);

            otherCol.gameObject.GetComponent<Target>().TakeDamage(Mathf.FloorToInt(damageOut) + 1);
        }

        if (otherCol.GetComponent<PlayerVitals>())
        {
            otherCol.GetComponent<PlayerVitals>().TakeDamage(Mathf.FloorToInt(damageOut) + 1);
        }
    }
}
