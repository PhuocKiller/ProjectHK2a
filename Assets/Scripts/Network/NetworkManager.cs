using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
   NetworkRunner runner;
    [SerializeField]
    GameObject gameManagerObj, playerManagerObj;
    [SerializeField]
    GameObject[] players, creeps;
    GameNetworkCallBack gameNetworkCallBack;
    [SerializeField]
    UnityEvent onConnected;
    [SerializeField]
    public Transform[] spawnPointTeam;
    public int playerIndex, playerTeam;

    private void Awake()
    {
        runner = GetComponent<NetworkRunner>();
        gameNetworkCallBack = GetComponent<GameNetworkCallBack>();
    }
   
    private void SpawnPlayer(NetworkRunner m_runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer && runner.IsSharedModeMasterClient)
        {
            runner.Spawn(gameManagerObj, inputAuthority: player);
            runner.Spawn(playerManagerObj, inputAuthority: player);
                    }
        if (player == runner.LocalPlayer)
        {
            // Transform spawn = spawnPointTeam[players[runner.LocalPlayer.PlayerId].GetComponent<PlayerController>().playerTeam];
            NetworkObject characterObj = runner.Spawn(players[playerIndex],
                spawnPointTeam[playerTeam].position, spawnPointTeam[playerTeam].rotation,
                inputAuthority: player,
                onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                {
                    obj.GetComponent<PlayerController>().playerTeam = playerTeam;
                });
        }
    }
    public void SpawnCreep(PlayerRef player)
    {
        if (!runner.IsSharedModeMasterClient) return;
        SpawnMeleeCreep(player);
        SpawnRangeCreep(player);
    }
    void SpawnMeleeCreep(PlayerRef player)
    {
        for (int i = -1; i < 1; i++)
        {
            runner.Spawn(creeps[0], spawnPointTeam[0].position + Vector3.right * 3 + Vector3.back * 2f * i, spawnPointTeam[0].rotation,
                             inputAuthority: player,
                           onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                           {
                               obj.GetComponent<CreepController>().playerTeam = 0;
                           });
            runner.Spawn(creeps[0], spawnPointTeam[1].position + Vector3.left * 3 + Vector3.back * 2f * i, spawnPointTeam[1].rotation,
                 inputAuthority: player,
               onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
               {
                   obj.GetComponent<CreepController>().playerTeam = 1;
               });
        }
    }
    void SpawnRangeCreep(PlayerRef player)
    {
        runner.Spawn(creeps[1], spawnPointTeam[0].position + Vector3.right * 1, spawnPointTeam[0].rotation,
                                inputAuthority: player,
                              onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                              {
                                  obj.GetComponent<CreepController>().playerTeam = 0;
                              });
        runner.Spawn(creeps[1], spawnPointTeam[1].position + Vector3.left * 1, spawnPointTeam[1].rotation,
                             inputAuthority: player,
                           onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                           {
                               obj.GetComponent<CreepController>().playerTeam = 1;
                           });
    }
    public async void OnClickBtn(Button btn)
    {
        if (runner != null)
        {
            btn.interactable = false;
            Singleton<Loading>.Instance.ShowLoading();
            gameNetworkCallBack ??= GetComponent<GameNetworkCallBack>();
            gameNetworkCallBack.OnPlayerJoinRegister(SpawnPlayer);
            await runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = "Begin",
                CustomLobbyName = "VN",
                SceneManager = GetComponent<LoadSceneManager>()
            });
            btn.interactable = true;
            onConnected?.Invoke();
            Singleton<Loading>.Instance.HideLoading();
            
        }
    }
    public void ShutdownRunner()
    {
        if (runner.IsRunning)
        {
            runner.Shutdown(false, forceShutdownProcedure: true);
        }
    }
}
