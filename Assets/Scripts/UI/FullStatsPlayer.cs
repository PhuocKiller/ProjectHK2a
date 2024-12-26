using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FullStatsPlayer : MonoBehaviour
{
    public PlayerController player;
    [SerializeField] TextMeshProUGUI healthTMP, manaTMP,attackTMP, magicAmpliTMP, defTMP, magicResisTMP, attackSpeedTMP, moveSpeedTMP,critChanceTMP,critDamageTMP;
    private void OnEnable()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        this.player = player;
    }
    void Update()
    {
        if (player == null) return;
        healthTMP.text= player.playerStat.currentHealth.ToString() +"/"+ player.playerStat.maxHealth.ToString();
        manaTMP.text= player.playerStat.currentMana.ToString() + "/" + player.playerStat.maxMana.ToString();
        attackTMP.text = player.playerStat.damage.ToString();
        magicAmpliTMP.text = ((player.playerStat.magicAmpli * 100)).ToString() + "%";
        defTMP.text = player.playerStat.defend.ToString();
        magicResisTMP.text = player.playerStat.magicResistance.ToString();
        attackSpeedTMP.text = player.playerStat.attackSpeed.ToString();
        moveSpeedTMP.text = player.playerStat.moveSpeed.ToString();
        critChanceTMP.text = (player.playerStat.criticalChance*100).ToString() + "%";
        critDamageTMP.text = (player.playerStat.criticalDamage * 100).ToString() + "%";
    }
    IEnumerator DelayCheckPlay()
    {
        yield return new WaitForSeconds(0.2f);
        
    }
}
