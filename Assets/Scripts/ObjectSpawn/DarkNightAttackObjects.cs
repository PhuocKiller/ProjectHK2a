using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkNightAttackObjects: NetworkBehaviour
{
    PlayerController player;
    private Vector3 direction;
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage, levelSkill;
    string skillName;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        if (HasStateAuthority && HasInputAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
                   }
    }
    public void SetUp(PlayerController player,string skillName, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, bool isDestroyWhenCollider = false, int levelSkill=1)
    {
        this.player = player;
        this.skillName = skillName;
        transform.SetParent(parentObject);
        damage = levelDamage;
        this.isPhysicDamage = isPhysicDamage;
        this.isMakeStun = isMakeStun;
        this.isMakeSlow = isMakeSlow;
        this.isMakeSilen = isMakeSilen;
        this.timeEffect = timeEffect;
        this.isDestroyWhenCollider = isDestroyWhenCollider;
        timerDespawn = timeTrigger;
        this.levelSkill = levelSkill;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority && timer.Expired(Runner))
        {
            Destroy(gameObject);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority
            && other.gameObject.layer == 7 && collisions.Count == 0
            && other.gameObject.GetComponent<NetworkObject>().HasStateAuthority == false
            && other.gameObject.GetComponent<PlayerController>().state != 3
            && other.gameObject.GetComponent<PlayerController>().playerTeam!=player.playerTeam)
        {
            collisions.Add(other);
            other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage(CalculateAttackDamage(other.gameObject.GetComponent<PlayerController>())
                , isPhysicDamage, player,
                counter: (int counterDamage, bool isPhysicDamage) =>
                {
                    player.ApplyDamage(counterDamage, isPhysicDamage,
                         other.gameObject.GetComponent<PlayerController>());
                }
                , isKillPlayer: (int levelHeroKilled, List<PlayerController> playerMakeDamage) => // Nhận exp khi giêt địch ở đây
                {
                    player.playerStat.currentXP += (int)(100 * Mathf.Lerp(1 / playerMakeDamage.Count, 1, 0.5f) * levelHeroKilled);
                    player.playerScore.killScore += 1;
                    player.playerScore.assistScore -= 1;
                }
                , lifeSteal: (int damage) =>
                {
                    if (player.playerStat.isLifeSteal) player.playerStat.currentHealth += (int)(player.playerStat.lifeSteal * damage);
                }
                );
            other.gameObject.GetComponent<ICanTakeDamage>().ApplyEffect(Object.InputAuthority, isMakeStun, isMakeSlow, isMakeSilen,
                TimeEffect: timeEffect, callback: () =>
                {
                    if (isDestroyWhenCollider) Destroy(gameObject);//khi chạm vào địch thì hủy vật thể
                }
                );
        }
    }
    int CalculateAttackDamage(PlayerController playerBeingAttack)
    {
        if (skillName=="NormalAttack")
        {
            return damage + (int)(playerBeingAttack.playerStat.currentHealth * 0.015 * ((int)(player.playerStat.level / 3) + 1));
        }
        else if (skillName =="Ultimate")
        {
            return damage + (int)((playerBeingAttack.playerStat.maxHealth- playerBeingAttack.playerStat.currentHealth) * 0.25
                * (int)(player.playerStat.level / 3) );
        }
        else { return 0; }
        
    }
    
}
