using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomGame : MonoBehaviour
{
    GameNetworkCallBack gameNetworkCallBack;
    [SerializeField] Sprite[] playerImages;
    [SerializeField] GameObject playerItem;
    [SerializeField] Transform darkTeamParent, lightTeamParent;
    [SerializeField] Button playBtn;
    [SerializeField] TextMeshProUGUI roomNameText;
    private void Awake()
    {
        gameNetworkCallBack = FindObjectOfType<GameNetworkCallBack>();
    }
    private void OnEnable()
    {
        gameNetworkCallBack.OnPlayerJoinRegister(PlayerJoinRoom);
        playBtn.interactable=FindObjectOfType<NetworkRunner>().IsSharedModeMasterClient;
        roomNameText.text = FindObjectOfType<NetworkRunner>().SessionInfo.Name;
    }
    private void OnDisable()
    {
        gameNetworkCallBack.OnPlayerJoinUnRegister(PlayerJoinRoom);
    }
    private void PlayerJoinRoom(NetworkRunner m_runner, PlayerRef player)
    {
        FindObjectOfType<NetworkManager>().SpawnWhenJoinRoom(m_runner, player);
        foreach (Transform child in darkTeamParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in lightTeamParent)
        {
            Destroy(child.gameObject);
        }
        foreach (var playerJoin in m_runner.ActivePlayers)
        {
            string namePlayer = m_runner.GetPlayerUserId(playerJoin);
            int playerTeam = int.Parse(namePlayer[namePlayer.Length - 2].ToString());
            int playerIndex = int.Parse(namePlayer[namePlayer.Length - 1].ToString());
            GameObject playerPrefab = Instantiate
            (playerItem, playerTeam == 0 ? darkTeamParent : lightTeamParent);
            playerPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text
                = namePlayer.Substring(0, namePlayer.Length-2);
            playerPrefab.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = playerImages[playerIndex];
            
        }
    }
    public void PlayGame()
    {
        FindObjectOfType<GameManager>().GoWaitBeforeStart();
    }
}
