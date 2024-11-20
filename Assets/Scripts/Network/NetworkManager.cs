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
    GameObject playerStat, gameManagerObj, playerManagerObj;
    [SerializeField]
    GameObject[] players;
    GameNetworkCallBack gameNetworkCallBack;
    [SerializeField]
    UnityEvent onConnected;
    [SerializeField]
    Transform spawnPoint;

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
            NetworkObject characterObj = runner.Spawn(players[runner.LocalPlayer.PlayerId], spawnPoint.position, Quaternion.identity, inputAuthority: player);
        }

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
            
            onConnected?.Invoke();
            Singleton<Loading>.Instance.HideLoading();
            
        }
    }
    [SerializeField] Transform[] m_gridRoot;
    [SerializeField] SkillButton m_skillBtnPrefab;
    private Dictionary<SkillName, int> m_skillCollecteds;
    public IEnumerator DrawSkillButton(NetworkRunner m_runner, PlayerRef player)
    {
        yield return new WaitForSeconds(0.1f);
        m_skillCollecteds = FindObjectOfType<SkillManager>().SkillCollecteds;
        //if (m_skillCollecteds == null || m_skillCollecteds.Count <= 0) return;
        int index = -1;
        Debug.Log(m_skillCollecteds.Count);
        foreach (var skillCollected in m_skillCollecteds)
        {
            index++;
            Helper.ClearChilds(m_gridRoot[index]);
            var skillButtonClone = runner.Spawn(m_skillBtnPrefab, inputAuthority: player);
            Helper.AssignToRoot(m_gridRoot[index], skillButtonClone.transform,
                Vector3.zero, index == 4 ? Vector3.one * 1f : (index==0? 0.5f* Vector3.one:  0.7f *Vector3.one));
            skillButtonClone.Initialize(skillCollected.Key);
            skillButtonClone.skillButtonType = skillButtonClone.m_skillButtonTypes[index];

        }

    }
}
