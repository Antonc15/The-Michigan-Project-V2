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
    public Animator anim;

    [Header("Settings")]
    public float groundCheckerDistance = 0.5f;
    public float groundDistance = 0.4f;

    [Header("Settings - Gravity")]
    public float terminalVelocity = 20f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    [Header("Settings - Modes")]
    public float crouchHeight = 1.5f;
    public float crouchSpeed = 1f;
    public float walkSpeed = 3f;
    public float sprintSpeed = 5f;
    [HideInInspector]
    public bool isSprinting;

    Vector3 velocity;
    float currentSpeed;
    float normalHeight = 2f;
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
        if (isGrounded)
        {
            if (Input.GetButton("Crouch"))
            {
                groundCheck.position = new Vector3(controller.transform.position.x, controller.transform.position.y - 0.25f, controller.transform.position.z);
                currentSpeed = crouchSpeed;
                charController.height = crouchHeight;
                isSprinting = false;
            }
            else if (Input.GetButton("Sprint") && Input.GetButton("Horizontal") || Input.GetButton("Sprint") && Input.GetButton("Vertical"))
            {
                currentSpeed = sprintSpeed;

                groundCheck.position = new Vector3(controller.transform.position.x, controller.transform.position.y - groundCheckerDistance, controller.transform.position.z);
                charController.height = normalHeight;

                isSprinting = true;
            }
            else
            {
                groundCheck.position = new Vector3(controller.transform.position.x, controller.transform.position.y - groundCheckerDistance, controller.transform.position.z);
                currentSpeed = walkSpeed;
                charController.height = normalHeight;
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            //this does the jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (anim != null)
        {
            if (Mathf.Abs(z) > 0)
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }
        }

            if (velocity.y > -terminalVelocity)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}
