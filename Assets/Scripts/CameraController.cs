using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject Player;
    public Transform child;
    private Controller RR;
    public float speed;
    [Range(0,2)]public float smootTime = 0;
    public float defouldFOV;
    public float desiredFov;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        child = Player.transform.Find("Camera Constains").transform;
        RR = Player.GetComponent<Controller>();
    }

    private void FixedUpdate()
    {
        Follow();
        boosFOV();
    }

    private void Follow()
    {
        if (speed <= 23)
        {
            speed = Mathf.Lerp(speed, RR.KPH / 3, Time.deltaTime);
        }
        else
        {
            speed = 23;
        }

        gameObject.transform.position = Vector3.Lerp(transform.position, child.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(Player.transform.transform.position);
    }
    private void boosFOV()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, desiredFov, Time.deltaTime* smootTime);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defouldFOV, Time.deltaTime * smootTime);
        }
    }
}
