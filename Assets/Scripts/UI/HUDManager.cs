using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    PlayerController player;
    public GameObject followBtn, unFollowBtn, winPanel;
    [SerializeField] public TextMeshProUGUI coinsValue, playerName, winTeamTxt;
    private void Awake()
    {
     gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        StartCoroutine(DelayCheckPlay());
        playerName.text=FindObjectOfType<NetworkManager>().playerID;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        coinsValue.text=player.playerStat.coinsValue.ToString();
    }
    IEnumerator DelayCheckPlay()
    {
        yield return new WaitForSeconds(0.2f);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
    }
    public void FollowEnemy()
    {
        player.playerStat.isFollowEnemy = true;
    }
    public void UnFollowEnemy()
    {
        player.playerStat.isFollowEnemy =false;
        unFollowBtn.SetActive(false);
    }
}
