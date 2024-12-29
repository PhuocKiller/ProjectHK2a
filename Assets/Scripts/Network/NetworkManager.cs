using Fusion;
using Fusion.Editor;
using Fusion.Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject roomItem;
    [SerializeField]
    private Transform parentRoomItem;
    public string playerID;
    public NetworkRunner runner;
    public NavMeshSurface navMesh;
    [SerializeField]
    GameObject gameManagerObj, playerManagerObj, roomPlayerInfo;
    [SerializeField]
    public GameObject[] players, creeps, naturals, buildings, basicItems, shieldItems, armorItems, weaponItems, bootItems, onlineItems;
    public float[] itemsDropChance;
    GameNetworkCallBack gameNetworkCallBack;
    public string roomName;
    public string password;
    [SerializeField]
    public UnityEvent onConnected, onJoinRoom;
    [SerializeField] public Transform[] spawnPointPlayer, spawnPointCreep, spawnPointNatural, spawnPointTower, spawnPointBase;
    public int playerIndex, playerTeam;
    bool flagLogin;

    private void OnEnable()
    {
        //  gameNetworkCallBack.OnPlayerJoinRegister(SpawnPlayer);
    }
    private void OnDisable()
    {
        //  gameNetworkCallBack.OnPlayerJoinUnRegister(SpawnPlayer);
    }
    private void Awake()
    {
        runner = GetComponent<NetworkRunner>();
        gameNetworkCallBack = GetComponent<GameNetworkCallBack>();
        ResetNameAndPass();
    }

    public void SpawnPlayer(NetworkRunner m_runner, PlayerRef player)
    {
        bool flag = false;
        foreach (var playerObject in Login.playersGame)
        {
            if (playerObject.Key == m_runner.GetPlayerUserId(player))
            {
                if (player == m_runner.LocalPlayer)
                {
                    playerObject.Value.Object.RequestStateAuthority();
                    playerObject.Value.playerCallBack.CallBackReconect();
                }
                flag = true;
            }
        }
        if (flag == true) return;
        //SpawnWhenJoinRoom(m_runner, player);
        if (player == runner.LocalPlayer && FindObjectOfType<GameManager>().GetComponent<NetworkObject>().StateAuthority == runner.LocalPlayer)
        {
            StartCoroutine(SpawnWhenStartGame(m_runner, player));
        }

        if (player == runner.LocalPlayer)
        {
            NetworkObject characterObj = runner.Spawn(players[playerIndex],
                spawnPointPlayer[playerTeam].position + Vector3.right * 5 * (playerTeam == 0 ? 1 : -1), spawnPointPlayer[playerTeam].rotation,
                inputAuthority: player,
                onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                {
                    obj.GetComponent<PlayerController>().playerID = runner.GetPlayerUserId(player);
                    obj.GetComponent<PlayerController>().playerTeam = playerTeam;
                });
        }
    }
    public void SpawnWhenJoinRoom(NetworkRunner m_runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            if (runner.IsSharedModeMasterClient)
            {
                runner.Spawn(gameManagerObj, inputAuthority: player);
                runner.Spawn(playerManagerObj, inputAuthority: player);
            }
            runner.Spawn(roomPlayerInfo, inputAuthority: player, onBeforeSpawned:
                (NetworkRunner runner, NetworkObject obj) =>
                {
                    obj.GetComponent<RoomPlayerInfo>().playerID = playerID;
                    obj.GetComponent<RoomPlayerInfo>().playerTeam = playerTeam;
                    obj.GetComponent<RoomPlayerInfo>().playerIndex = playerIndex;
                });
        }

    }
    IEnumerator SpawnWhenStartGame(NetworkRunner m_runner, PlayerRef player)
    {
        for (int i = 0; i < spawnPointBase.Length; i++)
        {
            NetworkObject towerObject = runner.Spawn(buildings[i + 1], spawnPointBase[i].position, spawnPointBase[i].rotation, player, //building 0 là tower
              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
              {
                  obj.GetComponent<BuildingController>().playerTeam = i;
                  obj.GetComponent<BuildingController>().towerID = 3;
              });
        }
        for (int i = 0; i < 2; i++)
        {
            NetworkObject baseRegen = runner.Spawn(buildings[3], spawnPointPlayer[i].position, spawnPointPlayer[i].rotation, player, //building 0 là tower
              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
              {
                  obj.GetComponent<BaseRegen>().SetUp(i, 0.05f);
              });
        }
        for (int i = 0; i < spawnPointTower.Length; i++)
        {
            NetworkObject towerObject = runner.Spawn(buildings[0], spawnPointTower[i].position, spawnPointTower[i].rotation, player,
              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
              {
                  obj.GetComponent<BuildingController>().playerTeam = i <= 3 ? 0 : 1;

                  obj.GetComponent<BuildingController>().towerID = i <= 2 ? i : ((i == 3 || i == 7) ? 2 : i - 4);
              });
            yield return new WaitForSeconds(0.05f);
        }
        // navMesh.BuildNavMesh();
    }
    public void SpawnCreep(PlayerRef player)
    {
        if (FindObjectOfType<GameManager>().GetComponent<NetworkObject>().StateAuthority != runner.LocalPlayer) return;
        StartCoroutine(SpawnMeleeCreep(player));
        StartCoroutine(SpawnRangeCreep(player));
        StartCoroutine(SpawnNatural(player));
    }
    #region SpawnCreep
    IEnumerator SpawnNatural(PlayerRef player)
    {
        if (FindObjectOfType<GameManager>().GetComponent<NetworkObject>().StateAuthority == runner.LocalPlayer)
        {
            for (int i = 0; i < spawnPointNatural.Length; i++)
            {
                if (AroundEnemies(spawnPointNatural[i], 15).Count() == 0)
                {
                    runner.Spawn(naturals[i], spawnPointNatural[i].position, spawnPointNatural[i].rotation,
                        inputAuthority: player,
                       onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                       {
                           obj.GetComponent<CreepController>().playerTeam = 2;
                           obj.GetComponent<CreepController>().finalTargetDestination = spawnPointNatural[i].position;
                       });
                }
                else
                {
                    List<CharacterController> creepEnemies = AroundEnemies(spawnPointNatural[i], 15)
                        .Where(s => s.GetComponent<CreepController>() != null
                    && s.GetComponent<CreepController>().creepType == Creep_Types.Natural).ToList();
                    foreach (var creep in creepEnemies)
                    {
                        creep.GetComponent<CreepController>().playerStat.currentHealth
                            = creep.GetComponent<CreepController>().playerStat.maxHealth;
                    }
                }
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
    public List<CharacterController> AroundEnemies(Transform spawnPoint, float range)
    {
        List<CharacterController> allEnemies = new List<CharacterController>();
        allEnemies.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(spawnPoint.position, range);
        foreach (var hitCollider in hitColliders)
        {
            CharacterController characPlayer = hitCollider.gameObject.GetComponent<CharacterController>();
            if (characPlayer != null && !allEnemies.Contains(characPlayer))
            {
                allEnemies.Add(characPlayer);
            }
        }
        return allEnemies;

    }
    IEnumerator SpawnMeleeCreep(PlayerRef player)
    {
        for (int i = 0; i < 3; i++)
        {
            runner.Spawn(creeps[0], spawnPointCreep[0].position + Vector3.left * 2f * i, spawnPointCreep[0].rotation,
                             inputAuthority: player,
                           onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                           {
                               obj.GetComponent<CreepController>().playerTeam = 0;
                           });
            yield return new WaitForSeconds(0.05f);
            runner.Spawn(creeps[0], spawnPointCreep[1].position + Vector3.right * 2f * i, spawnPointCreep[1].rotation,
                 inputAuthority: player,
               onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
               {
                   obj.GetComponent<CreepController>().playerTeam = 1;
               });
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator SpawnRangeCreep(PlayerRef player)
    {
        runner.Spawn(creeps[1], spawnPointCreep[0].position + Vector3.left * 6, spawnPointCreep[0].rotation,
                                inputAuthority: player,
                              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                              {
                                  obj.GetComponent<CreepController>().playerTeam = 0;
                              });
        yield return new WaitForSeconds(0.05f);
        runner.Spawn(creeps[1], spawnPointCreep[1].position + Vector3.right * 6, spawnPointCreep[1].rotation,
                             inputAuthority: player,
                           onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                           {
                               obj.GetComponent<CreepController>().playerTeam = 1;
                           });
    }
    #endregion
    #region SpawnItems
    public void SpawnObjWhenAddItem(GameObject itemObj, int indexItemSlot)
    {
        NetworkObject item = runner.Spawn(itemObj);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        player.SetParentItemRPC(item.Id, indexItemSlot);
    }
    public void DestroyObjWhenRemoveItem(string name)
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        foreach (Transform item in player.buffFromItemManager)
        {
            if (item.GetComponent<InventoryItemBase>().Name == name)
            {
                Destroy(item.gameObject);
                break;
            }
        }
    }
    public void SpawnItemFromCreep(int indexItem, Vector3 posSpawn)
    {
        NetworkObject item = runner.Spawn(basicItems[indexItem], posSpawn);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
    }
    public GameObject FindItemBaseOnName(string name)
    {
        for (int i = 0; i < basicItems.Length; i++)
        {
            if (name == basicItems[i].GetComponent<InventoryItemBase>().Name) return basicItems[i];
        }
        for (int i = 0; i < shieldItems.Length; i++)
        {
            if (name == shieldItems[i].GetComponent<InventoryItemBase>().Name) return shieldItems[i];
        }
        for (int i = 0; i < armorItems.Length; i++)
        {
            if (name == armorItems[i].GetComponent<InventoryItemBase>().Name) return armorItems[i];
        }
        for (int i = 0; i < weaponItems.Length; i++)
        {
            if (name == weaponItems[i].GetComponent<InventoryItemBase>().Name) return weaponItems[i];
        }
        for (int i = 0; i < bootItems.Length; i++)
        {
            if (name == bootItems[i].GetComponent<InventoryItemBase>().Name) return bootItems[i];
        }
        return null;
    }
    public int FindOnlineItemsIndex(string name)
    {
        for (int i = 0; i < onlineItems.Length; i++)
        {
            if (name == onlineItems[i].GetComponent<InventoryItemBase>().Name) return i;
        }
        return -1;
    }
    #endregion
    public void EnterRoomName(string roomName)
    {
        this.roomName = roomName;
    }
    public void EnterPassword(string password)
    {
        this.password = password;
    }
    public async void OnClickBtn(Button btn)
    {
        if (runner != null && playerID != "")
        {
            btn.interactable = false;
            Singleton<Loading>.Instance.ShowLoading();
            Singleton<AudioManager>.Instance.ClickButtonSound();
            gameNetworkCallBack ??= GetComponent<GameNetworkCallBack>();
            await runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = roomName,
                PlayerCount = 6,
                CustomLobbyName = "VN",
                SceneManager = GetComponent<LoadSceneManager>(),
                AuthValues = new AuthenticationValues()
                {
                    UserId = playerID,
                }
                ,
                SessionProperties = new Dictionary<string, SessionProperty>
                {
                    ["Password"] = password,
                }

            });
            btn.interactable = true;
            //  onConnected?.Invoke();
            onJoinRoom?.Invoke();
            Singleton<Loading>.Instance.HideLoading();
        }
        else
        {
            Singleton<AudioManager>.Instance.PlaySound(Singleton<AudioManager>.Instance.error);
        }
    }

    public async void OnClickJoinBtn()
    {
        Singleton<Loading>.Instance.ShowLoading();
        gameNetworkCallBack ??= GetComponent<GameNetworkCallBack>();
        gameNetworkCallBack.StartGameRegister(OnSessionListChanged);
        await runner.JoinSessionLobby(SessionLobby.Custom, "VN",
            authentication: new AuthenticationValues()
            {
                UserId = playerID,
            });
        Singleton<Loading>.Instance.HideLoading();
    }
    private void OnSessionListChanged(List<SessionInfo> sessionInfos)
    {
        foreach (Transform child in parentRoomItem)
        {
            Destroy(child.gameObject);
        }
        foreach (var session in sessionInfos)
        {
            GameObject room = Instantiate(roomItem, parentRoomItem);
            room.GetComponentInChildren<TextMeshProUGUI>().text = session.Name;
            bool isLock = session.Properties["Password"] != "";
            room.transform.GetChild(1).gameObject.SetActive(isLock); //lock
            Button btn = room.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                roomName = session.Name;
                if (!isLock)
                {
                    OnClickBtn(btn);
                }
                else
                {
                    FindObjectOfType<PanelChangeCharacter>().ActivePasswordConfirmPanel(session.Properties["Password"]);
                }
            });
        }
    }
    public void ResetNameAndPass()
    {
        roomName = ""; password = "";
    }
    public void ShutdownRunner()
    {
        if (runner.IsRunning)
        {
            runner.Shutdown(false, forceShutdownProcedure: true);
        }
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
