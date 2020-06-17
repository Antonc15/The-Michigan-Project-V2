using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invisDeleter : MonoBehaviour
{

    void OnTriggerStay(Collider col)
    {
        //checks for invis objects touching it.
        if (col.gameObject.layer == LayerMask.NameToLayer("FloorPlacement") || col.gameObject.layer == LayerMask.NameToLayer("WallPlacement") || col.gameObject.layer == LayerMask.NameToLayer("RoofPlacement"))
        {
            //deletes selcted gameobject.
            Destroy(col.gameObject);
        }
    }

}
