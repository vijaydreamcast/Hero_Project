using System.Collections;
using TMPro;
using UnityEngine;

public class SpeedAndDistancePanel : MonoBehaviour
{
    public BikeDataSO bikeData;

    public TMP_Text speedText;
    public TMP_Text timeText;
    public TMP_Text distanceText;

    private Coroutine timeCoroutine;

    private void Start()
    {
        timeCoroutine = StartCoroutine(UpdateTimeCoroutine());
    }

    private void Update()
    {
        speedText.text = ((int)bikeData.currentSpeed).ToString("00");
        distanceText.text = bikeData.currentDistance.ToString("F2") + "Km";
    }

    private IEnumerator UpdateTimeCoroutine()
    {
        while (true)
        {
            // Display totalTime as a clock (HH:MM) in 24-hour format with AM/PM
            int totalSeconds = Mathf.FloorToInt(bikeData.totalTime);
            int hours = (totalSeconds / 3600) % 24;
            int minutes = (totalSeconds % 3600) / 60;

            string ampm = hours < 12 ? "AM" : "PM";
            int displayHour = hours == 0 ? 12 : (hours > 12 ? hours - 12 : hours);

            // 24-hour format with AM/PM
            timeText.text = $"{hours:D2}:{minutes:D2} {ampm}";

            yield return new WaitForSeconds(1f);
        }
    }
}
