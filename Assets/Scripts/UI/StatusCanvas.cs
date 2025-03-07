using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusCanvas : NetworkBehaviour
{
    [SerializeField] GameObject playerBuffs;
    [SerializeField] PlayerController player;
    [SerializeField] CreepController creep;
    [SerializeField] Sprite[] hpImages;
    NetworkManager networkManager;
    Shield firstShield;
    public Bars healthBarPlayer, manaBarPlayer, XPbar, TimeRemainingBar, timeShieldRemainingBar, teleBar;
    public TextMeshProUGUI statusBeingTMP, injureDamage,levelTMP;
    Vector3 fixPosInjureDamage;
    TickTimer timerhideInjureDamage;
    public Image minimapImage;
    bool isSameTeam;
    [Networked] bool playerBeingAttack {  get; set; }
    [Networked] public TickTimer TimeOfTele { get; set; }
    [Networked] public float currentTeleTimeStatus { get; set; }
    
    public override void Spawned()
    {
        base.Spawned();
        player=GetComponentInParent<PlayerController>();
        minimapImage.gameObject.SetActive(true);
        networkManager=FindObjectOfType<NetworkManager>();
        if (!player) //ko l� player
        {
            creep = GetComponentInParent<CreepController>();
            isSameTeam = creep.playerTeam == networkManager.playerTeam;
            minimapImage.color = creep.playerTeam == 0 ? Color.red : (creep.playerTeam == 1 ? Color.blue : Color.magenta);
        }
        else //l� player
        {
            isSameTeam = player.playerTeam == networkManager.playerTeam;
            minimapImage.color = player.playerTeam == 0 ? Color.red : (player.playerTeam == 1 ? Color.blue : Color.magenta);
            if (HasStateAuthority)
            {
                fixPosInjureDamage = injureDamage.GetComponent<RectTransform>().localPosition;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        healthBarPlayer.fillBar.sprite = isSameTeam ? hpImages[0] : hpImages[1];
        healthBarPlayer.fadeFillBar.sprite = isSameTeam ? hpImages[0] : hpImages[1];
        firstShield =playerBuffs.GetComponentInChildren<Shield>();
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
            manaBarPlayer.UpdateBar(player.playerStat.currentMana, player.playerStat.maxMana);
            levelTMP.text = player.playerStat.level.ToString();
            if (player.playerStat.isBeingTele)
            {
                if (TimeOfTele.RemainingTime(Runner) > 0)
                {
                    currentTeleTimeStatus = (float)TimeOfTele.RemainingTime(Runner);
                }
                else
                {
                    if (HasStateAuthority) player.TeleToBase();
                }
            }
            player.transform.GetChild(0).GetChild(0).rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
            if(HasStateAuthority)
            {
                player.playerStat.canBuyItem = (player.transform.position - player.runnerManager.spawnPointPlayer[player.playerTeam].position).magnitude < 15;
            }
                
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
