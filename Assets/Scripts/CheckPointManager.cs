using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public List<LapController> cars = new List<LapController>();
    public List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> NameText = new List<TextMeshProUGUI>();

    public TextMeshProUGUI LapText;
    public TextMeshProUGUI TotalLapText;

    public int TotalLap;
    public GameObject FinishPanel;
    public TextMeshProUGUI rankText;
    public trackWaypoints point;
    private void Start()
    {
        cars.Add(point.car);
        FinishPanel.SetActive(false);
        TotalLapText.text = TotalLap.ToString();
        for (int i = 0; i < cars.Count; i++)
        {
            NameText[i].text = cars[i].name;
        }
    }
    private void Update()
    {
        UpdateUI();
    }

    private void SortCars()
    {
        cars.Sort((x, y) =>
        {
            if (x.Lap == y.Lap)
            {
                return x.elapsedTime.CompareTo(y.elapsedTime);
            }
            return y.Lap.CompareTo(x.Lap);
        });
        cars.Reverse();
    }
    private void UpdateCarList()
    {
        cars.Sort((x, y) => y.elapsedTime.CompareTo(x.elapsedTime));

    }
    public void UpdateUI()
    {
        for (int i = 0; i < texts.Count; i++)
        {
            if (i < cars.Count)
            {
                NameText[i].text = (i + 1) + "." + cars[i].name;
                texts[i].text = cars[i].elapsedTime.ToString("F3");
            }
            else
            {
                texts[i].text = "";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car") || other.gameObject.CompareTag("Player"))
        {

            LapController car = other.GetComponent<LapController>();
            car.Lap++;
            car.isRunning = true;

            if (!cars.Contains(car))
            {
                cars.Add(car);
            }

            if (car.Lap > 1)
            {
                car.elapsedTime = 0f;
            }
            UpdateCarList();
            if (car.CompareTag("Player"))
            {
                LapText.text = other.GetComponent<LapController>().Lap.ToString();
                if (car.Lap > 1)
                {
                    car.elapsedTime = 0f;
                }
                if (car.Lap > TotalLap)
                {
                    int playerRank = cars.IndexOf(car) + 1;
                    rankText.text = "Tour Rank: " + playerRank.ToString();
                    FinishPanel.SetActive(true);
                }
                UpdateCarList();
                cars.Reverse();
                UpdateUI();
            }

        }
    }
}
