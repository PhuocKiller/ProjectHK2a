using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObjects : NetworkBehaviour
{
    PlayerController player;
    private Vector3 direction;
    private NetworkRigidbody rb;
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        rb = GetComponent<NetworkRigidbody>();
        if (HasStateAuthority && HasInputAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
            if (rb!=null)
            {
                isDestroyWhenCollider = true;
                rb.Rigidbody.AddForce(direction * 1500);
                transform.forward = direction;
            }
        }
    }
    public void SetUp(PlayerController player, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, bool isDestroyWhenCollider = false)
    {
        this.player = player;
        transform.SetParent(parentObject);
        damage = levelDamage;
        this.isPhysicDamage = isPhysicDamage;
        this.isMakeStun = isMakeStun;
        this.isMakeSlow = isMakeSlow;
        this.isMakeSilen = isMakeSilen;
        this.timeEffect = timeEffect;
        this.isDestroyWhenCollider = isDestroyWhenCollider;
        timerDespawn = timeTrigger;
    }
    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority && timer.Expired(Runner)
            )
        {
           Destroy(gameObject);
        }

    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority
            && other.gameObject.layer == 7 && collisions.Count == 0
            && other.gameObject.GetComponent<NetworkObject>().HasStateAuthority == false
            && other.gameObject.GetComponent<PlayerController>().state!=3)
        {
            collisions.Add(other);
            other.gameObject.GetComponent<ICanTakeDamage>().ApplyDamage(damage, isPhysicDamage, Object.InputAuthority,
                callback: () => 
                {
                  
                }
                ,isKillPlayer: (int levelHeroKilled)  => // Nhận exp khi giêt địch ở đây
                {
                    player.playerStat.currentXP += 100 * levelHeroKilled;
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
}
