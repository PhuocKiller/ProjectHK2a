using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeClockText;
    public int minuteTime;
    public int secondTime;
    public float currentTime;
    void CalculateTime()
    {
        if(FindObjectOfType<GameManager>()!=null)
        {
            currentTime = FindObjectOfType<GameManager>().currentTime;
            minuteTime = Mathf.FloorToInt(currentTime / 60f);
            secondTime = Mathf.FloorToInt(currentTime % 60f);
            timeClockText.text = string.Format("{0:00}:{1:00}", minuteTime, secondTime);
        }
        
    }
    private void Update()
    {
        CalculateTime();
    }

}
