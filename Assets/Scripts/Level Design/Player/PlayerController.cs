﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* Player */
    private CharacterController controller;
    public static event FreezableHandler ObjectFreezed;

    /* Movement */
    private float horizontalMovement, verticalMovement;
    public float playerSpeed = 12f;
    public float jumpHeight = 3f;
    bool doubleJump = false;

    /* Gravity */
    public float gravity = -9.81f;
    public float groundDistance = 0.2f;
    public Transform groundCheck;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;

    /* Freezable */
    private Collider[] freezables;
    float minDist = Mathf.Infinity;
    public LayerMask freezeMask;
    public float freezeRange = 100f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();

        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (!doubleJump)
            {
                doubleJump = true;
                Jump();
            }
        }

        MovePlayer();
        Gravity();

        // Freeze Command
        if (Input.GetKeyDown("e"))
        {
            Freeze(ReturnCloser());
        }
    }

    private void Freeze(GameObject box)
    {
        if (box)
        {
            FreezableObject controller = box.GetComponent<FreezableObject>();

            if (controller.movementEnabled)
            {
                controller.movementEnabled = false;
            }
            else if (!controller.movementEnabled)
            {
                controller.movementEnabled = true;
            }
        }
    }
    
    private GameObject ReturnCloser()
    {
        freezables = Physics.OverlapSphere(transform.position, freezeRange, freezeMask);
        GameObject tMin = null;

        if (freezables.Length > 0)
        {
            foreach (Collider freezable in freezables)
            {
                float dist = Vector3.Distance(transform.position, freezable.transform.position);
                if (dist < minDist)
                {
                    tMin = freezable.gameObject;
                    minDist = dist;
                }
            }
        }

        return tMin;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -5f;
            doubleJump = false;
        }
    }

    private void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void MovePlayer()
    {
        Vector3 move = transform.right * horizontalMovement + transform.forward * verticalMovement;

        controller.Move(move * playerSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
