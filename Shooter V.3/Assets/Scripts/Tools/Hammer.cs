using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hammer : MonoBehaviour
{
    [Header("Objects")]
    public GameObject iconHolder;

    public GameObject[] floor;
    public GameObject[] wall;
    public GameObject[] roof;

    [Header("Weapon Sway")]
    public float swayAmount = 0.06f;
    public float smoothAmount = 6;
    public float maxSwayAmount = 0.12f;

    [Header("Settings")]
    public float reach = 10;
    public float rotateIncrement = 0.1f;
    public LayerMask ignoreMe;

    [Header("Audio")]
    public GameObject woodPlaceAudio;

    [Header("UI")]
    public Image[] icons;

    [Header("Placeholder")]
    public Transform[] placeHolders;

    Vector3 initialPosition;
    aimingFov aimingFov;
    Transform currentPlaceHolder;
    WeaponSwitch weaponSwitcher;
    Interact playerInteract;

    bool inBuildMode;
    bool freeBuildMode;
    bool snapped;
    int currentType;
    int buildingAmount = 0;


    private void Start()
    {
        AssignLayers();

        playerInteract = GameObject.Find("Player").GetComponent<Interact>();

        aimingFov = GameObject.Find("Main Camera").GetComponent<aimingFov>();

        weaponSwitcher = GameObject.Find("Weapons").GetComponent<WeaponSwitch>();

        initialPosition = transform.localPosition;

        currentPlaceHolder = placeHolders[0];

        currentType = 1;

        buildingAmount = 3;
    }

    void OnEnable()
    {
        if(aimingFov == null)
        {
            aimingFov = GameObject.Find("Main Camera").GetComponent<aimingFov>();
        }

        if(aimingFov.isAiming)
            aimingFov.isAiming = false;
    }

    void OnDisable()
    {
            if (iconHolder.activeSelf)
                iconHolder.SetActive(false);

        if (inBuildMode)
        {
            inBuildMode = false;
            weaponSwitcher.canSwitch = true;
            freeBuildMode = true;

            //makes the placholder invisible on disable
            if (currentPlaceHolder != null && currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
            {
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    void Update()
    {
        ToolSway();
        CheckForBuild();

        if (inBuildMode)
        {
            if (!iconHolder.activeSelf)
                iconHolder.SetActive(true);

            if (weaponSwitcher.canSwitch)
                weaponSwitcher.canSwitch = false;

            PlaceholderLocation();
            BuildingType();
        }
        else if (!inBuildMode)
        {
            if (iconHolder.activeSelf)
                iconHolder.SetActive(false);

            weaponSwitcher.canSwitch = true;
        }
    }

    void FixedUpdate()
    {
        if (!snapped & inBuildMode)
        {
            if (Input.GetButton("Rotate"))
            {
                currentPlaceHolder.transform.Rotate(0, rotateIncrement, 0, Space.World);
            }
        }    
    }

    //**********************************************************************************\\
    //                                                                                  \\
    //                     Below are functions I have written.                          \\
    //                                                                                  \\
    //**********************************************************************************\\

    //as name suggests, adds weapon sway, copy / pasted from Raycast Gun script
    void BuildingType()
    {
        int previousType = currentType;
        //**********************************************************************************\\
        //                                                                                  \\
        //                This Detects whether the mouse wheel is rolled                    \\
        //                                                                                  \\
        //**********************************************************************************\\

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if(currentType < buildingAmount)
            {
                currentType++;
            }
            else
            {
                currentType = 1;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(currentType > 1)
            {
                currentType--;
            }
            else
            {
                currentType = buildingAmount;
            }
        }

        //**********************************************************************************\\
        //                                                                                  \\
        //               This assigns building pieces to numbers: 1 - (buildingAmount)      \\
        //                Also it changes colors of the icons to display selected.          \\
        //                                                                                  \\
        //**********************************************************************************\\

        if(previousType != currentType)
        {
            AssignLayers();

            for (int i = 0; i < buildingAmount; i++)
            {
                Debug.Log("s");
                if (i == (currentType - 1))
                {
                    icons[i].color = new Color32(122, 255, 127, 160);

                    if (!placeHolders[i].GetComponent<MeshRenderer>().enabled)
                        placeHolders[i].GetComponent<MeshRenderer>().enabled = true;

                    currentPlaceHolder = placeHolders[i];
                }
                else
                {
                    icons[i].color = new Color32(255, 255, 225, 160);

                    if (placeHolders[i].GetComponent<MeshRenderer>().enabled)
                        placeHolders[i].GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }

    void ToolSway()
    {
        float movementX = -Input.GetAxis("Mouse X") * swayAmount;
        float movementY = -Input.GetAxis("Mouse Y") * swayAmount;

        movementX = Mathf.Clamp(movementX, -maxSwayAmount, maxSwayAmount);
        movementY = Mathf.Clamp(movementY, -maxSwayAmount, maxSwayAmount);


        Vector3 finalPosition = new Vector3(movementX, movementY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }

    //this checks whether the right mouse button is clicked, and 
    void CheckForBuild()
    {
        if (!playerInteract.carrying)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                inBuildMode = !inBuildMode;
            }

            if (Input.GetButtonDown("Fire1") && inBuildMode)
            {
                freeBuildMode = !freeBuildMode;
            }
        }
        else 
        {
            if(inBuildMode)
                inBuildMode = false;
        }

        if (!inBuildMode)
        {
            //disables free build mode
            if(!freeBuildMode)
               freeBuildMode = true;

            //makes the placeholder invisible
            if (currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void PlaceholderLocation()
    {
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.GetComponent<Camera>().transform.forward, out hit, reach, ~ignoreMe))
        {
            if (freeBuildMode)
            {
                //enables the mesh rendere when raycast hits, making it visible
                if (!currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                    currentPlaceHolder.GetComponent<MeshRenderer>().enabled = true;
            }

            //This checks for invisible placeholder blocks, basicvally in charge of smooth snapping

            if (currentType == 1)
            {
                FloorSnapper(hit);
            }
            else if (currentType == 2)
            {
                WallSnapper(hit);
            }
            else
            {
                RoofSnapper(hit);
            }

            Build(hit);
        }
        else
        {
            //disables the mesh renderer when raycast doesnt hit, making the placeholder invisible
            if (currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void Build(RaycastHit hit)
    {
        //******************************************************************\\
        //                                                                  \\
        //                       Floor Building                             \\
        //                                                                  \\
        //******************************************************************\\

        //this handles the placing of objects
        if (Input.GetButtonDown("Interact") & currentType == 1)
        {

            Debug.Log("flor");

            if (freeBuildMode)
            {
                //spawning the building
                var myObject = Instantiate(floor[0], currentPlaceHolder.position, currentPlaceHolder.rotation);
                myObject.transform.parent = GameObject.Find("BuildingHolder").transform;

                //spawning the sound
                if (woodPlaceAudio != null)
                {
                    var audio = Instantiate(woodPlaceAudio, hit.transform.position, Quaternion.identity);
                    audio.transform.parent = GameObject.Find("AudioHolder").transform;
                }
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("FloorPlacement"))
            {

                //spawning the building
                var myObject = Instantiate(floor[0], hit.transform.position, Quaternion.identity);
                myObject.transform.parent = GameObject.Find("BuildingHolder").transform;

                //spawning the sound
                if (woodPlaceAudio != null)
                {
                    var audio = Instantiate(woodPlaceAudio, hit.transform.position, Quaternion.identity);
                    audio.transform.parent = GameObject.Find("AudioHolder").transform;
                }

                Destroy(hit.transform.gameObject);
            }
        }

        //******************************************************************\\
        //                                                                  \\
        //                       Wall Building                              \\
        //                                                                  \\
        //******************************************************************\\

        //this handles the placing of objects
        if (Input.GetButtonDown("Interact") & currentType == 2)
        {

            Debug.Log("wall");

            if (freeBuildMode)
            {
                //spawning the building
                var myObject = Instantiate(wall[0], currentPlaceHolder.position, currentPlaceHolder.rotation);
                myObject.transform.parent = GameObject.Find("BuildingHolder").transform;

                //spawning the sound
                if (woodPlaceAudio != null)
                {
                    var audio = Instantiate(woodPlaceAudio, hit.transform.position, Quaternion.identity);
                    audio.transform.parent = GameObject.Find("AudioHolder").transform;
                }
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("WallPlacement"))
            {

                //spawning the building
                var myObject = Instantiate(wall[0], hit.transform.position, hit.transform.rotation);
                myObject.transform.parent = GameObject.Find("BuildingHolder").transform;

                //spawning the sound
                if (woodPlaceAudio != null)
                {
                    var audio = Instantiate(woodPlaceAudio, hit.transform.position, Quaternion.identity);
                    audio.transform.parent = GameObject.Find("AudioHolder").transform;
                }

                Destroy(hit.transform.gameObject);
            }
        }

        //******************************************************************\\
        //                                                                  \\
        //                       Roof Building                              \\
        //                                                                  \\
        //******************************************************************\\

        //this handles the placing of objects
        if (Input.GetButtonDown("Interact") & currentType == 3)
        {

            Debug.Log("roof");

            if (freeBuildMode)
            {
                //spawning the building
                var myObject = Instantiate(roof[0], currentPlaceHolder.position, Quaternion.identity);
                myObject.transform.parent = GameObject.Find("BuildingHolder").transform;

                //spawning the sound
                if (woodPlaceAudio != null)
                {
                    var audio = Instantiate(woodPlaceAudio, hit.transform.position, Quaternion.identity);
                    audio.transform.parent = GameObject.Find("AudioHolder").transform;
                }
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("RoofPlacement"))
            {

                //spawning the building
                var myObject = Instantiate(roof[0], hit.transform.position, Quaternion.identity);
                myObject.transform.parent = GameObject.Find("BuildingHolder").transform;

                //spawning the sound
                if (woodPlaceAudio != null)
                {
                    var audio = Instantiate(woodPlaceAudio, hit.transform.position, Quaternion.identity);
                    audio.transform.parent = GameObject.Find("AudioHolder").transform;
                }

                Destroy(hit.transform.gameObject);
            }
        }


        //******************************************************************\\
        //                                                                  \\
        //                       Destroy Building                           \\
        //                                                                  \\
        //******************************************************************\\
        if (Input.GetButtonDown("Destroy"))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlacedBlock"))
            {
                //spawning the sound
                if (woodPlaceAudio != null)
                {
                    var audio = Instantiate(woodPlaceAudio, hit.transform.position, Quaternion.identity);
                    audio.transform.parent = GameObject.Find("AudioHolder").transform;
                }

                Destroy(hit.transform.gameObject);
            }
        }
    }

    void AssignLayers()
    {
        ignoreMe = 0;

        if (currentType == 3)
        {
            //roof layer
            ignoreMe |= (1 << LayerMask.NameToLayer("WallPlacement"));
            ignoreMe |= (1 << LayerMask.NameToLayer("FloorPlacement"));
            ignoreMe |= (1 << LayerMask.NameToLayer("Placeholder"));
        }
        else if (currentType == 2)
        {
            //wall layer
            ignoreMe |= (1 << LayerMask.NameToLayer("FloorPlacement"));
            ignoreMe |= (1 << LayerMask.NameToLayer("RoofPlacement"));
            ignoreMe |= (1 << LayerMask.NameToLayer("Placeholder"));
        }
        else
        {
            //floor layer
            ignoreMe |= (1 << LayerMask.NameToLayer("WallPlacement"));
            ignoreMe |= (1 << LayerMask.NameToLayer("RoofPlacement"));
            ignoreMe |= (1 << LayerMask.NameToLayer("Placeholder"));
        }
    }

    void FloorSnapper(RaycastHit hit)
    {
        
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("FloorPlacement"))
        {
            snapped = true;

            currentPlaceHolder.position = hit.transform.position;
            currentPlaceHolder.rotation = hit.transform.rotation;

            //enables the mesh rendere when raycast hits, making it visible
            if (!currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (freeBuildMode)
        {
            snapped = false;
            currentPlaceHolder.position = hit.point;
        }
        else
        {
            //disables the mesh renderer when raycast doesnt hit, making the placeholder invisible
            if (currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void WallSnapper(RaycastHit hit)
    {

        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("WallPlacement"))
        {
            snapped = true;

            currentPlaceHolder.position = hit.transform.position;
            currentPlaceHolder.rotation = hit.transform.rotation;

            //enables the mesh rendere when raycast hits, making it visible
            if (!currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (freeBuildMode)
        {
            snapped = false;
            currentPlaceHolder.position = hit.point;
        }
        else
        {
            //disables the mesh renderer when raycast doesnt hit, making the placeholder invisible
            if (currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void RoofSnapper(RaycastHit hit)
    {

        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("RoofPlacement"))
        {
            snapped = true;

            currentPlaceHolder.position = hit.transform.position;

            //enables the mesh rendere when raycast hits, making it visible
            if (!currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (freeBuildMode)
        {
            snapped = false;
            currentPlaceHolder.position = hit.point;
        }
        else
        {
            //disables the mesh renderer when raycast doesnt hit, making the placeholder invisible
            if (currentPlaceHolder.GetComponent<MeshRenderer>().enabled)
                currentPlaceHolder.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
