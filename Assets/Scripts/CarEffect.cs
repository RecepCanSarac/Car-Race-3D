
using UnityEngine;

public class CarEffect : MonoBehaviour
{
    public ParticleSystem smoke;
    public Transform[] smokeTransform;
    public Controller cont;

    private void Start()
    {
        cont = GetComponent<Controller>();
    }
    private void FixedUpdate()
    {

        if (cont.playPauseSmoke == true)
        {
            startSmoke();
        }

    }
    public void startSmoke()
    {
        ParticleSystem smokeIns = Instantiate(smoke, smokeTransform[0].position, Quaternion.identity);
        Invoke("destronEffect", 2f);

    }

    void destronEffect()
    {
        GameObject effect = GameObject.Find("msVFX_Stylized Smoke 4(Clone)");
        if (effect != null)
        {
            Destroy(effect);
        }
    }

}
