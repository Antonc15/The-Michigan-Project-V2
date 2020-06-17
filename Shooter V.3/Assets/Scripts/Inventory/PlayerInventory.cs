using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    private Inventory inventory;
    [SerializeField] private UI_Inventory uiInventory;

    void Awake()
    {
        inventory = new Inventory();
        uiInventory.SetInventory(inventory);
    }
}
