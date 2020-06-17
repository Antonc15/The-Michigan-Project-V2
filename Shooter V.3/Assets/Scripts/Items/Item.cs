using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector]
    public bool isCollected;

    [Range(0f, 100f)]
    public float weight = 100;

    public bool canBeCollected = true;
    public bool canBePickedUp = true;

    public enum ItemType
    {
        Pistol,
        Shotgun,
    }

    public ItemType itemType;
    public int amount;

    void Update()
    {
        if (isCollected)
        {
            Destroy(gameObject);
        }    
    }
}
