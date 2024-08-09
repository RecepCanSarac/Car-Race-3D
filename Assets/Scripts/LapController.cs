using System.Collections;
using UnityEngine;

public class LapController : MonoBehaviour
{
    public int Lap;
    public float elapsedTime;
    public bool isRunning;
    public string Name;
    private void Start()
    {
        elapsedTime = 0f;
        isRunning = false;
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            if (isRunning)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
    }
}
