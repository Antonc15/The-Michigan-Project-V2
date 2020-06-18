using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    [Header("Object")]
    RectTransform crosshair;

    [Header("Settings")]
    public float restingSize;
    public float aimSize;
    public float interactSize;
    public float speed;

    [HideInInspector]
    public bool aiming;
    [HideInInspector]
    public bool lookingAtItem;
    [HideInInspector]
    public float waitTime;

    float currentSize;
    void Start()
    {
        crosshair = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (lookingAtItem)
        {
            if (waitTime < Time.time)
            {
                lookingAtItem = false;
            }
        }

        if(lookingAtItem)
        {
            currentSize = Mathf.Lerp(currentSize, interactSize, Time.deltaTime * speed);
        }
        else if(aiming)
        {
            currentSize = Mathf.Lerp(currentSize, aimSize, Time.deltaTime * speed);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        }

        crosshair.sizeDelta = new Vector2(currentSize, currentSize);
    }
}
