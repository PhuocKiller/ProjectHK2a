using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelChangeCharacter : MonoBehaviour
{
    public bool isChoseTeam, isChosePlayer;
    [SerializeField] Button darkTeamBtn, lightTeamBtn, fastGameBtn, networkBtn,
        darkNightBtn,dumbleBtn,ryanBtn,sagiBtn,teslaBtn,vikingBtn, characBtn;
    [SerializeField] GameObject passwordConfirmPanel;
    [SerializeField] TMP_InputField roomNameField, passwordField;
    string passwordToConfirm;
    private void OnEnable()
    {
        isChoseTeam = false; isChosePlayer = false;
        fastGameBtn.interactable = false;
        networkBtn.interactable = false;
        ButtonBackWhite();
        HideAllMenuCharac();
    }
    void ButtonBackWhite()
    {
        darkTeamBtn.GetComponent<Image>().color = Color.white;
        lightTeamBtn.GetComponent<Image>().color = Color.white;
        darkNightBtn.GetComponent<Image>().color = Color.white;
        dumbleBtn.GetComponent<Image>().color = Color.white;
        ryanBtn.GetComponent<Image>().color = Color.white;
        sagiBtn.GetComponent<Image>().color = Color.white;
        teslaBtn.GetComponent<Image>().color = Color.white;
        vikingBtn.GetComponent<Image>().color = Color.white;
    }
    void HideAllMenuCharac()
    {
        darkNightBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        dumbleBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        ryanBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        sagiBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        teslaBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        vikingBtn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        isChoseTeam = false; isChosePlayer = false;
    }
    #region playerButton
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
    #endregion
    public void ActivePasswordConfirmPanel(string passwordToConfirm)
    {
        passwordConfirmPanel.SetActive(true);
        this.passwordToConfirm= passwordToConfirm;
    }
    public void ConfirmPassword(Button btn)
    {
        NetworkManager networkManager=FindObjectOfType<NetworkManager>();
        if(networkManager.password==passwordToConfirm)
        {
            networkManager.OnClickBtn(btn);
            Singleton<AudioManager>.Instance.ClickButtonSound();
        }
        else
        {
            Singleton<AudioManager>.Instance.PlaySound(Singleton<AudioManager>.Instance.error);
        }
        btn.transform.parent.GetChild(0).GetComponent<TMP_InputField>().text = "";
    }
    public void ResetRoomPassWhenCreat()
    {
        roomNameField.text = "";
        passwordField.text = "";
    }
}
