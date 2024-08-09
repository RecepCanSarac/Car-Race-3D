using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    public GameObject UIPanel;
    public GameObject CarSelectedPanel;

    public Camera Camera;
    public float moveDistance = 5f;
    private int currentCarIndex = 0;
    public List<GameObject> Cars = new List<GameObject>();
    public List<GameObject> CarsPrefab = new List<GameObject>();
    public int index = 0;
    public SOPlayer player;
    public float animationDuration = 0.5f;
    public GameObject MapSelectionPanel;
    public Button rightButton;
    public Button leftButton;
    private bool isAnimating = false;
    void OnEnable()
    {
        rightButton.interactable = true;
        leftButton.interactable = true;
        isAnimating = false;
        index = 0;
    }

    void Start()
    {
        Time.timeScale = 1f;
        isAnimating = false;
        index = 0;
        rightButton.onClick.AddListener(RightButton);
        leftButton.onClick.AddListener(LeftButton);
    }
    public void PlayButton()
    {
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, new Vector3(Camera.transform.position.x, Camera.transform.position.y, Camera.transform.position.z - 1), .01f);
        CarSelectedPanel.SetActive(true);
        UIPanel.SetActive(false);
    }
    public void BackPanel()
    {
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, new Vector3(Camera.transform.position.x, Camera.transform.position.y, Camera.transform.position.z + 1), .01f);
        CarSelectedPanel.SetActive(false);
        UIPanel.SetActive(true);
    }
    public void BackSelectCar()
    {
        CarSelectedPanel.SetActive(true);
        MapSelectionPanel.SetActive(false);
    }
    public void RightButton()
    {
        Debug.Log("Right Button Clicked");
        if (index < Cars.Count - 1 && !isAnimating)
        {
            Debug.Log("Moving Right");
            index++;
            StartCoroutine(MoveCars(-moveDistance));
        }
    }
    public void Map1()
    {
        player.Position = new Vector3(-112.8f, 0.14f, -31.5f);
        player.rotation = new Vector3(0, 90, 0);
        SceneManager.LoadScene(1);
    }
    public void Map2()
    {
        player.Position = new Vector3(56.29f, 21.23f, 138.76f);
        player.rotation = new Vector3(0, -79, 0.013f);
        SceneManager.LoadScene(2);
    }
    public void LeftButton()
    {
        Debug.Log("Left Button Clicked");
        if (index > 0 && !isAnimating)
        {
            Debug.Log("Moving Left");
            index--;
            StartCoroutine(MoveCars(moveDistance));
        }
    }
    public void StartGame()
    {
        if (Cars.Count > 0 && index < Cars.Count)
        {
            player.Car = CarsPrefab[index];
            MapSelectionPanel.SetActive(true);
            CarSelectedPanel.SetActive(false);
            index = 0;
            isAnimating = false;
        }
    }
    private IEnumerator MoveCars(float zMove)
    {
        isAnimating = true;
        rightButton.interactable = false;
        leftButton.interactable = false;
        float elapsedTime = 0f;
        List<Vector3> startPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();

        for (int i = 0; i < Cars.Count; i++)
        {
            Vector3 startPosition = Cars[i].transform.position;
            Vector3 targetPosition = startPosition + new Vector3(0, 0, zMove);
            startPositions.Add(startPosition);
            targetPositions.Add(targetPosition);
        }

        while (elapsedTime < animationDuration)
        {
            Debug.Log("Elapsed Time: " + elapsedTime);
            for (int i = 0; i < Cars.Count; i++)
            {
                Cars[i].transform.position = Vector3.Lerp(startPositions[i], targetPositions[i], elapsedTime / animationDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < Cars.Count; i++)
        {
            Cars[i].transform.position = targetPositions[i];
        }
        isAnimating = false;
        rightButton.interactable = true;
        leftButton.interactable = true;
    }

}
