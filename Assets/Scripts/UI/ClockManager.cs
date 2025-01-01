using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeClockText;
    public GameManager gameManager;
    public int minuteTime;
    public int secondTime;
    public float currentTime;
    public Image clockImage;
    public Sprite[] clockSprites;
   
    private void Update()
    {
        if (gameManager != null)
        {
            CalculateTime();
        }
            
    }
    
    IEnumerator DelayFindObject()
    {
        yield return new WaitForSeconds(0.2f);
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnEnable()
    {
        StartCoroutine(DelayFindObject());
    }
    void ShowInClock()
    {
        minuteTime = Mathf.FloorToInt(currentTime / 60f);
        secondTime = Mathf.FloorToInt(currentTime % 60f);
        timeClockText.text = string.Format("{0:00}:{1:00}", minuteTime, secondTime);
        clockImage.sprite= clockSprites[gameManager.moonLightTime];
        if (gameManager.state==GameState.WaitBeforeStart &&currentTime<=5)
        {
            timeClockText.color = Color.red;
        }
        else timeClockText.color = Color.black;
        RenderSettings.skybox.SetColor("_SkyTint", gameManager.moonLightTime==0? Color.white:Color.black);
    }
    void CalculateTime()
    {
        if(gameManager.GetComponent<NetworkObject>().IsValid)
        {
            currentTime = gameManager.currentTime;
            ShowInClock();
        }
    }
}
