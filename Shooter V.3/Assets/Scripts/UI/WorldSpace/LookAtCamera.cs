using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    Transform camTransform;
    Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
        camTransform = GameObject.Find("Main Camera").transform;
        transform.rotation = camTransform.rotation * originalRotation;
    }

    void Update()
    {
        transform.rotation = camTransform.rotation * originalRotation;
    }
}
