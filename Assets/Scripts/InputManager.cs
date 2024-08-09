using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum Driver
    {
        AI,
        Keyboard,
        Mobile
    }

    [SerializeField] private Driver driverController;
    public float vertical;
    public float verticalInput;
    public float horizontal;
    public bool handbrake;
    public bool boosting;
    public trackWaypoints waypoints;
    public Transform currentWaypoint;
    public List<Transform> node = new List<Transform>();
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 10)] public float sterrForce;

    private void Start()
    {
        waypoints = GameObject.FindGameObjectWithTag("path").GetComponent<trackWaypoints>();
        node = waypoints.nodes;
    }

    private void FixedUpdate()
    {
        switch (driverController)
        {
            case Driver.AI:
                CalculateDistanceOfWaypoints();
                AIDriver();
                break;
            case Driver.Keyboard:
                KeyboardDriver();
                break;
            case Driver.Mobile:
                MobileDriver();
                break;
            default:
                break;
        }
    }

    private void AIDriver()
    {
        vertical = verticalInput;
        AISteer();

        if (currentWaypoint.name == "BigPoint")
        {
            vertical = -verticalInput*2.8f;
        }
    }

    private void KeyboardDriver()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        handbrake = Input.GetAxis("Jump") != 0;
        boosting = Input.GetKey(KeyCode.LeftShift);
    }

    private void MobileDriver()
    {
        // Mobil sürücü için kod buraya eklenecek
    }

    private void CalculateDistanceOfWaypoints()
    {
        Vector3 position = transform.position;
        float distance = Mathf.Infinity;

        for (int i = 0; i < node.Count; i++)
        {
            Vector3 difference = node[i].transform.position - position;
            float currentDistance = difference.magnitude;
            if (currentDistance < distance)
            {
                int index = i + distanceOffset;
                if (index >= node.Count)
                {
                    index = index % node.Count; // Listenin sonuna gelirse baþa dön
                }
                currentWaypoint = node[index];
                distance = currentDistance;
            }
        }

        if (Vector3.Distance(transform.position, currentWaypoint.position) < 2f)
        {
            int currentIndex = node.IndexOf(currentWaypoint);
            currentWaypoint = node[(currentIndex + 1) % node.Count];
        }
    }

    private void AISteer()
    {
        Vector3 relative = transform.InverseTransformPoint(currentWaypoint.position);
        float distanceToWaypoint = relative.magnitude;
        relative /= distanceToWaypoint;

        float slowDownFactor = Mathf.Clamp(distanceToWaypoint / 2, 0.1f, 1f);

        horizontal = (relative.x / relative.magnitude) * sterrForce * slowDownFactor;
    }
   
}
