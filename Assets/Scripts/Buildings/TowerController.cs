using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.AI;
using UnityEngine.UIElements;
public class TowerController : NetworkBehaviour,ICanTakeDamage
{
    public NetworkManager runnerManager;
    public CharacterController targetCharacter;
    public GameManager gameManager;
    public OverlapSphereTower overlapSphere;
    public PlayerScore playerScore;
    public Bars hpBar;
    public Transform weapon, shootPosition;
    [Networked] public int playerTeam { get; set; }
     
    public CharacterController characterControllerPrototype;
   
    [Networked] public int state { get; set; }
    
    [Networked] public int currentHealth { get; set; }
    [Networked] public int maxHealth { get; set; }
    [Networked] public int damage { get; set; }
    [Networked] public int defend { get; set; }
    [Networked] public bool isLive { get; set; }
    [Networked] public bool isAttack { get; set; }
    [Networked] public TickTimer TimeOfAttack { get; set; }
    public Mesh[] meshTower;
    public MeshFilter[] meshFilters;
    public GameObject[] shootVFX;
  
    public override void Spawned()
    {
        base.Spawned();
        runnerManager = FindObjectOfType<NetworkManager>();
        gameManager = FindObjectOfType<GameManager>();
        hpBar = GetComponentInChildren<Bars>();
        maxHealth = 3000 + gameManager.levelCreep * 50;
        currentHealth = maxHealth;
        state = 0;
        characterControllerPrototype = GetComponent<CharacterController>();
        for (int i = 0;i<3;i++)
        {
            meshFilters[i].mesh=meshTower[i + 3*playerTeam];
        }
        TimeOfAttack = TickTimer.CreateFromSeconds(Runner, 1);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        defend = 10 + Mathf.FloorToInt(gameManager.levelCreep * 0.5f);
        damage=1 + gameManager.levelCreep * 2;
        if (state != 3)
        {
            hpBar.UpdateBar(currentHealth, maxHealth);
        }
        hpBar.transform.rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        if (HasStateAuthority)
        {
            if (overlapSphere.CheckAllEnemyAround().Count == 0)
            {
                Quaternion look = Quaternion.LookRotation
               ((runnerManager.spawnPointTeam[playerTeam == 0 ? 1 : 0].position - transform.position).normalized);
                weapon.rotation = Quaternion.RotateTowards(transform.rotation, look, 360 * Runner.DeltaTime);
                isAttack = false;
            }
            else //có enemy xung quanh
            {
                if (overlapSphere.CheckPlayerFollowEnemy(overlapSphere.CheckAllEnemyAround()).Count == 0)// nhưng ko có player follow
                {
                    targetCharacter = overlapSphere.FindClosestCharacterInRadius(overlapSphere.CheckAllEnemyAround(), transform.position);
                }
                else //có player follow
                {
                    targetCharacter = overlapSphere.FindClosestPlayerFollowInRadius
                        (overlapSphere.CheckPlayerFollowEnemy(overlapSphere.CheckAllEnemyAround()), transform.position)
                        .GetComponent<CharacterController>();
                }
                isAttack = true;
            }
           if(isAttack &&TimeOfAttack.Expired(Runner))
            {
                NormalAttack();
                TimeOfAttack = TickTimer.CreateFromSeconds(Runner, 1);
            }
        }
       
    }
  
    public virtual void NormalAttack()
    {
        Runner.Spawn(shootVFX[playerTeam], shootPosition.position, shootPosition.rotation, inputAuthority: Object.InputAuthority
       , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
         {
            obj.GetComponent<AttackObjectsTower>().SetUpTower(this, damage, null,3f);
            obj.GetComponent<AttackObjectsTower>().SetDirection(targetCharacter);
         });
            
    }
   
    public void ApplyDamage(int damage, bool isPhysicDamage, PlayerController player,
        Action<int, bool> counter = null, Action<int, List<PlayerController>> isKillPlayer = null,
        Action<Vector3, float> isKillCreep = null,
        Action<int> lifeSteal = null, bool activeInjureAnim = true, bool isCritPhysic = false)
    {
        CalculateHealthRPC(damage, isPhysicDamage, player, activeInjureAnim, isCritPhysic);
    }
    public void ApplyEffect(PlayerRef player, bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float TimeEffect = 0f, Action callback = null)
    {
        callback?.Invoke();
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

        if (currentHealth  > damage)
        {
          currentHealth -= damage;
        }
        else
        {
            WhenCreepDie();
        }
    }
    void WhenCreepDie()
    {
        currentHealth = 0;
        isLive = false;
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
        if (HasStateAuthority) StartCoroutine(TowerCollapse());
    }
    public IEnumerator TowerCollapse()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    [Networked] public TickTimer timeDie { get; set; }
    
    void CalculateXPWhenKill(PlayerController playerAround)
    {
      //  playerAround.playerStat.GainXPWhenKill((int)(EXPBaseOnCreepType(playerStat.level) / overlapSphere.CheckPlayerAround().Count));
    }
    void CalculateCoinsWhenKill(PlayerController playerAround)
    {
      //  playerAround.playerStat.GainCoinWhenKill((int)(CoinBaseOnCreepType(playerStat.level) / overlapSphere.CheckPlayerAround().Count));
    }
    /*int EXPBaseOnCreepType(int level)
    {
        if (creepType == Creep_Types.Melee)
        {
            return 20 + (level - 1) * 5;
        }
        else if (creepType == Creep_Types.Range)
        {
            return 30 + (level - 1) * 8;
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
        else
        {
            return 0;
        }
    }*/
    
}
