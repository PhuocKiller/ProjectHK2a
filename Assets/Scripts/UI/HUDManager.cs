using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    PlayerController player;
    public GameObject followBtn, unFollowBtn;
    private void OnEnable()
    {
        StartCoroutine(DelayCheckPlay());
    }

    // Update is called once per frame
    void Update()
    {
        
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
