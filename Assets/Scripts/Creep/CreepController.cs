using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class CreepController : NetworkBehaviour, ICanTakeDamage
{
    public NetworkManager runnerManager;
    public GameManager gameManager;
    public Joystick joystick;
    public OverlapSphereCreep overlapSphere;
    public NetworkObject normalAttackObj;
    Transform spawnTransform;
    [Networked] public int playerTeam { get; set; }
    public ListNetworkObject networkObjs;
    public List<Collider> collisionsEnvi = new List<Collider>();
    public BuffsOfPlayer buffsFromEnvi, buffsFromPassive;
    public PlayerStat playerStat;
    public PlayerScore playerScore;
    public StatusCanvas statusCanvas;
    Vector3 moveDirection;
    public CharacterController characterControllerPrototype;
    Animator animator;
    float speed;
    private int targetX, targetY, beforeTarget;
    float previousSpeedX, currentSpeedX, previousSpeedY, currentSpeedY;
    
    // 0 là normal
    // 1 là jump
    // 2 là injured
    // 3 là die
    // 4 là active attack
    // 5 là đang cast skill
    [Networked]public int state { get; set; }
   
    [SerializeField]
    public Transform  normalAttackTransform;

    [SerializeField] public Player_Types playerType;
    [HideInInspector] public SkillButton[] skillButtons;
    [SerializeField] GameObject[] statusDebuffs;

    [SerializeField][Networked] TickTimer TimeOfStunDebuff { get; set; }
    [SerializeField][Networked] TickTimer TimeOfSlowDebuff { get; set; }
    [SerializeField][Networked] TickTimer TimeOfSilenDebuff { get; set; }
    [Networked] float maxStunTimeStatus { get; set; }
    [Networked] float currentStunTimeStatus { get; set; }
    [Networked] float maxSilenTimeStatus { get; set; }
    [Networked] float currentSilenTimeStatus { get; set; }
    [Networked] float maxSlowTimeStatus { get; set; }
    [Networked] float currentSlowTimeStatus { get; set; }
    private void Awake()
    {
        characterControllerPrototype = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

    }
    public override void Spawned()
    {
        base.Spawned();

        if (Object.InputAuthority.PlayerId == Runner.LocalPlayer.PlayerId)
        {
            runnerManager = FindObjectOfType<NetworkManager>();
            gameManager = FindObjectOfType<GameManager>();
            spawnTransform = runnerManager.spawnPointTeam[playerTeam];
            TimeOfStunDebuff = TickTimer.CreateFromSeconds(Runner, 0);
            TimeOfSlowDebuff = TickTimer.CreateFromSeconds(Runner, 0);
            TimeOfSilenDebuff = TickTimer.CreateFromSeconds(Runner, 0);
            statusCanvas = GetComponentInChildren<StatusCanvas>();
            overlapSphere=GetComponentInChildren<OverlapSphereCreep>();
        }

    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        CalculateCanvas();
        CalculateStatusDebuff();
        if (state != 2) animator.enabled = !playerStat.isBeingStun;
        if (state == 3) return;
        
        if (!playerStat.isBeingStun && state != 4)
        {
            CalculateMove();
        }
        if (overlapSphere.closestCharac)
        {
            moveDirection = overlapSphere.closestCharac.transform.position - transform.position;
        }
        else
        {
            moveDirection = (runnerManager.spawnPointTeam[playerTeam == 0 ? 1 : 0].position - transform.position);
        }
        if (moveDirection.magnitude < 2)
        {
            AnimatorSetBoolRPC("isAttack", true);
            state = 4;
        }
        else
        {
            AnimatorSetBoolRPC("isAttack", false);
            state = 0;
        }
        CalculateEXP();
        animator.SetFloat("AttackSpeed", (float)playerStat.attackSpeed / 100);
        animator.SetFloat("MoveSpeed", (float)playerStat.moveSpeed / 300);
    }


    #region "SkillButton"
    public virtual void NormalAttack()
    {
        if(HasStateAuthority)
        {
            Runner.Spawn(normalAttackObj.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
     {
         obj.GetComponent<AttackObjectsCreep>().SetUpCreep(this, playerStat.damage, true, normalAttackTransform,
             false, false, false, 0.5f, 0);
     });
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void AnimatorSetTriggerRPC(string name)
    {
        animator.SetTrigger(name);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void AnimatorSetBoolRPC(string name, bool isActive)
    {
        animator.SetBool(name,isActive);
    }
    #endregion
    #region Move
    void CalculateMove()
    {
        if (HasStateAuthority)
        {
            
            characterControllerPrototype.Move(moveDirection.normalized  * 0.02f * playerStat.moveSpeed *Runner.DeltaTime);
            Quaternion look=Quaternion.LookRotation(moveDirection.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, 360 * Runner.DeltaTime);
        }
    }
    private void CalculateAnimSpeed(string animationName, float speed, bool isMoveX)
    {
        if (isMoveX)
        {
            currentSpeedX = speed;
        }
        else
        {
            currentSpeedY = speed;
        }

        if (isMoveX && previousSpeedX != currentSpeedX)
        {
            CaculateSmoothAnimation(animationName, true, speed);
        }
        if (!isMoveX && previousSpeedY != currentSpeedY)
        {
            CaculateSmoothAnimation(animationName, false, speed);
        }

        if (isMoveX)
        {
            previousSpeedX = speed;
        }
        else
        {
            previousSpeedY = speed;
        }
    }

    void CaculateSmoothAnimation(string animationName, bool isMoveX, float? Speedtarget = null)
    {
        float time = 0;
        float start = animator.GetFloat(animationName);
        float x = Speedtarget == null ? 2 : 5;
        float targetTime = 1 / x;
        while (time <= targetTime)
        {
            if (Speedtarget != null
                && Speedtarget != (isMoveX ? currentSpeedX : currentSpeedY))
            {
                time = targetTime;
                break;
            }
            float valueRandomSmooth = Mathf.Lerp(start, Speedtarget == null ?
                (isMoveX ? targetX : targetY) : Speedtarget.Value, x * time);
            animator.SetFloat(animationName, valueRandomSmooth);
            time += Runner.DeltaTime;
        }
    }
    void SpawnAtStartPos()
    {
        Vector3 directionSpawn = spawnTransform.position - transform.position;
        characterControllerPrototype.Move(directionSpawn);
        transform.rotation = spawnTransform.rotation;
    }
    #endregion
    
    #region State
    public void SwithCharacterState(int newstate)
    {
        switch (state)
        {
            case 0: { break; }
            case 1: { break; }
            case 2: { break; }
            case 3: { break; }
        }
        switch (newstate)
        {
            case 0: { break; }
            case 1: { break; }
            case 2: { animator.SetTrigger("Injured"); break; }
            case 3:
                {
                    animator.SetTrigger("Die");
                    break;
                }
        }
        state = newstate;
    }
    public int GetCurrentState()
    {
        return state;
    }

    #endregion
    
    #region Collider
   
    private void OnTriggerStay(Collider otherColi)
    {
        if (!HasStateAuthority) return;
        if (!collisionsEnvi.Contains(otherColi) && otherColi.gameObject.CompareTag("Environment")) //Nếu là environment
        {
            collisionsEnvi.Add(otherColi);
        }
        buffsFromEnvi.maxHealth = 0;
        buffsFromEnvi.maxMana = 0;
        buffsFromEnvi.damage = 0;
        buffsFromEnvi.defend = 0;
        buffsFromEnvi.magicResistance = 0;
        buffsFromEnvi.magicAmpli = 0;
        buffsFromEnvi.criticalChance = 0;
        buffsFromEnvi.criticalDamage = 0;
        buffsFromEnvi.moveSpeed = 0;
        buffsFromEnvi.attackSpeed = 0;
        foreach (var other in collisionsEnvi)
        {
            if (other != null && other.GetComponent<NetworkObject>().IsValid)
            {
                if (other.GetComponent<EnvironmentObjects>().playerTeam == playerTeam)
                {
                    BuffsOfPlayer buffs = other.GetComponent<BuffsOfPlayer>();
                    buffsFromEnvi.maxHealth += buffs.maxHealth;
                    buffsFromEnvi.maxMana += buffs.maxMana;
                    buffsFromEnvi.damage += buffs.damage;
                    buffsFromEnvi.defend += buffs.defend;
                    buffsFromEnvi.magicResistance += buffs.magicResistance;
                    buffsFromEnvi.magicAmpli += buffs.magicAmpli;
                    buffsFromEnvi.criticalChance += buffs.criticalChance;
                    buffsFromEnvi.criticalDamage += buffs.criticalDamage;
                    buffsFromEnvi.moveSpeed += buffs.moveSpeed;
                    buffsFromEnvi.attackSpeed += buffs.attackSpeed;
                }
                else
                {
                    BuffsOfPlayer buffs = other.GetComponent<BuffsOfPlayer>();
                    buffsFromEnvi.maxHealth -= buffs.maxHealth;
                    buffsFromEnvi.maxMana -= buffs.maxMana;
                    buffsFromEnvi.damage -= buffs.damage;
                    buffsFromEnvi.defend -= buffs.defend;
                    buffsFromEnvi.magicResistance -= buffs.magicResistance;
                    buffsFromEnvi.magicAmpli -= buffs.magicAmpli;
                    buffsFromEnvi.criticalChance -= buffs.criticalChance;
                    buffsFromEnvi.criticalDamage -= buffs.criticalDamage;
                    buffsFromEnvi.moveSpeed -= buffs.moveSpeed;
                    buffsFromEnvi.attackSpeed -= buffs.attackSpeed;
                }
            }

        }
    }
    private void OnTriggerExit(Collider otherColi)
    {
        if (collisionsEnvi.Contains(otherColi))
        {
            collisionsEnvi.Remove(otherColi);
        }
    }
    #endregion
    #region Apply Damage
    public void ApplyDamage(int damage, bool isPhysicDamage, PlayerController player,
        Action<int, bool> counter = null, Action<int, List<PlayerController>> isKillPlayer = null,
        Action<int> lifeSteal = null, bool activeInjureAnim = true)
    {
        CalculateHealthRPC(damage, isPhysicDamage, player, activeInjureAnim);
        if (playerStat.isCounter)
        {
            counter?.Invoke(playerStat.counterDamage, isPhysicDamage);
        }

        if (state == 3)
        {
            isKillPlayer?.Invoke(playerStat.level, playerScore.playersMakeDamages);
        }
        lifeSteal?.Invoke(damage);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateHealthRPC
        (int damage, bool isPhysicDamage, PlayerController player, bool activeInjureAnim = true)
    {
        if (state == 3) return;
        if (!playerScore.playersMakeDamages.Contains(player))
        {
            playerScore.playersMakeDamages.Add(player);
        }
        if ((playerStat.currentHealth + statusCanvas.GetCurrentDamageAbsorbShield()) > damage)
        {
            if (statusCanvas.GetCurrentDamageAbsorbShield() > 0)
            {
                statusCanvas.ReduceDamageAbsoreShield(damage, out int overBalanceDmg);
                playerStat.currentHealth -= overBalanceDmg;
            }
            else
            {
                playerStat.currentHealth -= damage;
            }
        }
        else
        {
            WhenPlayerDie();
        }
    }
    void WhenPlayerDie()
    {
        playerStat.currentHealth = 0;
        SwithCharacterState(3);
        playerStat.isBeingStun = false; playerStat.isBeingSlow = false; playerStat.isBeingSilen = false;
        playerStat.isLive = false;
        foreach (var playerDamage in playerScore.playersMakeDamages)
        {
            if(playerDamage)
            {
                CalculateWhenKill(playerDamage);
            }
            
        }
        timeDie = TickTimer.CreateFromSeconds(Runner, 5 + 2 * playerStat.level); //thời gian hồi sinh
        StartCoroutine(DelayDie());
    }
    void CalculateWhenKill(PlayerController playerDamage)
    {
        playerDamage.playerStat.GainXPWhenKill((int)100 * playerStat.level / playerScore.playersMakeDamages.Count);
        playerDamage.GetComponent<Tesla>()?.PassiveWhenKill();
    }
    void CalculateEXP()
    {
        if (playerStat.currentXP >= playerStat.maxXP)
        {
            playerStat.currentXP -= playerStat.maxXP;
            playerStat.UpgradeLevel();
        }
    }
    public IEnumerator DelayDie()
    {
        yield return new WaitForSeconds(3f);
        Runner.Despawn(GetComponent<NetworkObject>());
    }
    
    [Networked] public TickTimer timeDie { get; set; }
    #endregion

    #region Apply Effect
    public void ApplyEffect(PlayerRef player, bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float TimeEffect = 0, Action callback = null)
    {
        CalculateEffectRPC(player, isMakeStun, isMakeSlow, isMakeSilen, TimeEffect);
        callback?.Invoke();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateEffectRPC(PlayerRef player, bool isMakeStun = false,
        bool isMakeSlow = false, bool isMakeSilen = false, float TimeEffect = 0)
    {
        if (playerStat.isUnstopAble || !playerStat.isLive) return;
        if (isMakeStun)
        {
            if (TimeOfStunDebuff.RemainingTime(Runner) == null ||
                TimeEffect >= (float)TimeOfStunDebuff.RemainingTime(Runner))
            {
                TimeOfStunDebuff = TickTimer.CreateFromSeconds(Runner, TimeEffect);
                maxStunTimeStatus = TimeEffect;
                currentStunTimeStatus = TimeEffect;
            }
            playerStat.isBeingStun = true;
        }
        if (isMakeSilen)
        {
            if (TimeEffect >= (float)TimeOfSilenDebuff.RemainingTime(Runner))
            {
                TimeOfSilenDebuff = TickTimer.CreateFromSeconds(Runner, TimeEffect);
                maxSilenTimeStatus = TimeEffect;
                currentSilenTimeStatus = TimeEffect;
            }
            playerStat.isBeingSilen = true;
        }
        if (isMakeSlow)
        {
            if (TimeEffect >= (float)TimeOfSlowDebuff.RemainingTime(Runner))
            {
                TimeOfSlowDebuff = TickTimer.CreateFromSeconds(Runner, TimeEffect);
                maxSlowTimeStatus = TimeEffect;
                currentSlowTimeStatus = TimeEffect;
            }
            playerStat.isBeingSlow = true;
        }

    }
    public void ApplyHeal(int heal)
    {
        CalculateHealRPC(heal);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateHealRPC(int heal)
    {
        if (state == 3) return;
        playerStat.currentHealth += heal;
    }
    #endregion
    #region Status Canvas
    void CalculateStatusDebuff()
    {

        if (HasStateAuthority)
        {
            if (TimeOfStunDebuff.RemainingTime(Runner) > 0)
            {
                currentStunTimeStatus = (float)TimeOfStunDebuff.RemainingTime(Runner);
            }
            else if (TimeOfSilenDebuff.RemainingTime(Runner) > 0)
            {
                currentSilenTimeStatus = (float)TimeOfSilenDebuff.RemainingTime(Runner);
            }
            else if (TimeOfSlowDebuff.RemainingTime(Runner) > 0)
            {
                currentSlowTimeStatus = (float)TimeOfSlowDebuff.RemainingTime(Runner);
            }
        }

        if (HasStateAuthority && TimeOfStunDebuff.Expired(Runner))
        {
            playerStat.isBeingStun = false;
        }
        statusDebuffs[0].SetActive(playerStat.isBeingStun);
        if (HasStateAuthority && TimeOfSlowDebuff.Expired(Runner))
        {
            playerStat.isBeingSlow = false;
        }
        statusDebuffs[1].SetActive(playerStat.isBeingSlow);
        if (HasStateAuthority && TimeOfSilenDebuff.Expired(Runner))
        {
            playerStat.isBeingSilen = false;
        }
        statusDebuffs[2].SetActive(playerStat.isBeingSilen);
    }
    void CalculateCanvas()
    {
        statusCanvas.TimeRemainingBar.gameObject.SetActive
            ((TimeOfStunDebuff.RemainingTime(Runner) > 0 || TimeOfSilenDebuff.RemainingTime(Runner) > 0
            || TimeOfSlowDebuff.RemainingTime(Runner) > 0) && state != 3);

        if (TimeOfStunDebuff.RemainingTime(Runner) > 0)
        {
            statusCanvas.TimeRemainingBar.UpdateBar(currentStunTimeStatus, maxStunTimeStatus);
        }
        else if (TimeOfSilenDebuff.RemainingTime(Runner) > 0)
        {
            statusCanvas.TimeRemainingBar.UpdateBar(currentSilenTimeStatus, maxSilenTimeStatus);
        }
        else if (TimeOfSlowDebuff.RemainingTime(Runner) > 0)
        {
            statusCanvas.TimeRemainingBar.UpdateBar(currentSlowTimeStatus, maxSlowTimeStatus);
        }
        statusCanvas.healthBarPlayer.UpdateBar(playerStat.currentHealth, playerStat.maxHealth);
        statusCanvas.statusBeingTMP.text =
         (playerStat.isBeingStun ? "Stunned " : "") + (playerStat.isBeingSlow ? "Slowed " : "") + (playerStat.isBeingSilen ? "Silened " : "");

        //xoay các bar để mọi player nhìn rõ
    }
    #endregion
    
}

