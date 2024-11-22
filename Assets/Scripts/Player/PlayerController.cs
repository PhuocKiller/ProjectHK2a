using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static Fusion.SimulationInput;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class PlayerController : NetworkBehaviour, ICanTakeDamage
{
    public NetworkProjectConfigAsset projectConfig;
    public Joystick joystick;
    [Networked]  public int playerTeam { get; set; }
    public ListNetworkObject networkObjs;
    public List<Collider> collisionsEnvi = new List<Collider>();
    public BuffsOfPlayer buffsFromEnvi;
    public PlayerStat playerStat;
    public StatusCanvas statusCanvas;
    Vector2 moveInput;
    Vector3 moveDirection;
    public CharacterController characterControllerPrototype;
    Animator animator;
    float speed;
    private int targetX, targetY, beforeTarget;
    float previousSpeedX, currentSpeedX, previousSpeedY, currentSpeedY;
    [HideInInspector] public bool isGround;
    [Networked]
    bool isJumping { get; set; }
    [Networked]
    bool isBasicAttackAttack { get; set; }
    [Networked]
    float jumpHeight { get; set; }
    Vector3 velocity;


    // 0 là normal
    // 1 là jump
    // 2 là injured
    // 3 là die
    // 4 là active attack
    // 5 là đang cast skill
    [Networked(OnChanged = nameof(listenState))]
    [SerializeField]
    public int state { get; set; }

    [SerializeField]
    public Transform jumpTransform,normalAttackTransform, skill_1Transform, skill_2Transform, ultimateTransform, rayCastTransform, transformCamera;
    
    [SerializeField] public Player_Types playerType;
    [HideInInspector] public SkillButton[] skillButtons;
    [SerializeField] GameObject[] statusDebuffs;
    
    [SerializeField] [Networked] TickTimer TimeOfStunDebuff { get; set; }
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
            Singleton<CameraController>.Instance.SetFollowCharacter(transform);
            Singleton<PlayerManager>.Instance.SetRunner(Runner);
            playerStat.UpgradeLevel();
            TimeOfStunDebuff = TickTimer.CreateFromSeconds(Runner,0);
            TimeOfSlowDebuff = TickTimer.CreateFromSeconds(Runner, 0);
            TimeOfSilenDebuff = TickTimer.CreateFromSeconds(Runner, 0);
            statusCanvas=GetComponentInChildren<StatusCanvas>();
            joystick=FindObjectOfType<Joystick>();
        }
        
    }

    public void Start()
    {
        
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        CalculateCanvas();
        CalculateStatusDebuff();
        if (state == 3)return;

        if (!playerStat.isBeingStun && state != 4)
        {
            CalculateMove();
            CalculateJump();
        }
        animator.enabled = !playerStat.isBeingStun;
        CalculateEXP();
        animator.SetFloat("AttackSpeed", (float)playerStat.attackSpeed / 100);
        animator.SetFloat("MoveSpeed", (float)playerStat.moveSpeed / 300);
    }

    private void CalculateJump()
    {
        if (HasStateAuthority)
        {
            if (isJumping)
            {
                isGround = false;
                isJumping = false;
                velocity += new Vector3(0, 50f, 0);
            }
            if (isGround)
            {
                velocity.y = 0;
                characterControllerPrototype.Move(velocity * Runner.DeltaTime);
            }
            else
            {
                velocity += new Vector3(0, -100f * Runner.DeltaTime, 0);

                characterControllerPrototype.Move(velocity * Runner.DeltaTime);
            }
        }
    }

    public void Jump(NetworkObject VFXEffect)
    {
        isJumping = true;
        AnimatorRPC("Jump");
        NetworkObject jumpVFX= Runner.Spawn(VFXEffect, jumpTransform.transform.position,
            jumpTransform.rotation, inputAuthority: Object.InputAuthority);
        StartCoroutine(DespawnJumpVFX(jumpVFX));
    }
    IEnumerator DespawnJumpVFX(NetworkObject jumpVFX)
    {
        yield return new WaitForSeconds(0.5f);
        Runner.Despawn(jumpVFX);
    }
    

    #region "SkillButton"
    public virtual void NormalAttack(NetworkObject VFXEffect, int levelDamage, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f, float TimeEffect = 0f)
    {
        AnimatorRPC("Attack");
    }
    public virtual void Skill_1(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        AnimatorRPC("Skill_1");
        playerStat.currentMana -= manaCost;
    }

    public virtual void Skill_2(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp= null,int levelSkill = 1)
    {
        AnimatorRPC("Skill_2");
        playerStat.currentMana -= manaCost;
    }
    public virtual void Ultimate(NetworkObject VFXEffect, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float timeTrigger = 0f, float TimeEffect = 0f, Vector3? posMouseUp = null, int levelSkill = 1)
    {
        AnimatorRPC("Ultimate");
        playerStat.currentMana -= manaCost;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void AnimatorRPC(string name)
    {
        animator.SetTrigger(name);
    }
    #endregion
    void Update()
    {
        if (state == 3 || state == 4)
        {
            return;
        }

        //   moveInput = state != 5 ? characterInput.Character.Move.ReadValue<Vector2>() : Vector2.zero;
        if (HasStateAuthority)
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (moveInput.magnitude == 0)
            {
                moveInput = new Vector2(joystick.Horizontal, joystick.Vertical).normalized;
            }
        }
        
        if (isGround)
        {
            velocity = Vector3.zero;
        }
        
    }
   
    void CalculateMove()
    {
        if (HasStateAuthority)
        {

            moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            CalculateAnimSpeed("MoveX", moveInput.x, true);
            CalculateAnimSpeed("MoveY", moveInput.y, false);
            speed = 2f + Vector2.Dot(moveInput, Vector2.up);
            Quaternion look = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
            if (moveDirection.magnitude > 0)
            {
            characterControllerPrototype.Move(look * moveDirection * speed * 0.015f
                *playerStat.moveSpeed*(playerStat.isBeingSlow ?0.3f:1f) * Runner.DeltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, 360 * Runner.DeltaTime);
            }
        
        }
    }
    void CalculateEXP()
    {
        if (playerStat.currentXP >= playerStat.maxXP)
        {
            playerStat.currentXP -= playerStat.maxXP;
            playerStat.UpgradeLevel();
        }
    }
    protected static void listenState(Changed<PlayerController> changed)
    {

    }
    public void SwithCharacterState(int newstate)
    {
        //Khi ket thuc trang thai cu thi toi lam gi do...
        switch (state)
        {
            case 0: { break; }
            case 1: { break; }
            case 2: { break; }
            case 3: { break; }
        }
        //Bat dau trang thai moi thi toi lam gi do...
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
            //doi lai 1 khung hinh
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

    public void CheckCamera(PlayerRef player, bool isFollow)
    {
        if (player == Runner.LocalPlayer)
        {
            if (isFollow)
            {
                Singleton<CameraController>.Instance.SetFollowCharacter(transform);
            }
            else
            {
                Singleton<CameraController>.Instance.RemoveFollowCharacter();
            }
        }
    }
    #region Collider
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Ground") && !isGround)
        {
            isGround = true;
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
            if (other != null &&other.GetComponent<NetworkObject>().IsValid)
            {
                if(other.GetComponent<EnvironmentObjects>().playerTeam ==playerTeam)
                {
                    BuffsOfPlayer buffs = other.GetComponent<BuffsOfPlayer>();
                    buffsFromEnvi.maxHealth += buffs.maxHealth;
                    buffsFromEnvi.maxMana += buffs.maxMana ;
                    buffsFromEnvi.damage += buffs.damage ;
                    buffsFromEnvi.defend += buffs.defend ;
                    buffsFromEnvi.magicResistance += buffs.magicResistance ;
                    buffsFromEnvi.magicAmpli += buffs.magicAmpli ;
                    buffsFromEnvi.criticalChance += buffs.criticalChance ;
                    buffsFromEnvi.criticalDamage += buffs.criticalDamage ;
                    buffsFromEnvi.moveSpeed += buffs.moveSpeed ;
                    buffsFromEnvi.attackSpeed += buffs.attackSpeed ;
                }
                else
                {
                    BuffsOfPlayer buffs = other.GetComponent<BuffsOfPlayer>();
                    buffsFromEnvi.maxHealth -= buffs.maxHealth ;
                    buffsFromEnvi.maxMana -= buffs.maxMana ;
                    buffsFromEnvi.damage -= buffs.damage ;
                    buffsFromEnvi.defend -= buffs.defend ;
                    buffsFromEnvi.magicResistance -= buffs.magicResistance ;
                    buffsFromEnvi.magicAmpli -= buffs.magicAmpli ;
                    buffsFromEnvi.criticalChance -= buffs.criticalChance ;
                    buffsFromEnvi.criticalDamage -= buffs.criticalDamage ;
                    buffsFromEnvi.moveSpeed -= buffs.moveSpeed ;
                    buffsFromEnvi.attackSpeed -= buffs.attackSpeed ;
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
    public void ApplyDamage(int damage, bool isPhysicDamage, PlayerRef player,
        Action<int> counter = null, Action<int> isKillPlayer = null, bool activeInjureAnim = true)
    {
        CalculateHealthRPC(damage, isPhysicDamage, player, activeInjureAnim);
        if(playerStat.isCounter)
        {
            counter?.Invoke(playerStat.counterDamage);
        }
        
        if (state==3)
        {
            isKillPlayer?.Invoke(playerStat.level);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CalculateHealthRPC
        (int damage, bool isPhysicDamage, PlayerRef player, bool activeInjureAnim = true)
    {
        if (state == 3) return;
        if ((playerStat.currentHealth + statusCanvas.GetCurrentDamageAbsorbShield()) > damage)
        {
            if (activeInjureAnim) SwithCharacterState(2);
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
            playerStat.currentHealth = 0;
            SwithCharacterState(3);
        }
    }
    public void ApplyEffect(PlayerRef player,bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float TimeEffect = 0, Action callback = null)
    {
        CalculateEffectRPC(player,isMakeStun,isMakeSlow,isMakeSilen,TimeEffect);
        callback?.Invoke();
    }
    [Rpc(RpcSources.All, RpcTargets.All)] public void CalculateEffectRPC(PlayerRef player, bool isMakeStun = false,
        bool isMakeSlow = false, bool isMakeSilen = false, float TimeEffect = 0)
    {
        if (playerStat.isUnstopAble) return;
        if(isMakeStun)
        {
            if (TimeOfStunDebuff.RemainingTime(Runner) == null||
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
            if(TimeEffect>=(float)TimeOfSlowDebuff.RemainingTime(Runner))
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
    [Rpc(RpcSources.All, RpcTargets.All)] public void CalculateHealRPC(int heal)
    {
        if (state == 3) return;
        playerStat.currentHealth += heal;
    }
    void CalculateStatusDebuff()
    {
        if(state==3) { playerStat.isBeingStun = false;playerStat.isBeingSlow = false;playerStat.isBeingSilen = false; }
        
        if (HasStateAuthority)
        {
            if (TimeOfStunDebuff.RemainingTime(Runner)>0)
            {
                currentStunTimeStatus = (float)TimeOfStunDebuff.RemainingTime(Runner); 
            }
            else if (TimeOfSilenDebuff.RemainingTime(Runner) > 0)
            {
                currentSilenTimeStatus = (float)TimeOfSilenDebuff.RemainingTime(Runner); 
            }
            else if(TimeOfSlowDebuff.RemainingTime(Runner) > 0)
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
            ((TimeOfStunDebuff.RemainingTime(Runner) > 0|| TimeOfSilenDebuff.RemainingTime(Runner) > 0
            || TimeOfSlowDebuff.RemainingTime(Runner) > 0)&& state!=3);

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

    public Player_Types GetPlayerTypes()
    {
        return playerType;
    }
    private void OnTriggerEnter(Collider other)
    {
        InventoryItemBase item = other.GetComponent<InventoryItemBase>();
        if (item != null)
        {
            Singleton<Inventory>.Instance.AddItem(item);
            item.OnPickUp();
        }
    }
 
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SkillRPC(int objectList, int levelDamage, int manaCost, bool isPhysicDamage,
        bool isMakeStun, bool isMakeSlow, bool isMakeSilen, float timeTrigger = 0f, float TimeEffect = 0f, int levelSkill = 1)
    {
        if(HasStateAuthority)
        {
            NetworkObject obj = Runner.Spawn(networkObjs.listNetworkObj[objectList], transform.position, transform.rotation,
                       Object.InputAuthority,
          onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
          {
              AttackObjects attObj = obj.GetComponent<AttackObjects>();
              if(attObj)
              {
                  attObj.SetUp(this, 0, isPhysicDamage, null,
                isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
              }
              DumbleAttackObjects dumObj = obj.GetComponent<DumbleAttackObjects>();
              if(dumObj)
              {
                  dumObj.SetUp(this, levelDamage, isPhysicDamage, null,
                isMakeStun, isMakeSlow, isMakeSilen, timeTrigger, TimeEffect);
              }
              Shield newshield = obj.GetComponent<Shield>();
              if(newshield)
              {
                  newshield.maxDamageAbsorb = levelDamage;
                  newshield.currentDamageAbsorb = levelDamage;
              }
              BuffsOfPlayer buff= obj.GetComponent<BuffsOfPlayer>();
              if(buff)
              {
                  buff.levelSkill = levelSkill;
                  Debug.Log(levelSkill);
                  if (buff.canHeal)
                  {
                      playerStat.currentHealth += levelDamage;
                  }
              }
             

          });
            SetParentRPC(obj.Id);
        }
        
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetParentRPC(NetworkId id)
    {
       if (! Runner.TryFindObject(id, out NetworkObject obj) ) return;
       if(obj.GetComponent<BuffsOfPlayer>() != null )
        {
            obj.transform.SetParent(transform.GetChild(2).GetChild(1));
        }
        if (obj.GetComponent<GrenadeController>() != null && playerType==Player_Types.Tesla)
        {
            obj.transform.SetParent(skill_2Transform);
        }
    }
}

