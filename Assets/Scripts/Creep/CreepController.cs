using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Cinemachine.DocumentationSortingAttribute;

public enum Creep_Types
{
    Melee,
    Range,
    Natural,
}
public class CreepController : NetworkBehaviour, ICanTakeDamage
{
    private NavMeshAgent agent;
    public NetworkManager runnerManager;
    public CharacterController targetCharacterToChase, targetCharacterToAttack;
    public GameManager gameManager;
    public Joystick joystick;
    public OverlapSphereCreep overlapSphere;
    public NetworkObject normalMeleeAttackObj, normalRangeAttackObj;
    public Creep_Types creepType;
    public Vector3 targetDestination;
    [Networked] public Vector3 finalTargetDestination { get; set; }
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
    float previousSpeedX, currentSpeedX, previousSpeedY, currentSpeedY, attackRange;
    [Networked] public int state { get; set; }

    [SerializeField]
    public Transform normalAttackTransform;

    [SerializeField] GameObject[] statusDebuffs;
    [SerializeField] GameObject[] dropItems;
    float chanceDropItem;
    [SerializeField][Networked] TickTimer TimeOfStunDebuff { get; set; }
    [SerializeField][Networked] TickTimer TimeOfSlowDebuff { get; set; }
    [SerializeField][Networked] TickTimer TimeOfSilenDebuff { get; set; }
    [Networked] public TickTimer timeDie { get; set; }
    [Networked] float maxStunTimeStatus { get; set; }
    [Networked] float currentStunTimeStatus { get; set; }
    [Networked] float maxSilenTimeStatus { get; set; }
    [Networked] float currentSilenTimeStatus { get; set; }
    [Networked] float maxSlowTimeStatus { get; set; }
    [Networked] float currentSlowTimeStatus { get; set; }
    [Networked] public bool isMovingBack { get; set; }
    [SerializeField] GameObject[] visualRender;
    [SerializeField] Material[] teamMaterial;
    private void Awake()
    {
        characterControllerPrototype = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        statusCanvas = GetComponentInChildren<StatusCanvas>();
        overlapSphere = GetComponentInChildren<OverlapSphereCreep>();
    }
    public override void Spawned()
    {
        base.Spawned();
        runnerManager = FindObjectOfType<NetworkManager>();
        gameManager = FindObjectOfType<GameManager>();
        TimeOfStunDebuff = TickTimer.CreateFromSeconds(Runner, 0);
        TimeOfSlowDebuff = TickTimer.CreateFromSeconds(Runner, 0);
        TimeOfSilenDebuff = TickTimer.CreateFromSeconds(Runner, 0);
        playerStat.level = gameManager.levelCreep;
        playerStat.CalculateBaseStatForCreep();
        agent = GetComponent<NavMeshAgent>();
        if (creepType == Creep_Types.Melee || creepType == Creep_Types.Range)
        {
            RenderVisualCreep();
            finalTargetDestination = runnerManager.spawnPointBase[playerTeam == 0 ? 1 : 0].position;
        }
        chanceDropItem = 0.1f;
    }
    void RenderVisualCreep()
    {
        foreach (var visual in visualRender)
        {
            MeshRenderer visualRender = visual.GetComponent<MeshRenderer>();
            if (visualRender) visualRender.material = teamMaterial[playerTeam];
            SkinnedMeshRenderer visualSkinRender = visual.GetComponent<SkinnedMeshRenderer>();
            if (visualSkinRender) visualSkinRender.material = teamMaterial[playerTeam];
        }
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        CalculateCanvas();
        CalculateStatusDebuff();
        if (state != 2) animator.speed = playerStat.isBeingStun? 0:1;
        if (state == 3) return;
        if (HasStateAuthority)
        {
            if (creepType == Creep_Types.Natural
                && (transform.position - finalTargetDestination).magnitude > 50)
            {
                isMovingBack = true; playerStat.isBeingAttack = false;
            }
            if (!isMovingBack) //trong phạm vi ko quá xa điểm spawn
            {
                if (!playerStat.isBeingAttack) //natural ko bị tấn công
                {
                    if (overlapSphere.CheckAllEnemyAround(12).Count == 0)
                    {

                        targetDestination = finalTargetDestination;
                        AnimatorSetBoolRPC("isAttack", false);
                        state = 0;
                        if (Vector3.Distance(transform.position, targetDestination) < 2)
                        {
                            agent.isStopped = true;
                            AnimatorSetBoolRPC("isMove", false);
                        }
                        else
                        {
                            agent.isStopped = playerStat.isBeingStun;
                            AnimatorSetBoolRPC("isMove", true);
                        }
                    }
                    else //có enemy xung quanh
                    {
                        if (overlapSphere.CheckPlayerFollowEnemy(overlapSphere.CheckAllEnemyAround(12)).Count == 0)// nhưng ko có player follow
                        {
                            targetCharacterToChase = overlapSphere.FindClosestCharacterInRadius
                                (overlapSphere.CheckAllEnemyAround(12), transform.position);
                            CalculateMoveDirection();
                        }
                        else //có player follow
                        {
                            targetCharacterToChase = overlapSphere.FindClosestPlayerFollowInRadius
                                (overlapSphere.CheckPlayerFollowEnemy(overlapSphere.CheckAllEnemyAround(12)), transform.position)
                                .GetComponent<CharacterController>();
                            CalculateMoveDirection();
                        }
                    }
                }
                else // natural đang bị attack
                {
                    CalculateMoveDirection();
                }
            }
            else //vượt qua phạm vi điểm spawn
            {
                if (creepType == Creep_Types.Natural
                && (transform.position - finalTargetDestination).magnitude < 1) isMovingBack = false;
                targetDestination = finalTargetDestination;
                AnimatorSetBoolRPC("isAttack", false);
                agent.isStopped = playerStat.isBeingStun;
                state = 0;
            }

            Vector3 lookVector = new Vector3(targetDestination.x, transform.position.y, targetDestination.z) - transform.position;
            if (lookVector.magnitude > 0.01)
            {
                Quaternion look = Quaternion.LookRotation(lookVector.normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, look, 360 * Runner.DeltaTime);
            }
            if (!playerStat.isBeingStun && state != 4)
            {
                agent.SetDestination(targetDestination);
            }
        }
        animator.SetFloat("AttackSpeed", (float)playerStat.attackSpeed / 100);
        animator.SetFloat("MoveSpeed", (float)playerStat.moveSpeed / 300);
    }
    void CalculateMoveDirection()
    {
        targetDestination = targetCharacterToChase.transform.position;
        targetCharacterToAttack = overlapSphere.FindClosestCharacterInRadius
                (overlapSphere.CheckAllEnemyAround
                (creepType == Creep_Types.Melee ? 1.5f : (creepType == Creep_Types.Natural ? 3f : 9f)), transform.position);
        if (targetCharacterToAttack == targetCharacterToChase)
        {
            Attack();
        }
        else
        {
            DontAttack();
        }

    }
    void Attack()
    {
        AnimatorSetBoolRPC("isAttack", true);
        AnimatorSetBoolRPC("isMove", false);
        state = 4;
        agent.isStopped = true;
    }
    void DontAttack()
    {
        AnimatorSetBoolRPC("isAttack", false);
        AnimatorSetBoolRPC("isMove", true);
        state = 0;
        agent.isStopped = playerStat.isBeingStun;
    }

