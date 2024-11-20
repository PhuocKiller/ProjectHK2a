using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    [SerializeField] Bars hpBarHUD, manaBarHUD, expBarHUD;
    [SerializeField] TextMeshProUGUI levelText;
    public PlayerController player;
    private void Start()
    {
        StartCoroutine(DelayCheckPlay());
    }
    private void Update()
    {
        if (player == null) return;
        // Debug.Log(player.playerStat.currentHealth);
        hpBarHUD.UpdateBar(player.playerStat.currentHealth, player.playerStat.maxHealth);
        manaBarHUD.UpdateBar(player.playerStat.currentMana, player.playerStat.maxMana);
        expBarHUD.UpdateBar(player.playerStat.currentXP, player.playerStat.maxXP);
        levelText.text = player.playerStat.level.ToString();
    }
    IEnumerator DelayCheckPlay()
    {
        yield return new WaitForSeconds(0.2f);
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
    }
}
