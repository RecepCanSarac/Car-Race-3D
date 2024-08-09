using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Controller RR;
    public GameObject needle;
    private float startPos = 220, endPos = -41;
    private float desiredPosition;

    public float vehicleSpeed;

    public GameObject pausePanel;
    void Start()
    {
        Time.timeScale = 1f;
        RR = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>();
    }

    void Update()
    {
        vehicleSpeed = RR.KPH;
    }
    public void PauseButton()
    {
        Time.timeScale = 0.0f;
        pausePanel.SetActive(true);
    }
    public void ResumePanel()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
    private void FixedUpdate()
    {
        updateNeedle();
    }
    public void updateNeedle()
    {
        desiredPosition = startPos - endPos;
        float temp = vehicleSpeed / 180;

        needle.transform.eulerAngles = new Vector3(0, 0, (startPos - temp * desiredPosition));
    }


    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu(int index)
    {
        SceneManager.LoadScene(index);
    }
}
