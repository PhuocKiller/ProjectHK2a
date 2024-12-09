using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.AI;
public class TowerController : NetworkBehaviour,ICanTakeDamage
{
    public NetworkManager runnerManager;
    public CharacterController targetCharacter;
    public GameManager gameManager;
    public OverlapSphereCreep overlapSphere;
    public PlayerScore playerScore;
    public Bars hpBar;
    public Transform weapon;
    [Networked] public int playerTeam { get; set; }
     
    public CharacterController characterControllerPrototype;
   
    [Networked] public int state { get; set; }
    
    [Networked] public int currentHealth { get; set; }
    [Networked] public int maxHealth { get; set; }
    [Networked] public int damage { get; set; }
    [Networked] public int defend { get; set; }
    [Networked] public bool isLive { get; set; }
    //public Mesh[] meshTower;
  
  
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
        Debug.Log("vo spawm" + GetComponent<NetworkObject>().Id);

    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        Debug.Log("vo update" + GetComponent<NetworkObject>().Id);

        if (state != 3)
        {
            hpBar.UpdateBar(currentHealth, maxHealth);
        }
        if (HasStateAuthority)
        {
            if (overlapSphere.CheckAllEnemyAround().Count == 0)
            {
                
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
            }
            Quaternion look = Quaternion.LookRotation
                ((runnerManager.spawnPointTeam[playerTeam == 0 ? 1 : 0].position - transform.position).normalized);
            weapon.rotation = Quaternion.RotateTowards(transform.rotation, look, 360 * Runner.DeltaTime);
        }
       
    }
  
    public virtual void NormalAttack()
    {
        if (HasStateAuthority)
        {
           /* if (creepType == Creep_Types.Melee)
            {
                Runner.Spawn(normalMeleeAttackObj.gameObject, normalAttackTransform.transform.position, normalAttackTransform.rotation, inputAuthority: Object.InputAuthority
                     , onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
                     {
                         obj.GetComponent<AttackObjectsCreep>().SetUpCreep(this, playerStat.damage, true, normalAttackTransform,
                             false, false, false, 0.5f, 0, isDestroyWhenCollider: true);
                     });
            }*/
        }
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
