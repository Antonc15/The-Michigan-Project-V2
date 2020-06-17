using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aimingFov : MonoBehaviour
{
    [HideInInspector]
    public float aimFovSpeed;
    [HideInInspector]
    public float aimFovChange;
    public bool isAiming;

    float defaultFov;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        defaultFov = cam.fieldOfView;
    }

    void FixedUpdate()
    {
        AimFov();
    }

    void AimFov()
    {
        if (!isAiming)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFov, aimFovSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFov - aimFovChange, aimFovSpeed);
        }
    }
}
