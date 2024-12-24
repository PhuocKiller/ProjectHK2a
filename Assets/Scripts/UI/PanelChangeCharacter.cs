using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelChangeCharacter : MonoBehaviour
{
    public bool isChoseTeam, isChosePlayer;
    [SerializeField] Button darkTeamBtn, lightTeamBtn, fastGameBtn, networkBtn,
        darkNightBtn,dumbleBtn,ryanBtn,sagiBtn,teslaBtn,vikingBtn;
    private void OnEnable()
    {
        isChoseTeam = false; isChosePlayer = false;
        darkTeamBtn.GetComponent<Image>().color = Color.white;
        lightTeamBtn.GetComponent<Image>().color = Color.white;
        darkNightBtn.GetComponent<Image>().color = Color.white;
        dumbleBtn.GetComponent<Image>().color = Color.white;
        ryanBtn.GetComponent<Image>().color = Color.white;
        sagiBtn.GetComponent<Image>().color = Color.white;
        teslaBtn.GetComponent<Image>().color = Color.white;
        vikingBtn.GetComponent<Image>().color = Color.white;
    }
    private void OnDisable()
    {
        isChoseTeam = false; isChosePlayer = false;
    }
    public void DarkteamBtn()
    {
        darkTeamBtn.GetComponent<Image>().color=Color.green;
        lightTeamBtn.GetComponent<Image>().color = Color.white;
        isChoseTeam=true;
        if(isChosePlayer)
        {
            fastGameBtn.interactable = true;
            networkBtn.interactable = true;
        }
    }
    public void LightteamBtn()
    {
        darkTeamBtn.GetComponent<Image>().color = Color.white;
        lightTeamBtn.GetComponent<Image>().color = Color.green;
        isChoseTeam = true;
        if (isChosePlayer)
        {
            fastGameBtn.interactable = true;
            networkBtn.interactable = true;
        }
    }
    public void DarkNightBtn()
    {
        darkNightBtn.GetComponent<Image>().color = Color.green;
        dumbleBtn.GetComponent<Image>().color = Color.white;
        ryanBtn.GetComponent<Image>().color = Color.white;
        sagiBtn.GetComponent<Image>().color = Color.white;
        teslaBtn.GetComponent<Image>().color = Color.white;
        vikingBtn.GetComponent<Image>().color = Color.white;
        isChosePlayer = true;
        if (isChoseTeam)
        {
            fastGameBtn.interactable = true;
            networkBtn.interactable = true;
        }
    }
    public void DumbleBtn()
    {
        darkNightBtn.GetComponent<Image>().color = Color.white;
        dumbleBtn.GetComponent<Image>().color = Color.green;
        ryanBtn.GetComponent<Image>().color = Color.white;
        sagiBtn.GetComponent<Image>().color = Color.white;
        teslaBtn.GetComponent<Image>().color = Color.white;
        vikingBtn.GetComponent<Image>().color = Color.white;
        isChosePlayer = true;
        if (isChoseTeam)
        {
            fastGameBtn.interactable = true;
            networkBtn.interactable = true;
        }
    }
    public void RyanBtn()
    {
        darkNightBtn.GetComponent<Image>().color = Color.white;
        dumbleBtn.GetComponent<Image>().color = Color.white;
        ryanBtn.GetComponent<Image>().color = Color.green;
        sagiBtn.GetComponent<Image>().color = Color.white;
        teslaBtn.GetComponent<Image>().color = Color.white;
        vikingBtn.GetComponent<Image>().color = Color.white;
        isChosePlayer = true;
        if (isChoseTeam)
        {
            fastGameBtn.interactable = true;
            networkBtn.interactable = true;
        }
    }
    public void SagiBtn()
    {
        darkNightBtn.GetComponent<Image>().color = Color.white;
        dumbleBtn.GetComponent<Image>().color = Color.white;
        ryanBtn.GetComponent<Image>().color = Color.white;
        sagiBtn.GetComponent<Image>().color = Color.green;
        teslaBtn.GetComponent<Image>().color = Color.white;
        vikingBtn.GetComponent<Image>().color = Color.white;
        isChosePlayer = true;
        if (isChoseTeam)
        {
            fastGameBtn.interactable = true;
            networkBtn.interactable = true;
        }
    }
    public void TeslaBtn()
    {
        darkNightBtn.GetComponent<Image>().color = Color.white;
        dumbleBtn.GetComponent<Image>().color = Color.white;
        ryanBtn.GetComponent<Image>().color = Color.white;
        sagiBtn.GetComponent<Image>().color = Color.white;
        teslaBtn.GetComponent<Image>().color = Color.green;
        vikingBtn.GetComponent<Image>().color = Color.white;
        isChosePlayer = true;
        if (isChoseTeam)
        {
            fastGameBtn.interactable = true;
            networkBtn.interactable = true;
        }
    }
    public void VikingBtn()
    {
        darkNightBtn.GetComponent<Image>().color = Color.white;
        dumbleBtn.GetComponent<Image>().color = Color.white;
        ryanBtn.GetComponent<Image>().color = Color.white;
        sagiBtn.GetComponent<Image>().color = Color.white;
        teslaBtn.GetComponent<Image>().color = Color.white;
        vikingBtn.GetComponent<Image>().color = Color.green;
        isChosePlayer = true;
        if (isChoseTeam)
        {
            fastGameBtn.interactable = true;
            networkBtn.interactable = true;
        }
    }
}
