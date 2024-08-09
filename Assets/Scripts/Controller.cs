using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Controller : MonoBehaviour
{
    internal enum driverType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }
    [SerializeField] private driverType drive;
    private InputManager IM;
    public AudioSource drift;
    public WheelCollider[] wheels = new WheelCollider[4];
    public GameObject[] whellMesh = new GameObject[4];
    public float[] slip = new float[4];
    private Transform centerOfMass;
    private Rigidbody rigidbody;
    public float breakPower;
    public float KPH;
    public float downForceValue = 50;
    public float radius = 6;
    public float torque = 200;
    public float steeringMax = 4f;
    public float thrust = 10000;
    public Transform hitPoint;
    public bool playPauseSmoke;
    void Start()
    {
        getObject();
    }

    private void getObject()
    {
        IM = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();
        centerOfMass = GameObject.Find("Mass").transform;
        rigidbody.centerOfMass = centerOfMass.transform.localPosition;
    }

    void FixedUpdate()
    {
        animateWheels();
        addDownForce();
        moveVehicle();
        steerVehicle();
        getFriction();
    }

    private void getFriction()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);
            slip[i] = wheelHit.forwardSlip;
        }
    }

    private void addDownForce()
    {
        rigidbody.AddForce(-transform.up * downForceValue * rigidbody.velocity.magnitude);
    }

    private void steerVehicle()
    {

        if (IM.horizontal > 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
        }
        else if (IM.horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }

    }

    private void moveVehicle()
    {
        float totalPower;
        if (drive == driverType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (torque / 4);
            }
        }
        else if (drive == driverType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (torque / 2);
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].motorTorque = IM.vertical * (torque / 2);
            }
        }

        KPH = rigidbody.velocity.magnitude * 3.6f;

        if (KPH > 15 && IM.handbrake == true)
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = breakPower;

            if (!playPauseSmoke)
            {
                drift.Play();
                playPauseSmoke = true; 
            }
        }
        else
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = 0;

            if (playPauseSmoke)
            {
                drift.Stop();
                playPauseSmoke = false; 
            }
        }

        if (IM.boosting)
        {
            rigidbody.AddForce(transform.forward * thrust);
        }
    }

    private void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            whellMesh[i].transform.position = wheelPosition;
            whellMesh[i].transform.rotation = wheelRotation;
        }
    }


}
