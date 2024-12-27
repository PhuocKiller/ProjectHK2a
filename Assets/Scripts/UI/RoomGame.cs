using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomGame : MonoBehaviour
{
    GameNetworkCallBack gameNetworkCallBack;
    [SerializeField] NetworkRunner runner;
    [SerializeField] Sprite[] playerImages;
    [SerializeField] GameObject playerItem, coolDownPanel;
    [SerializeField] Transform darkTeamParent, lightTeamParent;
    [SerializeField] Button playBtn;
    [SerializeField] TextMeshProUGUI roomNameText, cooldownTime;
    private void Awake()
    {
        gameNetworkCallBack = FindObjectOfType<GameNetworkCallBack>();
    }
    private void OnEnable()
    {
        gameNetworkCallBack.OnPlayerJoinRegister(PlayerJoinRoom);
        gameNetworkCallBack.onPlayerLeft += UpdateUI;
        roomNameText.text = runner.SessionInfo.Name;
        foreach (Transform child in darkTeamParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lightTeamParent)
        {
            Destroy(child.gameObject);
        }
    }
    private void OnDisable()
    {
        gameNetworkCallBack.OnPlayerJoinUnRegister(PlayerJoinRoom);
        gameNetworkCallBack.onPlayerLeft -= UpdateUI;
        foreach (Transform child in darkTeamParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lightTeamParent)
        {
            Destroy(child.gameObject);
        }
    }
    private void PlayerJoinRoom(NetworkRunner m_runner, PlayerRef player)
    {
        FindObjectOfType<NetworkManager>().SpawnWhenJoinRoom(m_runner, player);
        UpdateUI(m_runner, player);
    }
    void UpdateUI(NetworkRunner m_runner, PlayerRef player)
    {
        foreach (Transform child in darkTeamParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lightTeamParent)
        {
            Destroy(child.gameObject);
        }
        foreach (var playerJoin in runner.ActivePlayers)
        {
            string namePlayer = runner.GetPlayerUserId(playerJoin);
            int playerTeam = int.Parse(namePlayer[namePlayer.Length - 2].ToString());
            int playerIndex = int.Parse(namePlayer[namePlayer.Length - 1].ToString());
            GameObject playerPrefab = Instantiate
            (playerItem, playerTeam == 0 ? darkTeamParent : lightTeamParent);
            playerPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text
                = namePlayer.Substring(0, namePlayer.Length - 2);
            playerPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = playerImages[playerIndex];
        }
        StartCoroutine(CheckPlayerBtn());
    }
    IEnumerator CheckPlayerBtn()
    {
        yield return null;
        playBtn.interactable = runner.IsSharedModeMasterClient
            && (darkTeamParent.childCount == lightTeamParent.childCount
            || darkTeamParent.childCount==1 || lightTeamParent.childCount==1);
    }
    public void PlayGame()
    {
        FindObjectOfType<GameManager>().GoTransitionState();
    }
    public void ControlCooldownTimeBeforePlay(string time, bool active)
    {
        coolDownPanel.SetActive(active);
        cooldownTime.text= time;
    }
}
