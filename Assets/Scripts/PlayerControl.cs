﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public string leftXAxis;
    public string leftYAxis;
    public string rightXAxis;
    public string rightYAxis;
    public string fire1;
    public float bumpCoolDown;
    public float lastBumpTime;
    public Bump bump;

    private Vector3 turnVector;
    public float turnSpeed = 5.0f;
    private Vector2 RightStickInput;
    public float RightStickDeadzone = 0.02f;
    public float turnAxisThreshold = 0.1f;


    internal void SetAxisName(int id)
    {
        leftXAxis = "L_XAxis_" + id;
        leftYAxis = "L_YAxis_" + id;
        rightXAxis = "R_XAxis_" + id;
        rightYAxis = "R_YAxis_" + id;
        fire1 = "RB_" + id;
    }

    public float deadZone;
    private Rigidbody rb;
    public float acceleration;
    public float basicAcceleration;
    private float trueAcceleration;
    public float maxVelocity;
    public void SetBoost(bool input)
    {
        if(input)
        {
            if(!isBoosted)
            {
                acceleration += speedBoost;
                isBoosted = input;
            }
        }
        else
        {
            if(isBoosted)
            {
                acceleration -= speedBoost;
                isBoosted = false;
            }
        }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bump = GetComponentInChildren<Bump>();
        //float idealDrag = acceleration / maxVelocity;
        //rb.drag = idealDrag / (idealDrag * Time.fixedDeltaTime + 1);
        lastBumpTime = Time.time - bumpCoolDown - 1f;
        basicAcceleration = acceleration;
    }

    // Update is called once per frame
    //public bool hasInput = false;
    private void FixedUpdate()
    {
        if (isStunned && stunEndTime <= Time.time)
            isStunned = false;
        if (isStunned)
            return;
        trueAcceleration = acceleration / Time.fixedDeltaTime;
        //hasInput = false;
        Vector3 force = Vector3.zero;
        if (Input.GetAxis(leftXAxis) > deadZone)
        {
            //hasInput = true;
            force += Vector3.right * acceleration;
            //rb.velocity = rb.velocity + trueAcceleration * Vector3.right;
            //Debug.Log("Right");
        }
        if (Input.GetAxis(leftXAxis) < -deadZone)
        {
            //hasInput = true;
            force += Vector3.left * acceleration;
            //rb.velocity = rb.velocity + trueAcceleration * Vector3.left;
            //Debug.Log("Left");
        }
        if (Input.GetAxis(leftYAxis) > deadZone)
        {
            //hasInput = true;
            force += Vector3.forward * acceleration;
            //rb.velocity = rb.velocity + trueAcceleration * Vector3.forward;
            //Debug.Log("Up");
        }
        if (Input.GetAxis(leftYAxis) < -deadZone)
        {
            //hasInput = true;
            force += Vector3.back * acceleration;
            //rb.velocity = rb.velocity + trueAcceleration * Vector3.back;
            //Debug.Log("Down");
        }
        if (Input.GetButtonDown(fire1) && !bumpLock)
        {
            bumpaxis = Input.GetAxis(fire1);
            bumpLock = true;
            if (Time.time - lastBumpTime > bumpCoolDown)
            {
                lastBumpTime = Time.time;
                bump.TriggerBump();
            }
        }
        if (Input.GetButtonUp(fire1) && bumpLock)
            bumpLock = false;
        force += Vector3.ClampMagnitude(force, acceleration);
        rb.AddForce(force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        //if (!hasInput)
        //{
        //    rb.AddForce(Vector3.zero, ForceMode.VelocityChange);
        //}

        RightStickInput = new Vector2(Input.GetAxis(rightXAxis), Input.GetAxis(rightYAxis));
        if (RightStickInput.magnitude < RightStickDeadzone)
        {
            RightStickInput = Vector2.zero;
        }
        else
        {
            RightStickInput = RightStickInput.normalized * ((RightStickInput.magnitude - RightStickDeadzone) / (1 - RightStickDeadzone));
        }

        if (RightStickInput.magnitude >= turnAxisThreshold)
        {
            turnVector.y = Mathf.Atan2(RightStickInput.x, -RightStickInput.y) * Mathf.Rad2Deg;
            StartCoroutine(Turn());
        }
    }


    IEnumerator Turn()
    {

        Quaternion oldRotation = transform.rotation;
        Quaternion newRotation = new Quaternion();
        newRotation.eulerAngles = turnVector;

        for (float t = 0.0f; t < 1.0f; t += (turnSpeed * Time.deltaTime))
        {
            transform.rotation = Quaternion.Lerp(oldRotation, newRotation, t);
            yield return null;
        }

    }




public void Stun()
    {
        isStunned = true;
        stunEndTime = Time.time + stunLength;
    }

    public float bumpaxis;
    private bool bumpLock;
    private bool isStunned;
    private float stunEndTime;
    private float stunLength;
    public bool isBoosted;
    public float speedBoost;
}
