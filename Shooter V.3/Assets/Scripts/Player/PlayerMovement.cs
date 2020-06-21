using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Objects")]
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("Settings")]
    public float groundCheckerDistance = 0.5f;
    public float groundDistance = 0.4f;

    [Header("Settings - Gravity")]
    public float takeDamageVelocity = 12f;
    public float terminalVelocity = 20f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    [Header("Settings - Modes")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 5f;
    [HideInInspector]
    public bool isSprinting;
    [HideInInspector]
    public bool canAim;

    Vector3 velocity;
    float currentSpeed;
    float normalHeight = 2f;
    bool takeDamageOnLand;
    float takeDamage;
    float timeInAir;
    [HideInInspector]
    public bool isGrounded;

    CharacterController charController;

    void Start()
    {
        charController = gameObject.GetComponent<CharacterController>();
        normalHeight = charController.height;
    }

    void Update()
    {
        Movement();
        FallDamage();
    }

    //********** This handles movement, falling *********\\
    void Movement()
    {
        if (isGrounded)
        {
            if (Input.GetButton("Sprint") && Input.GetButton("Horizontal") || Input.GetButton("Sprint") && Input.GetButton("Vertical"))
            {
                currentSpeed = sprintSpeed;

                groundCheck.position = new Vector3(controller.transform.position.x, controller.transform.position.y - groundCheckerDistance, controller.transform.position.z);
                charController.height = normalHeight;

                isSprinting = true;
                canAim = false;
            }
            else
            {
                groundCheck.position = new Vector3(controller.transform.position.x, controller.transform.position.y - groundCheckerDistance, controller.transform.position.z);
                currentSpeed = walkSpeed;
                charController.height = normalHeight;
                isSprinting = false;
                canAim = true;
            }
        }
        else
        {
            canAim = true;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //this does the jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        if (velocity.y > -terminalVelocity)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    //********** This handles fall damage *********\\
    void FallDamage()
    {
        if (!isGrounded)
        {
            timeInAir += 1 * Time.deltaTime;
            takeDamage = velocity.y * timeInAir * 2f;

            if (velocity.y < takeDamageVelocity)
            {
                takeDamageOnLand = true;
            }
            else
            {
                takeDamageOnLand = false;
            }
        }
        else
        {
            if(timeInAir != 0)
                timeInAir = 0;

            if (takeDamageOnLand)
            {
                takeDamageOnLand = false;

                GetComponent<PlayerVitals>().TakeDamage(Mathf.RoundToInt(Mathf.Abs(takeDamage)));
            }
        }
    }
}
