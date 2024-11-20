using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StatusCanvas : NetworkBehaviour
{
    [SerializeField] GameObject playerBuffs;
    [SerializeField] PlayerController player;
    Shield firstShield;
    public Bars healthBarPlayer, manaBarPlayer, XPbar, TimeRemainingBar, timeShieldRemainingBar;
    public TextMeshProUGUI statusBeingTMP;

    public override void Spawned()
    {
        base.Spawned();
        player=GetComponentInParent<PlayerController>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        firstShield=playerBuffs.GetComponentInChildren<Shield>();
        timeShieldRemainingBar.gameObject.SetActive(firstShield);
        if (firstShield)
        {
            if (firstShield.GetComponent<NetworkObject>().IsValid)
            {
                timeShieldRemainingBar.UpdateBar(firstShield.currentDamageAbsorb, firstShield.maxDamageAbsorb);
            }
        }
        timeShieldRemainingBar.transform.rotation= Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        healthBarPlayer.UpdateBar(player.playerStat.currentHealth, player.playerStat.maxHealth);
        player.transform.GetChild(0).GetChild(0).rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        statusBeingTMP.transform.rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        TimeRemainingBar.transform.rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
    }
    public void ReduceDamageAbsoreShield(int damage, out int overBalanceDmg)
    {
        firstShield.ReduceCurrentDamageAbsorb(damage, out int overBalanceDamage);
        overBalanceDmg = overBalanceDamage;
    }
    public int GetCurrentDamageAbsorbShield()
    {
        if (firstShield != null) return firstShield.currentDamageAbsorb;
        else return 0;
    }
}
