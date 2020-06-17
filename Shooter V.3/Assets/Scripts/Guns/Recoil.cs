using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Objects")]
    public Transform recoilObject;
    public Vector3 recoilRotation = new Vector3(2f, 2f, 2f);

    [Header("Settings")]
    public float rotationSpeed = 6;
    public float returnSpeed = 25;


    Vector3 currentRotation;
    Vector3 Rot;

    public bool fire;
    void FixedUpdate()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        Rot = Vector3.Slerp(Rot, currentRotation, rotationSpeed * Time.deltaTime);
        recoilObject.localRotation = Quaternion.Euler(Rot);
    }

    private void Update()
    {
        Fire();
    }

    public void Fire()
    {
        if (fire)
        {
            currentRotation += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
            fire = false;
        }
    }
}
