using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invisDeleter : MonoBehaviour
{

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("FloorPlacement") || col.gameObject.layer == LayerMask.NameToLayer("WallPlacement") || col.gameObject.layer == LayerMask.NameToLayer("RoofPlacement"))
        {
            Destroy(col.gameObject);
        }
    }

}
