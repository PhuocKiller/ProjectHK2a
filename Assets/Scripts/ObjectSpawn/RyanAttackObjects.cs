using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RyanAttackObjects: NetworkBehaviour
{
    PlayerController player;
    CalculateTriggerEnter trigger;
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
        trigger = GetComponent<CalculateTriggerEnter>();
        if (HasStateAuthority && HasInputAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
            StartCoroutine(player.GetComponent<Ryan>().ActiveKatana(true,0.7f*100/player.playerStat.attackSpeed));
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
            StartCoroutine(player.GetComponent<Ryan>().ActiveKatana(false, 0));
            Destroy(gameObject);
        }

    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority)
        {
            trigger.ControlTrigger(other, collisions, player, damage, timeEffect, isPhysicDamage,
                isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider, Object.InputAuthority);
        }
    }
}
