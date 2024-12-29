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
    [SerializeField] public Button playBtn,backBtn, kickBtn, passKeyBtn;
    [SerializeField] TextMeshProUGUI roomNameText, cooldownTime;
    public string namePlayer;
    bool isMasterPlayer;
    private void Awake()
    {
        gameNetworkCallBack = FindObjectOfType<GameNetworkCallBack>();
    }
    private void OnEnable()
    {
        gameNetworkCallBack.OnPlayerJoinRegister(PlayerJoinRoom);
        gameNetworkCallBack.onPlayerLeft += PlayerLeftRoom;
        roomNameText.text = runner.SessionInfo.Name;
        namePlayer = "";
        foreach (Transform child in darkTeamParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lightTeamParent)
        {
            Destroy(child.gameObject);
        }
        isMasterPlayer=false;
    }
    private void OnDisable()
    {
        gameNetworkCallBack.OnPlayerJoinUnRegister(PlayerJoinRoom);
        gameNetworkCallBack.onPlayerLeft -= PlayerLeftRoom;
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
        FindObjectOfType<GameManager>().AddPlayerWhenJoin(m_runner, player);
        UpdateUI(m_runner, player);
    }
    private void PlayerLeftRoom(NetworkRunner m_runner, PlayerRef player)
    {
        FindObjectOfType<GameManager>().RemovePlayerWhenLeave(m_runner, player);
        UpdateUI(m_runner, player);
    }
    public void UpdateUI(NetworkRunner m_runner, PlayerRef player)
    {
        isMasterPlayer = FindObjectOfType<GameManager>().GetComponent<NetworkObject>().StateAuthority == runner.LocalPlayer;
        foreach (Transform child in darkTeamParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lightTeamParent)
        {
            Destroy(child.gameObject);
        }
        kickBtn.interactable = isMasterPlayer;
        passKeyBtn.interactable = isMasterPlayer;
        foreach (var playerJoin in runner.ActivePlayers)
        {
            bool isKeyPlayer = FindObjectOfType<GameManager>().GetComponent<NetworkObject>().StateAuthority == playerJoin;
            string namePlayer = runner.GetPlayerUserId(playerJoin);
            int playerTeam = int.Parse(namePlayer[namePlayer.Length - 2].ToString());
            int playerIndex = int.Parse(namePlayer[namePlayer.Length - 1].ToString());
            GameObject playerPrefab = Instantiate
            (playerItem, playerTeam == 0 ? darkTeamParent : lightTeamParent);
            playerPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text
                = namePlayer.Substring(0, namePlayer.Length - 2);
            playerPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = playerImages[playerIndex]; //hình ảnh ava
            playerPrefab.transform.GetChild(0).GetComponent<Image>().color= playerJoin==runner.LocalPlayer? Color.red : Color.white; //Màu viền ava
            playerPrefab.transform.GetChild(2).GetComponent<Image>().enabled= isKeyPlayer; //hình ảnh key
            Button btnPlayer= playerPrefab.GetComponent<Button>();
            btnPlayer.onClick.AddListener(()=>
            {
                kickBtn.interactable = isMasterPlayer && !isKeyPlayer;
                passKeyBtn.interactable = isMasterPlayer && !isKeyPlayer;
                this.namePlayer = namePlayer;
            });
        }
        StartCoroutine(CheckPlayerBtn());
    }
    IEnumerator CheckPlayerBtn()
    {
        yield return null;
        playBtn.interactable = isMasterPlayer
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
    public void KickPlayerBtn()
    {
        for (int i = 0;i<6;i++)
        {
            if (FindObjectOfType<GameManager>().playersInRoom.Get(i)==namePlayer)
            {
                FindObjectOfType<GameManager>().playersInRoom.Set(i, "");
            }
        }
    }
    public void PassKeyBtn()
    {
        FindObjectOfType<GameManager>().keyPlayer= namePlayer;
    }
}
