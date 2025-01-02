using Fusion;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class RoomGame : MonoBehaviour
{
    GameNetworkCallBack gameNetworkCallBack;
    [SerializeField] NetworkRunner runner;
    [SerializeField] Sprite[] playerImages;
    [SerializeField] GameObject playerItem, coolDownPanel;
    [SerializeField] Transform darkTeamParent, lightTeamParent;
    [SerializeField] public Button playBtn,backBtn, kickBtn, passKeyBtn, rightTeamBtn,leftTeamBtn,rightPlayerBtn,leftPlayerBtn;
    [SerializeField] TextMeshProUGUI roomNameText, cooldownTime;
    [SerializeField] Image chosePlayerImage;
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
        leftTeamBtn.interactable = true;
        rightTeamBtn.interactable = true;
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
        StartCoroutine(DelayUpdateUI());
    }
    private void PlayerLeftRoom(NetworkRunner m_runner, PlayerRef player)
    {
        FindObjectOfType<GameManager>().RemovePlayerWhenLeave(m_runner, player);
        StartCoroutine(DelayUpdateUI());
    }
    public RoomPlayerInfo CheckInfoPlayer(PlayerRef player)
    {
        RoomPlayerInfo[] allPlayers = FindObjectsOfType<RoomPlayerInfo>();
        RoomPlayerInfo[] players= allPlayers.Where(s => s.playerID == runner.GetPlayerUserId(player)).ToArray();
        return players[0];
    }
    public void UpdateUI()
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
            RoomPlayerInfo playerInfo = CheckInfoPlayer(playerJoin);
            string namePlayer = playerInfo.playerID;
            int playerTeam = playerInfo.playerTeam;
            int playerIndex = playerInfo.playerIndex;
            GameObject playerPrefab = Instantiate
            (playerItem, playerTeam == 0 ? darkTeamParent : lightTeamParent);
            playerPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text= namePlayer;
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
        chosePlayerImage.sprite = playerImages[CheckInfoPlayer(runner.LocalPlayer).playerIndex];
        StartCoroutine(CheckPlayerBtn());
    }
    public IEnumerator DelayUpdateUI()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateUI();
    }
    IEnumerator CheckPlayerBtn()
    {
        yield return null;
        playBtn.interactable = isMasterPlayer
            && (darkTeamParent.childCount == lightTeamParent.childCount
            || darkTeamParent.childCount+lightTeamParent.childCount==1);
        backBtn.interactable = darkTeamParent.childCount + lightTeamParent.childCount == 1 || !isMasterPlayer;
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
    public void RightTeamBtn()
    {
        CheckInfoPlayer(runner.LocalPlayer).playerTeam = 1;
        rightTeamBtn.interactable = false;
        leftTeamBtn.interactable = true;
    }
    public void LeftTeamBtn()
    {
        CheckInfoPlayer(runner.LocalPlayer).playerTeam = 0;
        leftTeamBtn.interactable = false;
        rightTeamBtn.interactable = true;
    }
    public void RightPlayerBtn()
    {
        RoomPlayerInfo playerInfo = CheckInfoPlayer(runner.LocalPlayer);
        if (playerInfo.playerIndex<5)
        {
            playerInfo.playerIndex += 1;
        }
        else
        {
            rightPlayerBtn.interactable = false;
        }
        leftPlayerBtn.interactable = true;
    }
    public void LeftPlayerBtn()
    {
        RoomPlayerInfo playerInfo = CheckInfoPlayer(runner.LocalPlayer);
        if (playerInfo.playerIndex > 0)
        {
            playerInfo.playerIndex -= 1;
        }
        else
        {
            leftPlayerBtn.interactable = false;
        }
        rightPlayerBtn.interactable = true;
    }
}
