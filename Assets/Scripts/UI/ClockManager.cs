using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeClockText;
    GameManager gameManager;
    public int minuteTime;
    public int secondTime;
    public float currentTime;
   
    private void Update()
    {
        if (gameManager != null)
        {
            CalculateTime();
        }
            
    }
    private void Start()
    {
        StartCoroutine(DelayFindObject());
    }
    IEnumerator DelayFindObject()
    {
        yield return new WaitForSeconds(0.2f);
        gameManager = FindObjectOfType<GameManager>();
    }
    void ShowInClock()
    {
        minuteTime = Mathf.FloorToInt(currentTime / 60f);
        secondTime = Mathf.FloorToInt(currentTime % 60f);
        timeClockText.text = string.Format("{0:00}:{1:00}", minuteTime, secondTime);
        if(gameManager.state==GameState.WaitBeforeStart &&currentTime<=5)
        {
            timeClockText.color = Color.red;
        }
        else timeClockText.color = Color.black;

    }
    void CalculateTime()
    {
        currentTime = gameManager.currentTime;
        ShowInClock();
    }
}