    #region "Attack"
    public virtual void NormalAttack()
    {
        if (HasStateAuthority)
        {
            if (creepType == Creep_Types.Melee || creepType == Creep_Types.Natural)
            {
                Runner.Spawn(normalMeleeAttackObj.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation
                     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                     {
                         obj.GetComponent<AttackObjectsCreep>().SetUpCreep(this, playerStat.damage, true, normalAttackTransform,
                             false, false, false, 0.5f, 0, isDestroyWhenCollider: true);
                     });
            }
            else if (creepType == Creep_Types.Range)
            {
                Runner.Spawn(normalRangeAttackObj.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation
                     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                     {
                         obj.GetComponent<AttackObjectsCreep>().SetUpCreep(this, playerStat.damage, true, null,
                             false, false, false, 1.5f, 0, isDestroyWhenCollider: true);
                         obj.GetComponent<AttackObjectsCreep>().SetDirection(transform.forward);
                     });
            }
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
        animator.SetBool(name, isActive);
    }
    #endregion
    #region Move
    void CalculateMove()
    {
        if (HasStateAuthority)
        {

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
    private void OnTriggerEnter(Collider other)
    {

    }
    private void OnTriggerExit(Collider other)
    {
        if (collisionsEnvi.Contains(other))
        {
            collisionsEnvi.Remove(other);
        }

    }
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

    #endregion
    #region Apply Damage
    public void ApplyDamage(int damage, bool isPhysicDamage, PlayerController player,
        Action<int, bool> counter = null, Action<int, List<PlayerController>> isKillPlayer = null,
        Action<Vector3, float> isKillCreep = null,
        Action<int> lifeSteal = null, bool activeInjureAnim = true, bool isCritPhysic = false)
    {
        CalculateHealthRPC(damage, isPhysicDamage, player, activeInjureAnim, isCritPhysic);
        if (creepType == Creep_Types.Natural)
        {
            targetCharacterToChase = player.GetComponent<CharacterController>();
            playerStat.isBeingAttack = true;
        }
        if (playerStat.isCounter)
        {
            counter?.Invoke(playerStat.counterDamage, isPhysicDamage);
        }

        if (state == 3)
        {
            isKillCreep?.Invoke(transform.position + Vector3.up * 0.5f, 0); //LuckyChanceBaseOnCreepType()
        }
        lifeSteal?.Invoke(damage);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateHealthRPC
        (int damage, bool isPhysicDamage, PlayerController player, bool activeInjureAnim = true, bool isCritPhysic = false)
    {
        if (!playerScore.playersMakeDamages.Contains(player))
        {
            playerScore.playersMakeDamages.Add(player);
        }
        if (state == 3) return;

        if ((playerStat.currentHealth + statusCanvas.GetCurrentDamageAbsorbShield()) > damage)
        {
            if (statusCanvas.GetCurrentDamageAbsorbShield() > 0)
            {
                statusCanvas.ReduceDamageAbsoreShield(damage, out int overBalanceDmg);
                playerStat.currentHealth -= overBalanceDmg;
                if (player) statusCanvas.PlayerHaveInjure(overBalanceDmg, isCritPhysic);
            }
            else
            {
                playerStat.currentHealth -= damage;
                if (player) statusCanvas.PlayerHaveInjure(damage, isCritPhysic);
            }
        }
        else
        {
            WhenCreepDie();
        }
    }
    void WhenCreepDie()
    {
        playerStat.currentHealth = 0;
        SwithCharacterState(3);
        playerStat.isBeingStun = false; playerStat.isBeingSlow = false; playerStat.isBeingSilen = false;
        playerStat.isLive = false; agent.isStopped = true;
        if (overlapSphere != null)
        {
            if (overlapSphere.CheckPlayerAround().Count > 0)
            {
                foreach (var playerAround in overlapSphere.CheckPlayerAround())
                {
                    if (playerAround)
                    {
                        CalculateXPWhenKill(playerAround);
                        CalculateCoinsWhenKill(playerAround);
                    }
                }
            }
        }
        GetComponent<CharacterController>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
    }
    public void CreepDie()
    {
        if (HasStateAuthority)
        {
            if (creepType == Creep_Types.Natural) SpawnItemWhenDie();
            Destroy(gameObject);
        }
    }
    void SpawnItemWhenDie()
    {
        for (int i = 0; i < dropItems.Length; i++)
        {
            if (FindObjectOfType<MechanicDamage>().GetChance(chanceDropItem * (6 - i)))
            {

                Runner.Spawn(dropItems[i], transform.position + Vector3.up);
                return;
            }
        }
    }
    #endregion
    #region XP,Coin
    void CalculateXPWhenKill(PlayerController playerAround)
    {
        playerAround.playerStat.GainXPWhenKill((int)(EXPBaseOnCreepType(playerStat.level) / overlapSphere.CheckPlayerAround().Count));
    }
    void CalculateCoinsWhenKill(PlayerController playerAround)
    {
        playerAround.playerStat.GainCoinWhenKill((int)(CoinBaseOnCreepType(playerStat.level) / overlapSphere.CheckPlayerAround().Count));
    }
    int EXPBaseOnCreepType(int level)
    {
        if (creepType == Creep_Types.Melee)
        {
            return 20 + (level - 1) * 5;
        }
        else if (creepType == Creep_Types.Range)
        {
            return 30 + (level - 1) * 8;
        }
        else if (creepType == Creep_Types.Natural)
        {
            return 200 + (level - 1) * 8;
        }
        else
        {
            return 0;
        }
    }
    int CoinBaseOnCreepType(int level)
    {
        if (creepType == Creep_Types.Melee)
        {
            return 40 + (level - 1) * 5;
        }
        else if (creepType == Creep_Types.Range)
        {
            return 50 + (level - 1) * 8;
        }
        else if (creepType == Creep_Types.Natural)
        {
            return 250 + (level - 1) * 8;
        }
        else
        {
            return 0;
        }
    }
    float LuckyChanceBaseOnCreepType()
    {
        if (creepType == Creep_Types.Melee)
        {
            return 1;
        }
        else if (creepType == Creep_Types.Range)
        {
            return 0.5f;
        }
        else
        {
            return 0.3f;
        }
    }
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
        if (state != 3) statusCanvas.healthBarPlayer.UpdateBar(playerStat.currentHealth, playerStat.maxHealth);
        statusCanvas.statusBeingTMP.text =
         (playerStat.isBeingStun ? "Stunned " : "") + (playerStat.isBeingSlow ? "Slowed " : "") + (playerStat.isBeingSilen ? "Silened " : "");

        //xoay các bar để mọi player nhìn rõ
    }
    #endregion

}

