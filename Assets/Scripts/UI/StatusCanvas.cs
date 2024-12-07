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
    [SerializeField] CreepController creep;
    Shield firstShield;
    public Bars healthBarPlayer, manaBarPlayer, XPbar, TimeRemainingBar, timeShieldRemainingBar;
    public TextMeshProUGUI statusBeingTMP, injureDamage;
    Vector3 fixPosInjureDamage;
    TickTimer timerhideInjureDamage;
    [Networked] bool playerBeingAttack {  get; set; }

    public override void Spawned()
    {
        base.Spawned();
        player=GetComponentInParent<PlayerController>();
        if(!player)
        {
            creep = GetComponentInParent<CreepController>();
        }
        if (player&&HasStateAuthority)
        {
            fixPosInjureDamage = injureDamage.GetComponent<RectTransform>().position;
        }
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

        if(player)
        {
            healthBarPlayer.UpdateBar(player.playerStat.currentHealth, player.playerStat.maxHealth);
            player.transform.GetChild(0).GetChild(0).rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        }
        else
        {
            healthBarPlayer.UpdateBar(creep.playerStat.currentHealth, creep.playerStat.maxHealth);
            creep.transform.GetChild(0).GetChild(0).rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        }
        
        statusBeingTMP.transform.rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        TimeRemainingBar.transform.rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        injureDamage.transform.rotation = Quaternion.AngleAxis
                (Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        if (playerBeingAttack)
        {
            injureDamage.GetComponent<RectTransform>().position += Vector3.up * 2f * Runner.DeltaTime;
            if (timerhideInjureDamage.ExpiredOrNotRunning(Runner))
            {
                playerBeingAttack = false;
                injureDamage.GetComponent<RectTransform>().localPosition = fixPosInjureDamage;
                injureDamage.text = "";
            }
        }
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
    public void PlayerHaveInjure(int injureDamage,bool isCritPhysic)
    {
        this.injureDamage.text= injureDamage.ToString();
        this.injureDamage.fontSize = isCritPhysic ? (player != null ? 120 : 70) : (player != null ? 70 : 35);
        this.injureDamage.color = isCritPhysic ? Color.red: Color.white;
        this.injureDamage.GetComponent<RectTransform>().localPosition=fixPosInjureDamage;
        playerBeingAttack = true;
        timerhideInjureDamage = TickTimer.CreateFromSeconds(Runner, 0.5f);
    }
}
