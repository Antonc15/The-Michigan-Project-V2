using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [Header("Objects")]
    public Camera cam;
    public GameObject crosshair;
    public LayerMask ignoreMe;

    [Header("Settings - Interact")]
    public float reach = 5f;
    [Range(0f, 100f)]
    public int localStrength = 100;
    public float throwStrength;

    [Header("Settings - Carry")]
    public float closestCarryDistance = 0.3f;
    public float carrySmooth = 4;
    public float distanceChangeIncrement = 0.1f;
    public float rotateIncrement = 0.1f;
    [Range(0f, 10f)]
    public float buttonHoldTime;
    [HideInInspector]
    public bool carrying;
    float distance;
    float currentHoldTime;

    GameObject carriedObject;
    LayerMask defaultLayer;

    void Update()
    {
        Interaction();

        if (!Input.GetButton("Interact"))
        {
            currentHoldTime = Time.time + buttonHoldTime;
        }

        if (carrying)
        {
            Carry(carriedObject);
            CheckDrop();
        }
    }

    void FixedUpdate()
    {
        if (carrying)
        {
            if (Input.GetButton("Fire1"))
            {
                carriedObject.transform.Rotate(0, rotateIncrement, 0, Space.World);
            }
            else if (Input.GetButton("Fire2"))
            {
                carriedObject.transform.Rotate(0, -rotateIncrement, 0, Space.World);
            }
        }
    }

    void Interaction()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.GetComponent<Camera>().transform.position, cam.GetComponent<Camera>().transform.forward, out hit, reach, ~ignoreMe))
        {
            Item item = hit.transform.GetComponent<Item>();

            if (!carrying)
            {
                    distance = hit.distance;
            }


            if (hit.transform.tag == "Interactable")
            {
                if(item != null)
                {
                    if(localStrength >= item.weight)
                    {
                        crosshair.GetComponent<Reticle>().lookingAtItem = true;
                        crosshair.GetComponent<Reticle>().waitTime = Time.time + 0.1f;
                    }

                    if (Input.GetButtonUp("Interact") && !carrying && item.canBeCollected)
                    {
                        item.isCollected = true;
                    }

                    if (Input.GetButton("Interact") && item.canBePickedUp && !carrying && Time.time > currentHoldTime && localStrength >= item.weight)
                    {
                        carrying = true;
                        carriedObject = item.gameObject;
                        defaultLayer = carriedObject.layer;
                        item.gameObject.transform.GetComponent<Rigidbody>().useGravity = false;
                        item.gameObject.transform.GetComponent<Rigidbody>().freezeRotation = true;
                    }
                }
            }
        }
    }
    
    void Carry(GameObject o)
    {
        o.transform.position = Vector3.Lerp(o.transform.position, cam.transform.position + cam.transform.forward * distance, Time.deltaTime * carrySmooth);

        if (o.layer != LayerMask.NameToLayer("CarriedItem"))
            o.layer = LayerMask.NameToLayer("CarriedItem");


        if (Input.GetAxis("Mouse ScrollWheel") > 0f & distance < reach)
        {
            distance += distanceChangeIncrement;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0f & distance > closestCarryDistance)
        {
           distance -= distanceChangeIncrement;
        }
    }

    void CheckDrop()
    {
        if (Input.GetButtonUp("Interact"))
        {
            DropObject();
        }
        else if (Input.GetButtonDown("Throw"))
        {
            if(carriedObject.GetComponent<Item>().weight <= localStrength / 2)
            {
                ThrowObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    void DropObject()
    {
        currentHoldTime = Time.time + buttonHoldTime;

        carrying = false;
        carriedObject.layer = defaultLayer;
        carriedObject.transform.GetComponent<Rigidbody>().useGravity = true;
        carriedObject.transform.GetComponent<Rigidbody>().freezeRotation = false;

        //removes carried object from script
        carriedObject = null;
    }

    void ThrowObject()
    {
        currentHoldTime = Time.time + buttonHoldTime;

        carrying = false;
        carriedObject.layer = defaultLayer;
        carriedObject.transform.GetComponent<Rigidbody>().useGravity = true;
        carriedObject.transform.GetComponent<Rigidbody>().freezeRotation = false;

        //throws the carried object
        carriedObject.transform.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * localStrength * throwStrength, transform.position);

        //removes carried object from script
        carriedObject = null;
    }
}
