using UnityEngine;

public class CarAudioManager : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;

    private Rigidbody carRb;
    private AudioSource carAudio;

    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;

    private Controller controller;
    void Start()
    {
        carAudio = GetComponent<AudioSource>();
        carRb = GetComponent<Rigidbody>();
        controller = GetComponent<Controller>();
    }

    void FixedUpdate()
    {
        EngineSound();
    }
    void EngineSound()
    {
        currentSpeed = controller.KPH;
        pitchFromCar = carRb.velocity.magnitude / 50f;

        if (currentSpeed < minSpeed)
        {
            carAudio.pitch = minPitch;
        }

        if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            carAudio.pitch = minPitch + pitchFromCar;
        }

        if (currentSpeed >= maxSpeed)
        {
            carAudio.pitch = maxPitch;
        }
    }

}
